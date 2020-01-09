namespace BookShop
{
    using Data;
    using Initializer;
    using System;
    using BookShop.Models.Enums;
    using System.Linq;
    using System.Text;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                //int lenghtCheck = int.Parse(Console.ReadLine());
                //string input = Console.ReadLine();
                //DbInitializer.ResetDatabase(db);
                int intResult = RemoveBooks(db);
                //string result = GetBooksByAuthor(db, input);
                //string result = GetMostRecentBooks(db);
                //IncreasePrices(db);
                db.SaveChanges();
                Console.WriteLine(intResult);
                //Console.WriteLine(result);
            }
        }

        //Problem 15
        public static int RemoveBooks(BookShopContext context)
        {
            var books = context.Books.Where(c => c.Copies < 4200).ToArray();
            int booksDeleted = 0;

            foreach (var book in books)
            {
                context.Remove(book);
                booksDeleted++;
            }

            return booksDeleted;
        }

        //Problem 14
        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(rd => rd.ReleaseDate.Value.Year < 2100)
                .ToArray();

            foreach (var book in books)
            {
                book.Price += 5;
            }
        }

        //Problem 13
        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();

            var categories = context.Categories
                .OrderBy(c => c.Name)
                .Select(x => new
                {
                    CategoryName = x.Name,
                    Books = x.CategoryBooks.Select(b => new { b.Book.Title, b.Book.ReleaseDate }).OrderByDescending(b => b.ReleaseDate).Take(3).ToArray()
                })
                .ToArray();

            foreach (var cat in categories)
            {
                sb.AppendLine($"--{cat.CategoryName}");

                foreach (var book in cat.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }
                      
            return sb.ToString().Trim();
        }

        //Problem 12
        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var categories = context.Categories
                .Select(x => new
                {
                    x.Name,
                    Profit = x.CategoryBooks.Sum(p => p.Book.Price * p.Book.Copies)
                })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x => x.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var cat in categories)
            {
                sb.AppendLine($"{cat.Name} ${cat.Profit:f2}");
            }

            return sb.ToString().Trim();
        }

        //Problem 11
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authors = context.Authors
                .Select(x => new
                {
                    x.FirstName,
                    x.LastName,
                    Copies = x.Books.Sum(c => c.Copies)
                })
                .OrderByDescending(c => c.Copies)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FirstName} {author.LastName} - {author.Copies}");
            }

            return sb.ToString().Trim();
        }

        //Problem 10
        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int books = context.Books
                .Where(t => t.Title.Length > lengthCheck)
                .ToArray().Count();

            return books;

        }

        //Problem 9
        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            input = input.ToLower();

            var books = context.Books
                .Where(b => EF.Functions.Like(b.Author.LastName.ToLower(), $"{input}%"))
                .OrderBy(b => b.BookId)
                .Select(x => new { x.Title, x.Author.FirstName, x.Author.LastName })
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FirstName} {book.LastName})");
            }

            return sb.ToString().Trim();
        }

        //Problem 8
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            input = input.ToLower();

            var books = context.Books
                .Where(t => EF.Functions.Like(t.Title.ToLower(), $"%{input}%"))
                .OrderBy(t => t.Title)
                .Select(x => x.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //Problem 7
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(f => EF.Functions.Like(f.FirstName, $"%{input}"))
                .OrderBy(a => a)
                .Select(x => new { FullName = x.FirstName + " " + x.LastName })
                .ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var author in authors)
            {
                result.AppendLine(author.FullName);
            }

            return result.ToString().Trim();
        }

        //Problem 6
        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            var dateTime = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < dateTime)
                .Select(x => new
                {
                    x.Title,
                    x.EditionType,
                    x.Price,
                    x.ReleaseDate
                })
                .OrderByDescending(x => x.ReleaseDate)
                .ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            return result.ToString().Trim();
        }

        //Problem 5
        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.ToLower())
                    .ToArray();

            var books = context.Books
                .Where(bc => bc.BookCategories.Any(x => input.Contains(x.Category.Name)))
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            StringBuilder sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine(book);
            }

            return sb.ToString().Trim();
        }

        //Problem 4
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            var books = context.Books
                .Where(r => r.ReleaseDate.Value.Year != year)
                .OrderBy(i => i.BookId)
                .Select(x => x.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);
        }

        //Problem 3
        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(p => p.Price > 40)
                .Select(x => new
                {
                    Title = x.Title,
                    Price = x.Price
                })
                .OrderByDescending(p => p.Price)
                .ToArray();

            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            return result.ToString().Trim();
        }

        //Problem 2
        public static string GetGoldenBooks(BookShopContext context)
        {
            var edition = Enum.Parse<EditionType>("Gold");

            var books = context.Books
                .Where(x => x.EditionType == edition)
                .Where(c => c.Copies < 5000)
                .OrderBy(i => i.BookId)
                .Select(t => t.Title)
                .ToArray();

            return string.Join(Environment.NewLine, books);

        }

        //Problem 1
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);

            var books = context.Books
                .Where(a => a.AgeRestriction == ageRestriction)
                .Select(t => t.Title)
                .OrderBy(t => t)
                .ToArray();


            StringBuilder result = new StringBuilder();

            foreach (var book in books)
            {
                result.AppendLine(book);
            }

            return result.ToString();
        }
    }
}
