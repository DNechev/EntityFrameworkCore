using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.DataProcessor.ExportDtos
{
    public class ExportGamesByGenreDto
    {
        public int Id { get; set; }
        //   "Id": 4,
        public string Genre { get; set; }
        //Genre": "Violent"                                        
        public ICollection<ExportGamesDto> Games { get; set; }
        //Games"

        public int TotalPlayers { get; set; }
    }
}
