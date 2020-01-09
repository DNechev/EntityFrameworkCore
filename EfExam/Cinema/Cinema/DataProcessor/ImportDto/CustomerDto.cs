using Cinema.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;

namespace Cinema.DataProcessor.ImportDto
{
    [XmlType("Customer")]
    public class CustomerDto
    {
        [XmlElement("FirstName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }
        // <FirstName>Randi</FirstName>

        [XmlElement("LastName")]
        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }
        // <LastName>Ferraraccio</LastName>

        [XmlElement("Age")]
        [Required]
        [Range(12, 110)]
        public int Age { get; set; }
        // <Age>20</Age>

        [XmlElement("Balance")]
        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }
        // <Balance>59.44</Balance>

        [XmlArray("Tickets")]
        public TciketDto[] Tickets { get; set; }
        // <Tickets>
    }
}
