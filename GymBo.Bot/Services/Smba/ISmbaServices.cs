using System;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace GymBo.Bot.Services.Smba
{
    public interface ISmbaServices
    {
        Task<Uri> GetAttachmentAsync(Attachment attachment);
    }
}
