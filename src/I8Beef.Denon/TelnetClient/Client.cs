﻿using I8Beef.Denon.Commands;
using I8Beef.Denon.Events;
using I8Beef.Denon.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace I8Beef.Denon.TelnetClient
{
    public class Client : IClient
    {
        private bool _disposed;
        private readonly object _writeLock = new object();

        private string _host;
        private TcpClient _client;
        private NetworkStream _stream;

        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        // A lookup to correlate request and responses
        private readonly IDictionary<string, TaskCompletionSource<string>> _resultTaskCompletionSources = new ConcurrentDictionary<string, TaskCompletionSource<string>>();

        public Client(string host)
        {
            _host = host;
        }

        /// <summary>
        /// Connected.
        /// </summary>
        public bool Connected { get; private set; }

        /// <summary>
        /// The event that is raised when an unrecoverable error condition occurs.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// The event that is raised when messages are received.
        /// </summary>
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        /// <summary>
        /// The event that is raised when messages are sent.
        /// </summary>
        public event EventHandler<MessageSentEventArgs> MessageSent;

        /// <summary>
        /// The event that is raised when and event is received from the Denon unit.
        /// </summary>
        public event EventHandler<DenonMessage> EventReceived;

        /// <summary>
        /// Send command to the Denon.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>Awaitable Task.</returns>
        public async Task SendCommandAsync(string command)
        {
            await FireAndForgetAsync(command).ConfigureAwait(false);
        }

        /// <summary>
        /// Send command to the Denon.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>The response.</returns>
        public async Task<DenonMessage> SendQueryAsync(string command)
        {
            return ParseCommand(await RequestResponseAsync(command).ConfigureAwait(false));
        }

        /// <summary>
        /// Listens for incoming XML stanzas and raises the appropriate events.
        /// </summary>
        /// <remarks>This runs in the context of a separate thread. In case of an
        /// exception, the Error event is raised and the thread is shutdown.</remarks>
        private void ReadStream()
        {
            try
            {
                using (var reader = new StreamReader(_stream))
                {
                    while (!_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        var message = reader.ReadLine();
                        MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));

                        // Parse message
                        var command = ParseCommand(message);

                        TaskCompletionSource<string> resultTaskCompletionSource;
                        if (_resultTaskCompletionSources.TryGetValue(command.Code, out resultTaskCompletionSource))
                        {
                            // query response
                            resultTaskCompletionSource.TrySetResult(message);
                            _resultTaskCompletionSources.Remove(command.Code);
                        }
                        else
                        {
                            // event
                            EventReceived?.Invoke(this, command);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Stream has dropped, this happens naturally on Dispose
                if (e is IOException)
                {
                    Connected = false;
                }

                // An exception outside of Dispose should be raised to the caller to handle
                if (!_disposed)
                {
                    Error?.Invoke(this, new ErrorEventArgs(e));
                }
            }
        }

        private DenonMessage ParseCommand(string command)
        {
            var result = new DenonMessage();
            // TODO: Implement parsing logic

            // get command code based on most specific match
            var commandCode = TelnetCommand.Commands.Keys.OrderByDescending(x => x.Length).FirstOrDefault(x => command.StartsWith(x));
            if (!string.IsNullOrEmpty(commandCode))
            {
                result.Code = commandCode;
                result.Parameter = command.Substring(commandCode.Length);
                result.Value = command.Contains(" ") ? command.Substring(command.LastIndexOf(" ")) : command.Substring(commandCode.Length);
            }

            return result;
        }

        /// <summary>
        /// Send a message without waiting for response.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="timeout">Time to wait for an error response.</param>
        /// <returns>A <see cref="cref="Task"/>.</returns>
        private async Task FireAndForgetAsync(string message, int timeout = 50)
        {
            // Heartbeat check
            if (!Connected)
                throw new ConnectionException("Connection already closed or not authenticated");

            var commandCode = ParseCommand(message).Code;

            // Prepare the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<string>();
            if (!_resultTaskCompletionSources.ContainsKey(commandCode))
            {
                _resultTaskCompletionSources[commandCode] = resultTaskCompletionSource;

                // Start the sending
                Send(message);

                // Await, to make sure there wasn't an error
                var task = await Task.WhenAny(resultTaskCompletionSource.Task, Task.Delay(timeout)).ConfigureAwait(false);

                // Remove the result task, as we no longer need it.
                _resultTaskCompletionSources.Remove(commandCode);

                // This makes sure the exception, if there was one, is unwrapped
                await task;
            }
        }

        /// <summary>
        /// Sends a request response message.
        /// </summary>
        /// <param name="message">The message to be sent.</param>
        /// <param name="timeout">Timeout for the response, exception thrown on expiration.</param>
        /// <returns>Response message.</returns>
        private async Task<string> RequestResponseAsync(string message, int timeout = 2000)
        {
            if (!Connected)
                throw new ConnectionException("Connection already closed or not authenticated");

            var commandCode = ParseCommand(message).Code;

            // Prepate the TaskCompletionSource, which is used to await the result
            var resultTaskCompletionSource = new TaskCompletionSource<string>();
            if (!_resultTaskCompletionSources.ContainsKey(commandCode))
            {
                _resultTaskCompletionSources[commandCode] = resultTaskCompletionSource;

                // Create the action which is called when a timeout occurs
                Action timeoutAction = () =>
                {
                    _resultTaskCompletionSources.Remove(commandCode);
                    resultTaskCompletionSource.TrySetException(new TimeoutException($"Timeout while waiting on response {commandCode} after {timeout}"));
                };

                Send(message);

                // Handle timeout
                var cancellationTokenSource = new CancellationTokenSource(timeout);
                using (cancellationTokenSource.Token.Register(timeoutAction))
                {
                    // Await until response or timeout
                    return await resultTaskCompletionSource.Task.ConfigureAwait(false);
                }
            }

            // TODO: Something different here
            return "";
        }

        /// <summary>
        /// Sends the specified string to the server.
        /// </summary>
        /// <remarks>
        /// Do not use this method directly except for stream initialization and closing.
        /// </remarks>
        /// <param name="message">The string to send.</param>
        /// <exception cref="ArgumentNullException">The xml parameter is null.</exception>
        /// <exception cref="IOException">There was a failure while writing to the network.</exception>
        private void Send(string message)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            // Attach carriage return character if not present
            if (!message.EndsWith("\r"))
                message += "\r";

            byte[] buf = Encoding.ASCII.GetBytes(message);
            lock (_writeLock)
            {
                try
                {
                    _stream.Write(buf, 0, buf.Length);
                    MessageSent?.Invoke(this, new MessageSentEventArgs(message));
                }
                catch
                {
                    Connected = false;
                    throw;
                }
            }
        }

        #region Connection management

        /// <summary>
        /// Connect to Denon.
        /// </summary>
        public void Connect()
        {
            // Establish a connection
            if (_client == null)
                _client = new TcpClient(_host, 23);

            // Get stream handle
            if (_stream == null)
                _stream = _client.GetStream();

            // Set up the listener task after authentication has been handled
            Task.Factory.StartNew(ReadStream, _cancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            Connected = true;
        }

        /// <summary>
        /// Allows for explicit closing of session.
        /// </summary>
        public void Close()
        {
            // Stop parsing incoming feed
            _cancellationTokenSource.Cancel();

            // Dispose of stream
            if (_stream != null)
                _stream.Dispose();

            Connected = false;

            // Disconnect socket
            if (_client != null)
            {
                _client.Close();
                _client = null;
            }
        }

        #endregion

        #region IDisposable implementation

        /// <summary>
        /// Releases all resources used by the current instance of the XmppIm class.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
		/// Releases all resources used by the current instance of the XmppIm
		/// class, optionally disposing of managed resource.
		/// </summary>
		/// <param name="disposing">true to dispose of managed resources, otherwise
		/// false.</param>
		protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                // Indicate that the instance has been disposed.
                _disposed = true;

                // Get rid of managed resources.
                if (disposing)
                {
                    Close();
                }
            }
        }

        #endregion
    }
}