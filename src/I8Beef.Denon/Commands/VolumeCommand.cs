﻿using System;
using System.Text.RegularExpressions;

namespace I8Beef.Denon.Commands
{
    /// <summary>
    /// Denon Volume command class.
    /// </summary>
    public class VolumeCommand : Command
    {
        /// <inheritdoc/>
        public override string Code { get { return "MV"; } }

        /// <summary>
        /// Parses a commands string to return an instance of this <see cref="Command"/>.
        /// </summary>
        /// <param name="commandString">The command string to parse.</param>
        /// <returns>The <see cref="Command"/>.</returns>
        public static Command Parse(string commandString)
        {
            var matches = Regex.Match(commandString, @"^MV(UP|DOWN|\d\d\d?|\?)$");
            if (!matches.Success)
                throw new ArgumentException("Command string not recognized: " + commandString);

            var value = matches.Groups[1].Value;

            if (value.Length == 3)
                value = value.Substring(0, 2) + "." + value.Substring(2, 1);

            return new VolumeCommand { Value = value };
        }

        /// <inheritdoc/>
        public override string GetHttpCommand()
        {
            if (int.TryParse(Value, out int intVal))
                return $"MainZone/index.put.asp?cmd0=PutMasterVolumeSet/{intVal - 80}";

            var val = Value;
            if (val == "UP")
                val = ">";
            else if (val == "DOWN")
                val = "<";

            return $"MainZone/index.put.asp?cmd0=PutMasterVolumeBtn/{val}";
        }

        /// <inheritdoc/>
        public override string GetTelnetCommand()
        {
            return $"{Code}{Value.Replace(".", string.Empty)}";
        }
    }
}
