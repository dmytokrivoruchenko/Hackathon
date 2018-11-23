namespace GymBo.Bot.Models.Sql
{
    public class SimulatorExercise
    {
        public SimulatorExercise(int simulatorExerciseId, int simulatorId, int exerciseId)
        {
            SimulatorExerciseId = simulatorExerciseId;
            SimulatorId = simulatorId;
            ExerciseId = exerciseId;
        }

        public int SimulatorExerciseId { get; set; }

        public int SimulatorId { get; set; }

        public int ExerciseId { get; set; }
    }
}
