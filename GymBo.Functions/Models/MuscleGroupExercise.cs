using System;
using System.Collections.Generic;
using System.Text;

namespace GymBo.Functions.Models
{
    public class MuscleGroupExercise
    {
        public MuscleGroupExercise(int muscleGroupExerciseId, int muscleGroupId, int exerciseId)
        {
            MuscleGroupExerciseId = muscleGroupExerciseId;
            MuscleGroupId = muscleGroupId;
            ExerciseId = exerciseId;
        }

        public int MuscleGroupExerciseId { get; set; }
        public int MuscleGroupId { get; set; }
        public int ExerciseId { get; set; }
    }
}
