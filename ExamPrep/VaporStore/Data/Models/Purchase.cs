using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Enums;

namespace VaporStore.Data.Models
{
    public class Purchase
    {

        [Key]
        public int Id { get; set; }
        //Id – integer, Primary Key

        [Required]
        public PurchaseType Type { get; set; }
        //Type – enumeration of type PurchaseType, with possible values(“Retail”, “Digital”) (required) 

        [Required]
        [RegularExpression(@"^([A-Z0-9]{4}-){2}([A-Z0-9]{4})$")]
        public string ProductKey { get; set; }
        //ProductKey – text, which consists of 3 pairs of 4 uppercase Latin letters and digits, separated by dashes(ex. “ABCD-EFGH-1J3L”) (required)

        [Required]
        public DateTime Date { get; set; }
        //Date – Date(required)

        [Required]
        public int CardId { get; set; }
        //CardId – integer, foreign key(required)

        [Required]
        public Card Card { get; set; }
        //Card – the purchase’s card(required)

        [Required]
        public int GameId { get; set; }
        //GameId – integer, foreign key(required)

        [Required]
        public Game Game { get; set; }
        //Game – the purchase’s game(required)
    }
}
