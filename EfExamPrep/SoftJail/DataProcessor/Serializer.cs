namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var prisoners = context.Prisoners
                .Where(x => ids.Contains(x.Id))
                .Select(x => new ExportPrisonerDto
                {
                    Id = x.Id,
                    Name = x.FullName,
                    CellNumber = x.Cell.CellNumber,
                    Officers = x.PrisonerOfficers.Where(p => p.PrisonerId == x.Id)
                    .Select(o => new ExportOfficerDto
                    {
                        OfficerName = o.Officer.FullName,
                        Department = o.Officer.Department.Name
                    })
                    .OrderBy(o => o.OfficerName)
                    .ToArray(),
                    TotalOfficerSalary = x.PrisonerOfficers.Sum(os => os.Officer.Salary)
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToArray();

            var json = JsonConvert.SerializeObject(prisoners, Newtonsoft.Json.Formatting.Indented);
            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonersArray = prisonersNames.Split(",").ToArray();

            var prisoners = context.Prisoners
                .Where(p => prisonersArray.Contains(p.FullName))
                .Select(x => new ExportPrisonersMsg
                {
                    Id = x.Id,
                    Name = x.FullName,
                    IncarcerationDate = x.IncarcerationDate.ToString("yyyy-MM-dd"),
                    EncryptedMessages = x.Mails.Where(p => p.PrisonerId == x.Id).Select(m => new ExportEncryptedMessages
                    {
                        Description = Reverse(m.Description)
                    })
                    .ToArray()
                })
                .OrderBy(n => n.Name)
                .ThenBy(i => i.Id)
                .ToArray();

            var serializer = new XmlSerializer(typeof(ExportPrisonersMsg[]), new XmlRootAttribute("Prisoners"));

            var result = new StringBuilder();
            var namespaces = new XmlSerializerNamespaces(new[] { new XmlQualifiedName("", "") });
            serializer.Serialize(new StringWriter(result), prisoners, namespaces);

            return result.ToString().Trim();
        }

        public static string Reverse(string description)
        {
            char[] charArray = description.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}

