using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftJail.Data.Models
{
    public class Prisoner
    {
        //In case of a failed test check foreign keys and check if collections are required
        public Prisoner()
        {
            this.PrisonerOfficers = new HashSet<OfficerPrisoner>();
            this.Mails = new HashSet<Mail>();
        }

        [Key]
        public int Id { get; set; }

        [MinLength(3), MaxLength(20)]
        [Required]
        public string FullName { get; set; }

        [RegularExpression(@"^The [A-Z]{1}[a-z]*$")]
        [Required]
        public string Nickname { get; set; }

        [Range(18, 65)]
        [Required]
        public int Age { get; set; }

        [Required]
        public DateTime IncarcerationDate { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        [ForeignKey(nameof(Cell))]
        public int? CellId { get; set; }

        [Required]
        public Cell Cell { get; set; }

        [Required]
        public ICollection<Mail> Mails { get; set; }

        [Required]
        public ICollection<OfficerPrisoner> PrisonerOfficers { get; set; }
    }
}