using adoExercice;
using System;
using System.Data.SqlClient;

namespace Problem6
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();
                int villianIdToDelete = int.Parse(Console.ReadLine());
                string villianName = GetName(connection, villianIdToDelete);

                if (villianName == null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }

                int minionsReleased = GetMinionsCount(connection, villianIdToDelete);

                DeleteVillain(connection, villianIdToDelete);

                Console.WriteLine($"{villianName} was deleted.");
                Console.WriteLine($"{minionsReleased} minions were released.");
            }
        }

        private static void DeleteVillain(SqlConnection connection, int villianIdToDelete)
        {
            string querry = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            string querry2 = "DELETE FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                command.Parameters.AddWithValue("@villainId", villianIdToDelete);
                command.ExecuteNonQuery();
            }

            using (SqlCommand command = new SqlCommand(querry2, connection))
            {
                command.Parameters.AddWithValue("@villainId", villianIdToDelete);
                command.ExecuteNonQuery();
            }
        }

        private static int GetMinionsCount(SqlConnection connection, int villianIdToDelete)
        {
            string querry = "select COUNT(MinionId) from MinionsVillains where VillainId = @villainId";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                command.Parameters.AddWithValue("@villainId", villianIdToDelete);
                return (int)command.ExecuteScalar();
            }
        }

        private static string GetName(SqlConnection connection, int villianIdToDelete)
        {
            string querry = "SELECT Name FROM Villains WHERE Id = @villainId";

            using (SqlCommand command = new SqlCommand(querry, connection))
            {
                command.Parameters.AddWithValue("@villainId", villianIdToDelete);
                return (string)command.ExecuteScalar();
            }
        }
    }
}
