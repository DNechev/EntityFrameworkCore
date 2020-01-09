using adoExercice;
using System;
using System.Data.SqlClient;

namespace Problem3
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                int villainId = int.Parse(Console.ReadLine());
                string villain = " ";

                string findVillain = $"SELECT Name FROM Villains WHERE Id = {villainId}";
                string findMinions = $"SELECT ROW_NUMBER() OVER (ORDER BY m.Name) as RowNum, m.Name, m.Age FROM MinionsVillains AS mv JOIN Minions As m ON mv.MinionId = m.Id WHERE mv.VillainId = {villainId} ORDER BY m.Name";

                SqlCommand commandFindVillain = new SqlCommand(findVillain, connection);
                SqlCommand commandFindMinions = new SqlCommand(findMinions, connection);

                if (commandFindVillain.ExecuteScalar() == null)
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    connection.Close();
                }
                else if (commandFindMinions.ExecuteScalar() == null)
                {
                    villain = (string)commandFindVillain.ExecuteScalar();
                    Console.WriteLine($"Villain: {villain}");
                    Console.WriteLine("(no minions)");
                    connection.Close();
                }
                else
                {
                    villain = (string)commandFindVillain.ExecuteScalar();
                    Console.WriteLine($"Villain: {villain}");
                    SqlDataReader reader = commandFindMinions.ExecuteReader();
                    while(reader.Read())
                    {
                        Console.WriteLine($"{reader[0]}. {reader[1]} {reader[2]}");
                    }
                    connection.Close();
                }
            }
        }
    }
}
