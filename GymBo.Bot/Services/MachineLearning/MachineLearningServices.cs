using System;
using System.Net;
using System.Threading.Tasks;
using GymBo.Bot.Models;
using GymBo.Bot.Models.Sql;
using Newtonsoft.Json;
using RestSharp;

namespace GymBo.Bot.Services.MachineLearning
{
    public class MachineLearningServices : IMachineLearningServices
    {
        public MachineLearningServices(string mlConnection)
        {
            MLUrl = mlConnection;
        }

        public string MLUrl { get; set; }

        public async Task<string> SendDataToMlAsync(TrainingApparatus trainingApparatus)
        {
            try
            {
                return await Task.Run(() =>
                {
                    var client = new RestClient(MLUrl);
                    var request = new RestRequest(Method.POST);
                    request.AddJsonBody(trainingApparatus);
                    var response = client.Execute(request);
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        SimulatorBase sb = JsonConvert.DeserializeObject<SimulatorBase>(response.Content);
                        return sb.SimulatorName;
                    }
                    return null;
                });
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}
