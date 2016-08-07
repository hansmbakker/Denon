﻿using System.Xml.Serialization;
using System.Collections.Generic;

namespace I8Beef.Denon.Schema.Status
{
    [XmlRoot(ElementName = "FriendlyName")]
    public class FriendlyName
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Power")]
    public class Power
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "ZonePower")]
    public class ZonePower
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "RenameZone")]
    public class RenameZone
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "TopMenuLink")]
    public class TopMenuLink
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "VideoSelectDisp")]
    public class VideoSelectDisp
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "VideoSelect")]
    public class VideoSelect
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "VideoSelectOnOff")]
    public class VideoSelectOnOff
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "value")]
    public class Value
    {
        [XmlAttribute(AttributeName = "index")]
        public string Index { get; set; }
        [XmlText]
        public string Text { get; set; }
        [XmlAttribute(AttributeName = "table")]
        public string Table { get; set; }
        [XmlAttribute(AttributeName = "param")]
        public string Param { get; set; }
    }

    [XmlRoot(ElementName = "VideoSelectLists")]
    public class VideoSelectLists
    {
        [XmlElement(ElementName = "value")]
        public List<Value> Value { get; set; }
    }

    [XmlRoot(ElementName = "ECOModeDisp")]
    public class ECOModeDisp
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "ECOMode")]
    public class ECOMode
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "ECOModeLists")]
    public class ECOModeLists
    {
        [XmlElement(ElementName = "value")]
        public List<Value> Value { get; set; }
    }

    [XmlRoot(ElementName = "AddSourceDisplay")]
    public class AddSourceDisplay
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "ModelId")]
    public class ModelId
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "BrandId")]
    public class BrandId
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "SalesArea")]
    public class SalesArea
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "InputFuncSelect")]
    public class InputFuncSelect
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "NetFuncSelect")]
    public class NetFuncSelect
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "selectSurround")]
    public class SelectSurround
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "VolumeDisplay")]
    public class VolumeDisplay
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "MasterVolume")]
    public class MasterVolume
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Mute")]
    public class Mute
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "RemoteMaintenance")]
    public class RemoteMaintenance
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "SubwooferDisplay")]
    public class SubwooferDisplay
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "Zone2VolDisp")]
    public class Zone2VolDisp
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "SleepOff")]
    public class SleepOff
    {
        [XmlElement(ElementName = "value")]
        public string Value { get; set; }
    }

    [XmlRoot(ElementName = "item")]
    public class Item
    {
        [XmlElement(ElementName = "FriendlyName")]
        public FriendlyName FriendlyName { get; set; }
        [XmlElement(ElementName = "Power")]
        public Power Power { get; set; }
        [XmlElement(ElementName = "ZonePower")]
        public ZonePower ZonePower { get; set; }
        [XmlElement(ElementName = "RenameZone")]
        public RenameZone RenameZone { get; set; }
        [XmlElement(ElementName = "TopMenuLink")]
        public TopMenuLink TopMenuLink { get; set; }
        [XmlElement(ElementName = "VideoSelectDisp")]
        public VideoSelectDisp VideoSelectDisp { get; set; }
        [XmlElement(ElementName = "VideoSelect")]
        public VideoSelect VideoSelect { get; set; }
        [XmlElement(ElementName = "VideoSelectOnOff")]
        public VideoSelectOnOff VideoSelectOnOff { get; set; }
        [XmlElement(ElementName = "VideoSelectLists")]
        public VideoSelectLists VideoSelectLists { get; set; }
        [XmlElement(ElementName = "ECOModeDisp")]
        public ECOModeDisp ECOModeDisp { get; set; }
        [XmlElement(ElementName = "ECOMode")]
        public ECOMode ECOMode { get; set; }
        [XmlElement(ElementName = "ECOModeLists")]
        public ECOModeLists ECOModeLists { get; set; }
        [XmlElement(ElementName = "AddSourceDisplay")]
        public AddSourceDisplay AddSourceDisplay { get; set; }
        [XmlElement(ElementName = "ModelId")]
        public ModelId ModelId { get; set; }
        [XmlElement(ElementName = "BrandId")]
        public BrandId BrandId { get; set; }
        [XmlElement(ElementName = "SalesArea")]
        public SalesArea SalesArea { get; set; }
        [XmlElement(ElementName = "InputFuncSelect")]
        public InputFuncSelect InputFuncSelect { get; set; }
        [XmlElement(ElementName = "NetFuncSelect")]
        public NetFuncSelect NetFuncSelect { get; set; }
        [XmlElement(ElementName = "selectSurround")]
        public SelectSurround SelectSurround { get; set; }
        [XmlElement(ElementName = "VolumeDisplay")]
        public VolumeDisplay VolumeDisplay { get; set; }
        [XmlElement(ElementName = "MasterVolume")]
        public MasterVolume MasterVolume { get; set; }
        [XmlElement(ElementName = "Mute")]
        public Mute Mute { get; set; }
        [XmlElement(ElementName = "RemoteMaintenance")]
        public RemoteMaintenance RemoteMaintenance { get; set; }
        [XmlElement(ElementName = "SubwooferDisplay")]
        public SubwooferDisplay SubwooferDisplay { get; set; }
        [XmlElement(ElementName = "Zone2VolDisp")]
        public Zone2VolDisp Zone2VolDisp { get; set; }
        [XmlElement(ElementName = "SleepOff")]
        public SleepOff SleepOff { get; set; }
    }
}