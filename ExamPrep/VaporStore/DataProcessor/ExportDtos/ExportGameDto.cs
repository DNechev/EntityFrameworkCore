using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("Game")]
    public class ExportGameDto
    {
        [XmlAttribute("title")]
        public string Game { get; set; }

        [XmlElement]
        public string Genre { get; set; }

        [XmlElement]
        public decimal Price { get; set; }
    }
}
