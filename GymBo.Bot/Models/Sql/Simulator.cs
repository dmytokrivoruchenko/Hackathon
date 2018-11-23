namespace GymBo.Bot.Models.Sql
{
    public class Simulator
    {
        public Simulator(int simulatorId, string simulatorName, string description)
        {
            SimulatorId = simulatorId;
            SimulatorName = simulatorName;
            Description = description;
        }

        public int SimulatorId { get; set; }

        public string SimulatorName { get; set; }

        public string Description { get; set; }
    }
}
