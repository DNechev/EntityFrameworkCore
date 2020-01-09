using System;
using System.Collections.Generic;
using System.Text;

namespace VaporStore.DataProcessor.ExportDtos
{
    public class ExportGamesDto
    {
        public int Id { get; set; }
        //"Id": 49,
        public string Title { get; set; }
        //"Title": "Warframe",
        public string Developer { get; set; }
        //"Developer": "Digital Extremes",
        public string Tags { get; set; }
        //"Tags": "Single-player, In-App Purchases, Steam Trading Cards, Co-op, Multi-player, Partial Controller Support",
        public int Players { get; set; }
        //"Players": 6
    }
}
