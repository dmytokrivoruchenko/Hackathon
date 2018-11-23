namespace GymBo.Bot.Models
{
    public class TrainingApparatus
    {
        public TrainingApparatus(string dataUrl)
        {
            DataUrl = dataUrl;
        }

        public string DataUrl { get; set; }
    }
}
