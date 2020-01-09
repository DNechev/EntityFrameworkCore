using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using adoExercice;

namespace Problem8
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                int[] minions = Console.ReadLine().Split(" ").Select(int.Parse).ToArray();

                for (int i = 0; i < minions.Length; i++)
                {
                    UpdateMinions(connection, minions[i]);
                }

                PrintMinions(connection);
            }
        }

        private static void PrintMinions(SqlConnection connection)
        {
            string querry = "SELECT Name, Age FROM Minions";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine(reader[0] + " " + reader[1]);
                }
            }
        }

        private static void UpdateMinions(SqlConnection connection, int id)
        {
            string querry = "UPDATE Minions SET Name = UPPER(LEFT(Name, 1)) + SUBSTRING(Name, 2, LEN(Name)), Age += 1 WHERE Id = @Id";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }
    }
}
