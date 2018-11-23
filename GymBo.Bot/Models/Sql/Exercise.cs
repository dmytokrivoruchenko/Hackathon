using System;
using System.Collections.Generic;
using System.Text;

namespace GymBo.Bot.Models.Sql
{
    public class Exercise
    {
        public Exercise(int exerciseId, string name, string description, string minLevelComplexity)
        {
            ExerciseId = exerciseId;
            Name = name;
            Description = description;
            MinLevelComplexity = minLevelComplexity;
        }

        public int ExerciseId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string MinLevelComplexity { get; set; }
    }
}
