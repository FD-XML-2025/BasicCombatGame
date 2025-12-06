using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameDataTypes;

[Serializable]
[XmlRoot("settings")]
public class SettingsData
{
    [XmlElement("volume")]
    public VolumeSettings Volume;

    [XmlElement("windowMode")]
    public WindowMode WindowMode;

    [XmlElement("screenResolution")]
    public string ScreenResolution;

    [XmlElement("fps")]
    public int Fps;

    [XmlElement("quality")]
    public Quality Quality;
}

public enum WindowMode
{
    [XmlEnum("Windowed")]
    Windowed,

    [XmlEnum("Full Screen")]
    Fullscreen,

    [XmlEnum("Windowed Borderless")]
    WindowedBorderless
}

public enum Quality
{
    [XmlEnum("Low")]
    Low,
    [XmlEnum("Medium")]
    Medium,
    [XmlEnum("High")]
    High
}

public class VolumeSettings
{
    [XmlElement("general")]
    public float General;

    [XmlElement("music")]
    public float Music;

    [XmlElement("sfx")]
    public float Sfx;

    [XmlElement("ui")]
    public float Ui;
}