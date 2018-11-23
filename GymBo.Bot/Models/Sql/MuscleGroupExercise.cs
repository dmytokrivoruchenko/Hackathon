namespace GymBo.Bot.Models.Sql
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
