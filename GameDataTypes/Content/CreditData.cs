using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GameDataTypes;

/**
 * Credit class to retrieve data for XSLT sheet
 */
[Serializable]
[XmlRoot("credits", Namespace = "http://www.yakuzasrevenge.fr/credits")]
public class CreditData
{
    public String Name;
    public String Link;
}