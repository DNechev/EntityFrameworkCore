namespace VaporStore.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.ImportDtos;

    public static class Deserializer
    {
        public static string ImportGames(VaporStoreDbContext context, string jsonString)
        {
            var gamesJson = JsonConvert.DeserializeObject<ImportGameDto[]>(jsonString);
            StringBuilder result = new StringBuilder();

            foreach (var gameDto in gamesJson)
            {
                if (!IsValid(gameDto) || gameDto.Tags.Count == 0)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                Developer developer = GetDeveloper(context, gameDto);
                Genre genre = GetGenre(context, gameDto);
                Tag[] tags = GetTags(context, gameDto);

                var game = new Game
                {
                    Name = gameDto.Name,
                    Price = gameDto.Price,
                    ReleaseDate = DateTime.ParseExact(gameDto.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                };

                game.Developer = developer;
                game.Genre = genre;
                foreach (var tag in tags)
                {
                    game.GameTags.Add(new GameTag
                    {
                        Game = game,
                        Tag = tag
                    });
                }
                context.Games.Add(game);
                result.AppendLine($"Added {game.Name} ({game.Genre.Name}) with {game.GameTags.Count} tags");
            }

            context.SaveChanges();

            return result.ToString().Trim();
        }

        private static Tag[] GetTags(VaporStoreDbContext context, ImportGameDto game)
        {
            List<Tag> tags = new List<Tag>();

            foreach (var tagName in game.Tags)
            {
                var tag = context.Tags.FirstOrDefault(x => x.Name == tagName);

                if (tag == null)
                {
                    tag = new Tag
                    {
                        Name = tagName
                    };

                    context.Tags.Add(tag);
                    context.SaveChanges();
                }
                tags.Add(tag);
            }

            return tags.ToArray();
        }

        private static Genre GetGenre(VaporStoreDbContext context, ImportGameDto game)
        {
            Genre genre = context.Genres.FirstOrDefault(x => x.Name == game.Genre);

            if (genre == null)
            {
                genre = new Genre
                {
                    Name = game.Genre
                };

                context.Genres.Add(genre);
                context.SaveChanges();
            }
            return genre;
        }

        private static Developer GetDeveloper(VaporStoreDbContext context, ImportGameDto gameDto)
        {
            Developer dev = context.Developers.FirstOrDefault(x => x.Name == gameDto.Developer);

            if (dev == null)
            {
                dev = new Developer
                {
                    Name = gameDto.Developer
                };

                context.Developers.Add(dev);
                context.SaveChanges();
            }
            return dev;
        }

        public static string ImportUsers(VaporStoreDbContext context, string jsonString)
        {
            var usersJson = JsonConvert.DeserializeObject<ImportUserDto[]>(jsonString);
            StringBuilder result = new StringBuilder();

            foreach (var userDto in usersJson)
            {
                if (!IsValid(userDto) || userDto.Cards.Count == 0)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                var user = new User
                {
                    Username = userDto.Username,
                    FullName = userDto.FullName,
                    Email = userDto.Email,
                    Age = userDto.Age
                };

                List<Card> cards = new List<Card>();

                foreach (var cardDto in userDto.Cards)
                {
                    var card = context.Cards.FirstOrDefault(c => c.Number == cardDto.Number);

                    if (card == null)
                    {
                        card = new Card
                        {
                            Number = cardDto.Number,
                            Cvc = cardDto.CVC,
                            Type = cardDto.Type,
                            User = user
                        };
                    }
                    cards.Add(card);
                    context.Cards.Add(card);
                    //context.SaveChanges();
                }

                user.Cards = cards;
                context.Users.Add(user);
                char firstInitial = user.FullName[0];
                string[] names = user.FullName.Split(' ').ToArray();
                string lastName = names[1];
                string nameToPrint = firstInitial + lastName;

                result.AppendLine($"Imported {nameToPrint.ToLower()} with {user.Cards.Count} cards");
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        public static string ImportPurchases(VaporStoreDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(PurchaseDto[]), new XmlRootAttribute("Purchases"));

            var deserialized = (PurchaseDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder result = new StringBuilder();

            foreach (var purchaseDto in deserialized)
            {
                if (!IsValid(purchaseDto))
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                var purchase = new Purchase
                {
                    Type = purchaseDto.Type,
                    ProductKey = purchaseDto.Key,
                    Card = context.Cards.FirstOrDefault(c => c.Number == purchaseDto.Card),
                    Date = DateTime.ParseExact(purchaseDto.Date, "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                    Game = context.Games.FirstOrDefault(x => x.Name == purchaseDto.Title)
                };

                context.Purchases.Add(purchase);


                result.AppendLine($"Imported {purchaseDto.Title} for {purchase.Card.User.Username}");
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        private static bool IsValid(object entity)
        {
            var validationContext = new ValidationContext(entity);
            var validationResults = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(entity, validationContext, validationResults, true);

            return isValid;
        }
    }
}