using adoExercice;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Problem7
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                string getMinionsQuerry = "SELECT Name FROM Minions";

                Queue<string> minions = new Queue<string>();

                using (SqlCommand command = new SqlCommand(getMinionsQuerry, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        var minionName = ((string)reader[0]).Trim();
                        if (!minions.Contains(minionName))
                        {
                            minions.Enqueue(minionName);
                        }
                    }
                }

                var naRoboPromenlivata = (int)Math.Ceiling(minions.Count / 2d);

                var firstList = minions.Take(naRoboPromenlivata);
                var secondList = minions.Skip(naRoboPromenlivata).Reverse().ToArray();

                //for (int i = 0; i < naRoboPromenlivata; i++)
                //{
                //    Console.WriteLine(firstList[i]);
                //    if (secondList.Length > i)
                //    {
                //        Console.WriteLine(secondList[i]);
                //    }
                //}

                firstList.ForEach((el, i) =>
                {
                    Console.WriteLine(el);
                    if (secondList.Length > i)
                    {
                        Console.WriteLine(secondList[i]);
                    }
                });
            }
        }
    }
}
