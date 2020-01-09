namespace Cinema.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Cinema.Data.Models;
    using Cinema.Data.Models.Enums;
    using Cinema.DataProcessor.ImportDto;
    using Data;
    using Newtonsoft.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        public static string ImportMovies(CinemaContext context, string jsonString)
        {
            var movieDtos = JsonConvert.DeserializeObject<ImportMovieDto[]>(jsonString);
            StringBuilder result = new StringBuilder();

            foreach (var movieDto in movieDtos)
            {
                if (!isValid(movieDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                var currentMovie = context.Movies.FirstOrDefault(m => m.Title == movieDto.Title);

                if (currentMovie == null)
                {
                    var movie = new Movie
                    {
                        Title = movieDto.Title,
                        Genre = (Genre)Enum.Parse(typeof(Genre), movieDto.Genre),
                        Duration = TimeSpan.Parse(movieDto.Duration),
                        Rating = movieDto.Rating,
                        Director = movieDto.Director
                    };
                    context.Add(movie);
                    result.AppendLine($"Successfully imported {movie.Title} with genre {movie.Genre} and rating {movie.Rating:f2}!");
                }
                else
                {
                    result.AppendLine(ErrorMessage);
                }
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        public static string ImportHallSeats(CinemaContext context, string jsonString)
        {
            ImportHallSeatsDto[] hallSeatsDtos = JsonConvert.DeserializeObject<ImportHallSeatsDto[]>(jsonString);
            StringBuilder result = new StringBuilder();

            foreach (var hallSeatDto in hallSeatsDtos)
            {
                if (!isValid(hallSeatDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                var currentHallSeat = context.Halls.FirstOrDefault(x => x.Name == hallSeatDto.Name);

                if (currentHallSeat == null)
                {
                    var hall = new Hall
                    {
                        Name = hallSeatDto.Name,
                        Is4Dx = hallSeatDto.Is4Dx,
                        Is3D = hallSeatDto.Is3D,
                        Seats = new List<Seat>()
                    };

                    for (int i = 0; i < hallSeatDto.Seats; i++)
                    {
                        var seat = new Seat
                        {
                            Hall = hall,
                            HallId = hall.Id
                        };
                        context.Add(seat);
                    }
                    context.Halls.Add(hall);

                    string projectionType = GetProjectionType(hall.Is3D, hall.Is4Dx);

                    result.AppendLine($"Successfully imported {hall.Name}({projectionType}) with {hall.Seats.Count} seats!");
                }
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        private static string GetProjectionType(bool is3D, bool is4Dx)
        {
            if (!is3D && !is4Dx)
            {
                return "Normal";
            }
            else if (is3D && !is4Dx)
            {
                return "3D";
            }
            else if (!is3D && is4Dx)
            {
                return "4Dx";
            }
            else
            {
                return "4Dx/3D";
            }
        }

        public static string ImportProjections(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProjectionDto[]), new XmlRootAttribute("Projections"));

            var projectionsDtos = (ImportProjectionDto[])serializer.Deserialize(new StringReader(xmlString));

            var result = new StringBuilder();

            foreach (var projectionDto in projectionsDtos)
            {
                if (!projectionIsValid(projectionDto, context))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                var currentProjection = context.Projections
                    .FirstOrDefault(x => x.HallId == projectionDto.HallId && x.MovieId == projectionDto.MovieId);

                if (currentProjection == null)
                {
                    var dateInput = DateTime.ParseExact(projectionDto.DateTime, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);

                    string dateOutput = dateInput.ToString("MM/dd/yyyy");

                    var projection = new Projection
                    {
                        MovieId = projectionDto.MovieId,
                        Movie = context.Movies.FirstOrDefault(x => x.Id == projectionDto.MovieId),
                        HallId = projectionDto.HallId,
                        Hall = context.Halls.FirstOrDefault(x => x.Id == projectionDto.HallId),
                        DateTime = DateTime.ParseExact(dateOutput, "MM/dd/yyyy", CultureInfo.InvariantCulture)
                    };

                    context.Add(projection);
                    result.AppendLine($"Successfully imported projection {projection.Movie.Title} on {dateOutput}!");
                }

            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        private static bool projectionIsValid(ImportProjectionDto projectionDto, CinemaContext context)
        {
            bool valid = true;

            if (!context.Movies.Any(x => x.Id == projectionDto.MovieId))
            {
                valid = false;
            }

            if (!context.Halls.Any(x => x.Id == projectionDto.HallId))
            {
                valid = false;
            }

            return valid;
        }

        public static string ImportCustomerTickets(CinemaContext context, string xmlString)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerDto[]), new XmlRootAttribute("Customers"));

            var customersDtos = (CustomerDto[])serializer.Deserialize(new StringReader(xmlString));

            var result = new StringBuilder();

            foreach (var customerDto in customersDtos)
            {
                if (!isValid(customerDto))
                {
                    result.AppendLine(ErrorMessage);
                    continue;
                }

                var currentCustomer = context.Customers
                    .FirstOrDefault(x => x.FirstName == customerDto.FirstName && x.LastName == customerDto.LastName);

                if (currentCustomer == null)
                {
                    var customer = new Customer
                    {
                        FirstName = customerDto.FirstName,
                        LastName = customerDto.LastName,
                        Age = customerDto.Age,
                        Balance = customerDto.Balance,
                        Tickets = new List<Ticket>()
                    };

                    foreach (var ticketDto in customerDto.Tickets)
                    {
                        var ticket = new Ticket
                        {
                            Price = ticketDto.Price,
                            CustomerId = customer.Id,
                            Customer = customer,
                            ProjectionId = ticketDto.ProjectionId,
                            Projection = context.Projections.FirstOrDefault(p => p.Id == ticketDto.ProjectionId)
                        };
                        customer.Tickets.Add(ticket);
                    }

                    context.Customers.Add(customer);
                    result.AppendLine($"Successfully imported customer {customer.FirstName} {customer.LastName} with bought tickets: {customer.Tickets.Count}!");
                }
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        private static bool isValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}