using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBo.Bot.Models.Sql
{
    public class GymUserSimulator
    {
        public GymUserSimulator(int gymUserSimulatorId, int gymUserId, int simulatorId)
        {
            GymUserSimulatorId = gymUserSimulatorId;
            GymUserId = gymUserId;
            SimulatorId = simulatorId;
        }

        public int GymUserSimulatorId { get; set; }

        public int GymUserId { get; set; }

        public int SimulatorId { get; set; }
    }
}
