namespace GymBo.Bot.Models.Sql
{
    public class SimulatorBase
    {
        public SimulatorBase(string simulatorName)
        {
            SimulatorName = simulatorName;
        }

        public string SimulatorName { get; set; }
    }
}
