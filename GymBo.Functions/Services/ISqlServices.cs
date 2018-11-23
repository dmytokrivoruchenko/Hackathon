using System;
using System.Collections.Generic;
using System.Text;
using GymBo.Functions.Models;
using Microsoft.Azure.WebJobs.Host;

namespace GymBo.Functions.Services
{
    public interface ISqlServices
    {
        List<T> GetSources<T>(string cs, string table, TraceWriter log);

        Simulator GetSimulator(string cs, int id, TraceWriter log, string table = "Simulator");

        //List<SimulatorExercises> GetSimulatorExercises(string cs, int simulatorId, TraceWriter log, string table = "SimulatorExercises");
    }
}
