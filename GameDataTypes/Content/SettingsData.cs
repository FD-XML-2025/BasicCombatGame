using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameDataTypes;

/// <summary>
/// Settings serializable class for saving whole settings
/// </summary>
[Serializable]
[XmlRoot("settings", Namespace = "http://www.yakuzasrevenge.fr/settings")]
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

/// <summary>
/// Enum used for settings serialization, containing window desired mode
/// </summary>
public enum WindowMode
{
    [XmlEnum("Windowed")]
    Windowed,

    [XmlEnum("Full Screen")]
    Fullscreen,

    [XmlEnum("Windowed Borderless")]
    WindowedBorderless
}

/// <summary>
/// Enum only used for XML serialization
/// </summary>
public enum Quality
{
    [XmlEnum("Low")]
    Low,
    [XmlEnum("Medium")]
    Medium,
    [XmlEnum("High")]
    High
}

/// <summary>
/// Volume object for settings serialization, containing all volume settings
/// </summary>
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