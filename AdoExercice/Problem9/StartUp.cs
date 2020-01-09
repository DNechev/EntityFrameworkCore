using System;
using System.Data.SqlClient;
using adoExercice;

namespace Problem9
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                int id = int.Parse(Console.ReadLine());

                string querry = "SELECT Name, Age FROM Minions WHERE Id = @Id";
                string getOlderQuerry = "EXEC usp_GetOlder @id = @minionID";

                using (SqlCommand getOlder = new SqlCommand(getOlderQuerry, connection))
                {
                    getOlder.Parameters.AddWithValue("@minionID", id);
                    getOlder.ExecuteNonQuery();
                }

                using (SqlCommand command = new SqlCommand(querry, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    SqlDataReader reader = command.ExecuteReader();

                    while(reader.Read())
                    {
                        Console.WriteLine($"{reader[0]} – {reader[1]} years old");
                    }
                }
            }
        }
    }
}
