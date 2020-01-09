using adoExercice;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Problem5
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                string country = Console.ReadLine();
                string output = string.Empty;

                UpdateTownNames(connection, country);
                string[] towns = GetTowns(connection, country);

                int townsAffected = towns.Length;

                if (townsAffected == 0)
                {
                    output = "No town names were affected.";
                }
                else
                {
                    output = $"{towns.Length} town names were affected." + Environment.NewLine + $"[{string.Join(", ", towns)}]";
                }

                Console.WriteLine(output);
            }
        }

        private static void UpdateTownNames(SqlConnection connection, string country)
        {
            string querry = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryCode = (SELECT c.Id FROM Countries AS c WHERE c.Name = @countryName)";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                command.Parameters.AddWithValue("@countryName", country);
                command.ExecuteNonQuery();
            }
        }

        private static string[] GetTowns(SqlConnection connection, string countryName)
        {
            string countryNameQuerry = "SELECT t.Name FROM Towns as t JOIN Countries AS c ON c.Id = t.CountryCode WHERE c.Name = @countryName";

            using (SqlCommand command = new SqlCommand(countryNameQuerry, connection))
            {
                List<string> towns = new List<string>();
                command.Parameters.AddWithValue("@countryName", countryName);
                SqlDataReader sqlDataReader = command.ExecuteReader();
                while (sqlDataReader.Read())
                {
                    towns.Add((string)sqlDataReader[0]);
                }

                return towns.ToArray();
            }
        }
    }
}
