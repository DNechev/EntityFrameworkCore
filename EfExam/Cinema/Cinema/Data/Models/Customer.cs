using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Customer
    {
        public Customer()
        {
            this.Tickets = new List<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        //Id – integer, Primary Key

        [Required]
        [MinLength(3), MaxLength(20)]
        public string FirstName { get; set; }
        //FirstName – text with length[3, 20] (required)

        [Required]
        [MinLength(3), MaxLength(20)]
        public string LastName { get; set; }
        //LastName – text with length[3, 20] (required)

        [Required]
        [Range(12, 110)]
        public int Age { get; set; }
        //Age – integer in the range[12, 110] (required)

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Balance { get; set; }
        //Balance - decimal (non-negative, minimum value: 0.01) (required)

        public ICollection<Ticket> Tickets { get; set; }
        //Tickets - collection of type Ticket
    }
}
