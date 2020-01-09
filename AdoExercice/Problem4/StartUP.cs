using adoExercice;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace Problem4
{
    class StartUP
    {
        static void Main(string[] args)
        {
            using (SqlConnection connection = new SqlConnection(Config.connectionString))
            {
                connection.Open();

                string[] minionInfo = Console.ReadLine().Split(" ").ToArray();
                string minionName = minionInfo[1];
                int minionAge = int.Parse(minionInfo[2]);
                string minionTown = minionInfo[3];

                string[] villianInfo = Console.ReadLine().Split(" ").ToArray();
                string villianName = villianInfo[1];

                string addTown = $"INSERT INTO Towns([Name]) VALUES('{minionTown}')";
                string addVillain = $"INSERT INTO Villains (Name, EvilnessFactorId)  VALUES ('{villianName}', 4)";
                string checkTown = $"SELECT Id FROM Towns WHERE [Name] = '{minionTown}'";
                string checkVillian = $"SELECT Id FROM Villains WHERE Name = '{villianName}'";
                string getMinionId = $"SELECT Id FROM Minions WHERE Name = '{minionName}'";
                int townId = 0;
                int villianId = 0;
                int minionId = 0;

                using (SqlCommand checkTownCommand = new SqlCommand(checkTown, connection))
                {
                    if (checkTownCommand.ExecuteScalar() == null)
                    {
                        using (SqlCommand addTownCommand = new SqlCommand(addTown, connection))
                        {
                            addTownCommand.ExecuteNonQuery();
                            Console.WriteLine($"Town {minionTown} was added to the database.");
                        }
                    }
                    townId = (int)checkTownCommand.ExecuteScalar();
                }

                using (SqlCommand checkVillianCommand = new SqlCommand(checkVillian, connection))
                {
                    if (checkVillianCommand.ExecuteScalar() == null)
                    {
                        using (SqlCommand addVillianCommand = new SqlCommand(addVillain, connection))
                        {
                            addVillianCommand.ExecuteNonQuery();
                            Console.WriteLine($"Villain {villianName} was added to the database.");
                        }
                    }
                    villianId = (int)checkVillianCommand.ExecuteScalar();
                }

                string insertMinion = $"INSERT INTO Minions (Name, Age, TownId) VALUES ('{minionName}', {minionAge}, {townId})";
                string insertServant = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES (@minionId, @villainId)";

                using (SqlCommand insertMinionCommand = new SqlCommand(insertMinion, connection))
                {
                    insertMinionCommand.ExecuteNonQuery();
                }

                using (SqlCommand getMinionIdCommand = new SqlCommand(getMinionId, connection))
                {
                    minionId = (int)getMinionIdCommand.ExecuteScalar();
                }


                try
                {
                    using (SqlCommand insertServantCommand = new SqlCommand(insertServant, connection))
                    {
                        insertServantCommand.Parameters.AddWithValue("@minionId", minionId);
                        insertServantCommand.Parameters.AddWithValue("@villainId", villianId);
                        insertServantCommand.ExecuteNonQuery();
                        Console.WriteLine($"Successfully added {minionName} to be minion of {villianName}.");
                    }
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
