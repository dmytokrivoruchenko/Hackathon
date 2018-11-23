using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GymBo.Bot.Models;

namespace GymBo.Bot.Services.MachineLearning
{
    public interface IMachineLearningServices
    {
        Task<string> SendDataToMlAsync(TrainingApparatus trainingApparatus);
    }
}
