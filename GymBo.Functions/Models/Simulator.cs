using System;
using System.Collections.Generic;
using System.Text;

namespace GymBo.Functions.Models
{
    public class Simulator
    {
        public Simulator(int simulatorId, string name, string description)
        {
            SimulatorId = simulatorId;
            Name = name;
            Description = description;
        }

        public int SimulatorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
