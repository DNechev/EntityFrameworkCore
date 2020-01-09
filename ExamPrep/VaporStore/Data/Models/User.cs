using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace VaporStore.Data.Models
{
    public class User
    {
        public User()
        {
            this.Cards = new HashSet<Card>();
        }

        [Key]
        public int Id { get; set; }
        // Id – integer, Primary Key

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Username { get; set; }
        // Username – text with length[3, 20] (required)


        [Required]
        [RegularExpression(@"^[A-Z]([a-z]+)\s([A-Z])([a-z]+)$")]
        public string FullName { get; set; }
        // FullName – text, which has two words, consisting of Latin letters.Both start with an upper letter and are separated by a single space(ex. "John Smith") (required)

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        // Email – text(required)

        [Required]
        [Range(typeof(int), "3", "103")]
        public int Age { get; set; }
        // Age – integer in the range[3, 103] (required)

        public ICollection<Card> Cards { get; set; }
        // Cards – collection of type Card

    }
}
