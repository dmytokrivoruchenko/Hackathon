using GymBo.Bot.Models.Sql;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GymBo.Bot.Models;

namespace GymBo.Bot.Services.Sql
{
    public class SqlServices : ISqlServices
    {
        public string SqlConnectionString { get; set; }

        public SqlServices(string sqlConnectionStringcs)
        {
            SqlConnectionString = sqlConnectionStringcs;
        }

        public GymUser GetGymUser(string channelId, string table = "[dbo].[GymUsers]")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SqlConnectionString) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(SqlConnectionString))
                    {
                        conn.Open();
                        return conn.QueryFirstOrDefault<GymUser>($"SELECT * FROM {table} WHERE ChannelId = '{channelId}'", null);
                    }
                }

                throw new ArgumentNullException(SqlConnectionString, table);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task CreateUserAsync(GymUser gymUser, string table = "[dbo].[GymUsers]")
        {
            return Task.Run(() =>
                {
                    try
                    {

                        using (var connection = new SqlConnection(SqlConnectionString))
                        {
                            connection.Open();
                            using (var command = connection.CreateCommand())
                            {
                                var sql = $"IF NOT EXISTS (SELECT * FROM {table} WHERE ChannelId='{gymUser.ChannelId}') INSERT INTO {table} (ChannelId, Name, Age, Sex, Height, Weight, DaysPerWeek, TrainingTime, Power) VALUES ('{gymUser.ChannelId}', '{gymUser.Name}', {gymUser.Age}, '{gymUser.Sex}', {gymUser.Height}, {gymUser.Weight}, {gymUser.DaysPerWeek}, {gymUser.TrainingTime}, {gymUser.Power})";
                                command.CommandText = sql;
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
            });
        }

        public Simulator GetSimulator(string label, string table = "Simulators")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SqlConnectionString) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(SqlConnectionString))
                    {
                        conn.Open();
                        return conn.QueryFirstOrDefault<Simulator>($"SELECT * FROM {table} WHERE Description = '{label}'", null);
                    }
                }

                throw new ArgumentNullException(SqlConnectionString, table);
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public Task AddSimulatorsToUserAsync(string channelId, string[] simulatorsLabels, string table = "[dbo].[GymUserSimulators]")
        {
            return Task.Run(() =>
                {
                    try
                    {

                        if (!string.IsNullOrWhiteSpace(channelId) && !string.IsNullOrWhiteSpace(SqlConnectionString))
                        {
                            using (var connection = new SqlConnection(SqlConnectionString))
                            {
                                connection.Open();

                                foreach (var label in simulatorsLabels)
                                {
                                    if (!string.IsNullOrWhiteSpace(label))
                                    {
                                        Simulator simulator = GetSimulator(label);
                                        GymUser gymUser = GetGymUser(channelId);
                                        if (simulator != null && gymUser != null)
                                        {
                                            using (var command = connection.CreateCommand())
                                            {
                                                var sql =
                                                    $"IF NOT EXISTS (SELECT * FROM {table} WHERE GymUserId = {gymUser.GymUserId} AND SimulatorId = {simulator.SimulatorId}) INSERT INTO {table} (GymUserId, SimulatorId) VALUES ({gymUser.GymUserId}, {simulator.SimulatorId})";
                                                command.CommandText = sql;
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                });
        }

        public Simulator GetSimulator(int id, string table = "Simulators")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SqlConnectionString) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(SqlConnectionString))
                    {
                        conn.Open();
                        return conn.QueryFirstOrDefault<Simulator>($"SELECT * FROM {table} WHERE SimulatorId = {id}", null);
                    }
                }

                throw new ArgumentNullException(SqlConnectionString, table);
            }
            catch (Exception e)
            {

                return null;
            }
        }

        public List<SimulatorExercise> GetSimulatorExercises(int simulatorId, string table = "SimulatorExercises")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(SqlConnectionString) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(SqlConnectionString))
                    {
                        conn.Open();
                        return conn.QueryAsync<SimulatorExercise>($"SELECT * FROM {table} WHERE SimulatorId = {simulatorId}", null).Result.ToList();
                    }
                }

                throw new ArgumentNullException(SqlConnectionString, table);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task DeleteUser(string userChannelId)
        {
            throw new NotImplementedException();
        }
    }
}
