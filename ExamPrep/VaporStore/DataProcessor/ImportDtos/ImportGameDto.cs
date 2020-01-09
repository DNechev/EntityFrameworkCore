using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VaporStore.Data.Models;

namespace VaporStore.DataProcessor.ImportDtos
{
    public class ImportGameDto
    {
        [Required]
        public string Name { get; set; }
        //     "Name": "Dota 2",

        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        [Required]
        public decimal Price { get; set; }
        //"Price": 0,

        [Required]
        public string ReleaseDate { get; set; }
        //"ReleaseDate": "2013-07-09",

        [Required]
        public string Developer { get; set; }
        //"Developer": "Valve",

        [Required]
        public string Genre { get; set; }
        //"Genre": "Action",

        public List<string> Tags { get; set; }
        //"Tags": [
        //  "Multi-player",
        //  "Co-op",
        //  "Steam Trading Cards",
        //  "Steam Workshop",
        //  "SteamVR Collectibles",
        //  "In-App Purchases",
        //  "Valve Anti-Cheat enabled"


    }
}
