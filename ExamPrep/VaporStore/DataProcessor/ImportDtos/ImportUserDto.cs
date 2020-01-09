using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.ImportDtos
{
    public class ImportUserDto
    {
        [Required]
        [RegularExpression(@"^[A-Z]([a-z]+)\s([A-Z])([a-z]+)$")]
        public string FullName { get; set; }
        //    "FullName": "Anita Ruthven",

        [Required]
        [MinLength(3), MaxLength(20)]
        public string Username { get; set; }
        //"Username": "aruthven",

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //"Email": "aruthven@gmail.com",

        [Required]
        [Range(typeof(int), "3", "103")]
        public int Age { get; set; }
        //"Age": 75,


        public List<CardDto> Cards { get; set; }
        //"Cards": [
        //  {
        //    "Number": "5208 8381 5687 8508",
        //    "CVC": "624",
        //    "Type": "Debit"
    }
}
