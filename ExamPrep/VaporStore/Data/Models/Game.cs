using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VaporStore.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Purchases = new HashSet<Purchase>();
            this.GameTags = new HashSet<GameTag>();
        }

        [Key]
        public int Id { get; set; }
        //Id – integer, Primary Key

        [Required]
        public string Name { get; set; }
        //Name – text(required)

        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        [Required]
        public decimal Price { get; set; }
        //Price – decimal (non-negative, minimum value: 0) (required)

        [Required]
        public DateTime ReleaseDate { get; set; }
        //ReleaseDate – Date(required)

        [Required]
        public int DeveloperId { get; set; }
        //DeveloperId – integer, foreign key(required)

        [Required]
        public Developer Developer { get; set; }
        //Developer – the game’s developer(required)

        [Required]
        public int GenreId { get; set; }
        //GenreId – integer, foreign key(required)

        [Required]
        public Genre Genre { get; set; }
        //Genre – the game’s genre(required)

        public ICollection<Purchase> Purchases { get; set; }
        //Purchases - collection of type Purchase

        public ICollection<GameTag> GameTags { get; set; }
        //GameTags - collection of type GameTag.Each game must have at least one tag.
    }
}
