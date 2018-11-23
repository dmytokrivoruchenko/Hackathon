using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using GymBo.Functions.Models;
using Microsoft.Azure.WebJobs.Host;

namespace GymBo.Functions.Services
{
    public class SqlServices : ISqlServices
    {
        public List<T> GetSources<T>(string cs, string table, TraceWriter log)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cs) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        conn.Open();
                        return conn.QueryAsync<T>($"SELECT * FROM {table}", null).Result
                            .ToList<T>();
                    }
                }
                throw new ArgumentNullException(cs, table);
            }
            catch (Exception exception)
            {
                log.Error("SQLServices/GetSources error: ", exception);
                return null;
            }
        }

        public Simulator GetSimulator(string cs, int id, TraceWriter log, string table = "Simulators")
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(cs) && !string.IsNullOrWhiteSpace(table))
                {
                    using (SqlConnection conn = new SqlConnection(cs))
                    {
                        conn.Open();
                        return conn.QueryFirstOrDefault<Simulator>($"SELECT * FROM {table} WHERE SimulatorId = '{id}'",
                            null);
                    }
                }

                throw new ArgumentNullException(cs, table);
            }
            catch (Exception e)
            {
                log.Error("SqlServices/GetSimulator/Exception: ", e);
                return null;
            }
        }
    }
}
