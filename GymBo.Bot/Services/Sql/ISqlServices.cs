using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBo.Bot.Models;
using GymBo.Bot.Models.Sql;

namespace GymBo.Bot.Services.Sql
{
    public interface ISqlServices
    {
        List<SimulatorExercise> GetSimulatorExercises(int simulatorId, string table = "SimulatorExercises");

        Simulator GetSimulator(int id, string table = "Simulator");

        GymUser GetGymUser(string id, string table = "[dbo].[GymUsers]");

        Task CreateUserAsync(GymUser gymUser, string table = "[dbo].[GymUsers]");

        Task AddSimulatorsToUserAsync(string channelId, string[] simulatorsLabels, string table = "[dbo].[GymUserSimulators]");

        Task DeleteUser(string userChannelId);
    }
}
