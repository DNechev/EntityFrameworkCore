using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Enums;

namespace VaporStore.Data.Models
{
    public class Card
    {
        public Card()
        {
            this.Purchases = new HashSet<Purchase>();
        }

        [Key]
        public int Id { get; set; }
        //Id – integer, Primary Key

        [Required]
        [RegularExpression(@"^(\d{4}\s){3}(\d{4})$")]
        public string Number { get; set; }
        //Number – text, which consists of 4 pairs of 4 digits, separated by spaces(ex. “1234 5678 9012 3456”) (required)

        [Required]
        [RegularExpression(@"^\d{3}$")]
        public string Cvc { get; set; }
        //Cvc – text, which consists of 3 digits(ex. “123”) (required)

        [Required]
        public CardType Type { get; set; }
        //Type – enumeration of type CardType, with possible values(“Debit”, “Credit”) (required)

        [Required]
        public int UserId { get; set; }
        //UserId – integer, foreign key(required)

        [Required]
        public User User { get; set; }
        //User – the card’s user(required)

        public ICollection<Purchase> Purchases { get; set; }
        //Purchases – collection of type Purchase

    }
}
