using System;
using System.Collections.Generic;

namespace SoftUni.Models
{
    public  class Town
    {
        public Town()
        {
            this.Address = new HashSet<Address>();
        }

        public int TownId { get; set; }
        public string Name { get; set; }

        public ICollection<Address> Address { get; set; }
    }
}
