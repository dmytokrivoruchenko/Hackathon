using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBo.Bot.Models
{
    public class GymUser
    {
        public GymUser()
        {
        }

        public GymUser(int gymUserId, string channelId, string name, double age, string sex, double height, double weight, double daysPerWeek, double trainingTime, double power)
        {
            GymUserId = gymUserId;
            ChannelId = channelId;
            Name = name;
            Age = age;
            Sex = sex;
            Height = height;
            Weight = weight;
            DaysPerWeek = daysPerWeek;
            TrainingTime = trainingTime;
            Power = power;
        }

        public int GymUserId { get; set; }

        public string ChannelId { get; set; }

        public string Name { get; set; }

        public double Age { get; set; }

        public string Sex { get; set; }

        public double Height { get; set; }

        public double Weight { get; set; }

        public double DaysPerWeek { get; set; }

        public double TrainingTime { get; set; }

        public double Power { get; set; }
    }
}
