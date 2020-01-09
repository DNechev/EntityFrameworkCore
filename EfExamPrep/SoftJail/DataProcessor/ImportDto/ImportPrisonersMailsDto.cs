using SoftJail.Data.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonersMailsDto
    {
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
        public string IncarcerationDate { get; set; }

        public string ReleaseDate { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Bail { get; set; }

        public int CellId { get; set; }
        public List<ImportMailDto> Mails { get; set; }
    }
}
