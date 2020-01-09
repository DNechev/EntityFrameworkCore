namespace Cinema.DataProcessor
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Cinema.Data;
    using Cinema.DataProcessor.ExportDto;
    using Newtonsoft.Json;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportTopMovies(CinemaContext context, int rating)
        {
            MovieDto[] movies = context.Movies.Where(m => m.Rating >= rating && m.Projections.Any(t => t.Tickets.Count >= 1))
                .OrderByDescending(r => r.Rating)
                .ThenByDescending(x => x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)))
                .Select(x => new MovieDto
                {
                    MovieName = x.Title,
                    Rating = x.Rating.ToString("F2"),
                    TotalIncomes = x.Projections.Sum(p => p.Tickets.Sum(t => t.Price)).ToString("F2"),
                    Customers = x.Projections.SelectMany(p => p.Tickets).Select(c => new CustomerDto
                    {
                        FirstName = c.Customer.FirstName,
                        LastName = c.Customer.LastName,
                        Balance = c.Customer.Balance.ToString("F2")
                    })
                    .OrderByDescending(b => b.Balance)
                    .ThenBy(f => f.FirstName)
                    .ThenBy(l => l.LastName)
                    .ToList()
                })
                .Take(10)
                .ToArray();


            var json = JsonConvert.SerializeObject(movies, Formatting.Indented);

            return json;

        }

        public static string ExportTopCustomers(CinemaContext context, int age)
        {
            var customers = context.Customers
                .Where(c => c.Age >= age)
                .OrderByDescending(ms => ms.Tickets.Sum(t => t.Price))
                .Select(x => new ExportCustomerDto
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                SpentMoney = x.Tickets.Sum(t => t.Price).ToString("F2"),
                SpentTime = TimeSpan.FromSeconds(x.Tickets.Sum(t => t.Projection.Movie.Duration.TotalSeconds))
                .ToString(@"hh\:mm\:ss")
            })
                .Take(10)
                .ToArray();


            var xmlSerializer = new XmlSerializer(typeof(ExportCustomerDto[]), new XmlRootAttribute("Customers"));

            var result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });
            xmlSerializer.Serialize(new StringWriter(result), customers, namespaces);

            return result.ToString().TrimEnd();
        }
    }
}