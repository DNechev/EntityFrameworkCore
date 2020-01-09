using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cinema.Data.Models
{
    public class Projection
    {
        public Projection()
        {
            this.Tickets = new List<Ticket>();
        }

        [Key]
        public int Id { get; set; }
        //Id – integer, Primary Key

        [ForeignKey(nameof(Movie))]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        //MovieId – integer, foreign key(required)
        //Movie – the projection’s movie

        [ForeignKey(nameof(Hall))]
        public int HallId { get; set; }
        public Hall Hall { get; set; }
        //HallId – integer, foreign key(required)
        //Hall – the projection’s hall 

        [Required]
        public DateTime DateTime { get; set; }
        //DateTime - DateTime(required)

        public ICollection<Ticket> Tickets { get; set; }
        //Tickets - collection of type Ticket
    }
}
