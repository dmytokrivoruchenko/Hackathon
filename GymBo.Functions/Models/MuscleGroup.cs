using System;
using System.Collections.Generic;
using System.Text;

namespace GymBo.Functions.Models
{
    public class MuscleGroup
    {
        public MuscleGroup(int muscleGroupId, string name, string description)
        {
            MuscleGroupId = muscleGroupId;
            Name = name;
            Description = description;
        }

        public int MuscleGroupId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
