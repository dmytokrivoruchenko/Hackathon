using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace GymBo.Bot.Services.Sender
{
    public class SenderServices : ISenderServices
    {
        public async Task SendMessageAsync(string message, ITurnContext turnContext, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                await turnContext.SendActivityAsync(message, cancellationToken: cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "SenderService/SendMessageAsync");
            }
        }

        public async Task SendHeroCardAsync(HeroCard heroCard, ITurnContext turnContext, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                var reply = turnContext.Activity.CreateReply();
                reply.Attachments.Add(heroCard.ToAttachment());
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "SenderService/SendHeroCardAsync");
            }
        }
    }
}
