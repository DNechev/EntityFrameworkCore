namespace VaporStore.DataProcessor
{
	using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using VaporStore.Data.Enums;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.ExportDtos;

    public static class Serializer
	{
		public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
		{
            var export = context.Genres
                .Where(g => genreNames.Contains(g.Name))
                .Select(x => new ExportGamesByGenreDto
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games.Where(g => g.Purchases.Any()).Select(g => new ExportGamesDto
                    {
                        Id = g.Id,
                        Title = g.Name,
                        Developer = g.Developer.Name,
                        Tags = string.Join(", ", g.GameTags.Select(t => t.Tag.Name)),
                        Players = g.Purchases.Count
                    })
                    .OrderByDescending(p => p.Players)
                    .ThenBy(i => i.Id)
                    .ToArray(),
                    TotalPlayers = x.Games.Sum(p => p.Purchases.Count)
                })
                .OrderByDescending(tp => tp.TotalPlayers)
                .ThenBy(i => i.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(export, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

		public static string ExportUserPurchasesByType(VaporStoreDbContext context, string storeType)
		{
            var type = Enum.Parse<PurchaseType>(storeType);

            var users = context.Users.Select(u => new ExportUserDto
            {
                User = u.Username,
                Purchases = u.Cards
                .SelectMany(p => p.Purchases)
                .Where(t => t.Type == type)
                .Select(p => new ExportPurchaseDto
                {
                    Card = p.Card.Number,
                    Cvc = p.Card.Cvc,
                    Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                    Game = new ExportGameDto
                    {
                        Game = p.Game.Name,
                        Genre = p.Game.Genre.Name,
                        Price = p.Game.Price
                    }
                })
                .OrderBy(d => d.Date)
                .ToArray(),
                TotalSpent = u.Cards
                .SelectMany(c => c.Purchases)
                .Where(p => p.Type == type)
                .Sum(g => g.Game.Price)
            })
                .Where(p => p.Purchases.Any())
                .OrderByDescending(ts => ts.TotalSpent)
                .ThenBy(u => u.User)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportUserDto[]), new XmlRootAttribute("Users"));

            var sb = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(sb), users, namespaces);

            var result = sb.ToString();
            return result;
        }
	}
}