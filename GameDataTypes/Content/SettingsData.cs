using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameDataTypes;

public enum WindowMode
{
    
}

public enum Quality
{
    LOW,
    MEDIUM,
    HIGH
}

[Serializable]
[XmlRoot("settings", Namespace = "http://www.yakuzasrevenge.fr/settings")]
public class SettingsData
{
    public float MasterVolume;
    
    public float MusicVolume;

    public int FPS = -1;
    
    public Quality Quality = Quality.HIGH;
}
