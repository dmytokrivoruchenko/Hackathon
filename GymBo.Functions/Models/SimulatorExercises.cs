using System;
using System.Collections.Generic;
using System.Text;

namespace GymBo.Functions.Models
{
    public class SimulatorExercises
    {
        public SimulatorExercises(int simulatorExerciseId, int simulatorId, int exerciseId)
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
