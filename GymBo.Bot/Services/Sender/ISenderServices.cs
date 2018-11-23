using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Logging;

namespace GymBo.Bot.Services.Sender
{
    public interface ISenderServices
    {
        Task SendMessageAsync(string message, ITurnContext turnContext, CancellationToken cancellationToken, ILogger logger);
    }
}
