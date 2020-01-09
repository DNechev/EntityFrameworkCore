using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Enums;

namespace VaporStore.DataProcessor.ImportDtos
{
    public class CardDto
    {
        [Required]
        [RegularExpression(@"^(\d{4}\s){3}(\d{4})$")]
        public string Number { get; set; }
        //"Number": "1111 1111 1111 1111",

        [Required]
        [RegularExpression(@"^\d{3}$")]
        public string CVC { get; set; }
        //"CVC": "111",

        [Required]
        public CardType Type { get; set; }
        //"Type": "Debit"
    }
}
