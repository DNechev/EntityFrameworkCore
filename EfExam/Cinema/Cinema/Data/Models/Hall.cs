using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cinema.Data.Models
{
    public class Hall
    {
        public Hall()
        {
            this.Projections = new List<Projection>();
            this.Seats = new List<Seat>();
        }

        [Key]
        public int Id { get; set; }
        // Id – integer, Primary Key

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Name { get; set; }
        // Name – text with length[3, 20] (required)

        public bool Is4Dx { get; set; }
        // Is4Dx - bool

        public bool Is3D { get; set; }
        // Is3D - bool

        public ICollection<Projection> Projections { get; set; }
        // Projections - collection of type Projection

        public ICollection<Seat> Seats { get; set; }
        // Seats - collection of type Seat
    }
}
