using System;
using System.Xml.Serialization;

namespace GameDataTypes;

/// <summary>
/// Game save data serializable class (global save)
/// </summary>
[Serializable]
[XmlRoot("save", Namespace = "http://www.yakuzasrevenge.fr/save")]
public class SaveData
{
    [XmlElement("kills")]
    public int Kills;
    
    [XmlElement("wins")]
    public int Wins;
    
    [XmlElement("loses")]
    public int Loses;
    
    [XmlElement("damageDealt")]
    public int DamageDealt;
    
    [XmlElement("damageTaken")]
    public int DamageTaken;
}