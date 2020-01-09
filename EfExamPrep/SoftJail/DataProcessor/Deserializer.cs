namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            ImportDepartmentsCellsDto[] deserializedDepCells = JsonConvert
                .DeserializeObject<ImportDepartmentsCellsDto[]>(jsonString);

            StringBuilder result = new StringBuilder();

            foreach (var depCell in deserializedDepCells)
            {
                bool validCells = true;

                foreach (var cell in depCell.Cells)
                {
                    if (!isValid(cell))
                    {
                        validCells = false;
                        break;
                    }
                }

                if (!isValid(depCell) || !validCells)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                List<Department> departments = new List<Department>();
                List<Cell> cells = new List<Cell>();

                Department currentDep = context.Departments.FirstOrDefault(x => x.Name == depCell.Name);

                if (currentDep == null)
                {
                    Department departmentToAdd = new Department
                    {
                        Name = depCell.Name,
                        Cells = new List<Cell>()
                    };
                    foreach (var cell in depCell.Cells)
                    {
                        var currentCell = context.Cells.FirstOrDefault(c => c.CellNumber == cell.CellNumber);
                        if (currentCell == null)
                        {
                            var cellToAdd = new Cell
                            {
                                CellNumber = cell.CellNumber,
                                HasWindow = cell.HasWindow
                            };
                            context.Cells.Add(cellToAdd);
                            departmentToAdd.Cells.Add(cellToAdd);
                            continue;
                        }
                        departmentToAdd.Cells.Add(currentCell);
                    }

                    context.Departments.Add(departmentToAdd);
                    result.AppendLine($"Imported {departmentToAdd.Name} with {departmentToAdd.Cells.Count} cells");
                }
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            ImportPrisonersMailsDto[] deserializedPrisoners = JsonConvert
                .DeserializeObject<ImportPrisonersMailsDto[]>(jsonString);

            var result = new StringBuilder();

            foreach (var entity in deserializedPrisoners)
            {
                bool mailsAreValid = true;
                foreach (var mail in entity.Mails)
                {
                    if (!isValid(mail))
                    {
                        mailsAreValid = false;
                        break;
                    }
                }

                if (!isValid(entity) || !mailsAreValid)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                var currentPrisoner = context.Prisoners
                    .FirstOrDefault(p => p.FullName == entity.FullName && p.Nickname == entity.Nickname && p.Age == entity.Age);

                if (currentPrisoner == null)
                {
                    Prisoner prisoner = new Prisoner
                    {
                        FullName = entity.FullName,
                        Nickname = entity.Nickname,
                        Age = entity.Age,
                        IncarcerationDate = DateTime.ParseExact(entity.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture),
                        Bail = entity.Bail,
                        CellId = entity.CellId,
                        Mails = new List<Mail>()
                    };

                    bool releaseDateValidation = DateTime.TryParseExact(entity.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date);

                    if (!releaseDateValidation)
                    {
                        prisoner.ReleaseDate = null;
                    }
                    else
                    {
                        prisoner.ReleaseDate = DateTime.ParseExact(entity.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    }

                    foreach (var mail in entity.Mails)
                    {
                        var currentMail = context.Mails
                              .FirstOrDefault(m => m.Description == mail.Description && m.Sender == mail.Sender && m.Address == mail.Address);

                        if (currentMail == null)
                        {
                            var email = new Mail
                            {
                                Description = mail.Description,
                                Sender = mail.Sender,
                                Address = mail.Address
                            };
                            context.Mails.Add(email);
                            prisoner.Mails.Add(email);
                            continue;
                        }
                        prisoner.Mails.Add(currentMail);
                    }
                    context.Prisoners.Add(prisoner);
                    result.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
                }
            }
            context.SaveChanges();
            return result.ToString().Trim();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var serializer = new XmlSerializer(typeof(ImportOfficersPrisonersDto[]), new XmlRootAttribute("Officers"));

            var deserialized = (ImportOfficersPrisonersDto[])serializer.Deserialize(new StringReader(xmlString));

            StringBuilder result = new StringBuilder();

            foreach (var entity in deserialized)
            {
                bool isValidPosition = Enum.IsDefined(typeof(Position), entity.Position);
                bool isValidWeapon = Enum.IsDefined(typeof(Weapon), entity.Weapon);

                if (!isValid(entity) || !isValidPosition || !isValidWeapon)
                {
                    result.AppendLine("Invalid Data");
                    continue;
                }

                var currentOfficer = context.Officers.FirstOrDefault(o => o.FullName == entity.Name);

                if (currentOfficer == null)
                {
                    var officer = new Officer
                    {
                        FullName = entity.Name,
                        Salary = entity.Money,
                        Position = (Position)Enum.Parse(typeof(Position), entity.Position),
                        Weapon = (Weapon)Enum.Parse(typeof(Weapon), entity.Weapon),
                        DepartmentId = entity.DepartmentId,
                    };

                    foreach (var op in entity.Prisoners)
                    {
                        var ofccierPrisoner = new OfficerPrisoner
                        {
                            OfficerId = officer.Id,
                            Officer = officer,
                            PrisonerId = op.PrisonerId,
                            Prisoner = context.Prisoners.FirstOrDefault(p => p.Id == op.PrisonerId)
                        };
                        officer.OfficerPrisoners.Add(ofccierPrisoner);
                        context.OfficersPrisoners.Add(ofccierPrisoner);
                    }
                    context.Officers.Add(officer);
                    result.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
                }
            }
            context.SaveChanges();
            //result.AppendLine($"OFF -- {context.Officers.Count()} ---- OP {context.OfficersPrisoners.Count()}");
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