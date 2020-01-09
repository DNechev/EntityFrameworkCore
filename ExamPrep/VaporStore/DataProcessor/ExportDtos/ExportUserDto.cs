using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.ExportDtos
{
    [XmlType("User")]
    public class ExportUserDto
    {
        [XmlAttribute("username")]
        public string User { get; set; }

        [XmlArray]
        public ExportPurchaseDto[] Purchases { get; set; }

        [XmlElement]
        public decimal TotalSpent { get; set; }
    }
}
