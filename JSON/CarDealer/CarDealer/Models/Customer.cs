using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CarDealer.Models
{
    public class Customer
    {
        [JsonIgnore]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate { get; set; }

        [Required]
        public bool IsYoungDriver { get; set; }

        public ICollection<Sale> Sales { get; set; }
    }
}