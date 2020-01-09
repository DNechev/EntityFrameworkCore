using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Enums;

namespace VaporStore.DataProcessor.ImportDtos
{
    [XmlType("Purchase")]
    public class PurchaseDto
    {
        [XmlAttribute("title")]
        public string Title { get; set; }
        //       <Purchase title = "Dungeon Warfare 2" >

        [XmlElement("Type")]
        public PurchaseType Type { get; set; }
        //  < Type > Digital </ Type >

        [XmlElement("Key")]
        [RegularExpression(@"^([A-Z0-9]{4}-){2}([A-Z0-9]{4})$")]
        public string Key { get; set; }
        //  < Key > ZTZ3 - 0D2S-G4TJ</Key>

        [XmlElement("Card")]
        public string Card { get; set; }
        //  <Card>1833 5024 0553 6211</Card>

        [XmlElement("Date")]
        public string Date { get; set; }
        //  <Date>07/12/2016 05:49</Date>
        //</Purchase>
    }
}
