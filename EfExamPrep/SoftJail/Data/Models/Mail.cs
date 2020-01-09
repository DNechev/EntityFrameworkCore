using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Mail
    {
        //In case of a failed test check foreign keys and check if collections are required

        [Key]
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Sender { get; set; }


        [RegularExpression(@"^[A-Za-z0-9\s]*str.$")]
        [Required]
        public string Address { get; set; }

        [ForeignKey(nameof(Prisoner))]
        public int PrisonerId { get; set; }

        [Required]
        public Prisoner Prisoner { get; set; }
        //PrisonerId - integer, foreign key
        //Prisoner – the mail's Prisoner (required)
    }
}
