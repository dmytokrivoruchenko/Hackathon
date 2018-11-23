using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GymBo.Bot.Models;
using GymBo.Bot.Services.MachineLearning;
using GymBo.Bot.Services.Sender;
using GymBo.Bot.Services.Smba;
using GymBo.Bot.Services.Sql;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace GymBo.Bot
{
    public class GymBot : IBot
    {
        private readonly GymAccessors _accessors;

        private readonly ILogger _logger;
        private readonly DialogSet _dialogs;
        private GymUser _user = null;
        private readonly ISqlServices _sqlServices;
        private readonly ISmbaServices _smbaServices;
        private readonly ISenderServices _senderServices;
        private readonly IMachineLearningServices _machineLearningServices;

        public GymBot(GymAccessors accessors, ILoggerFactory loggerFactory, ISmbaServices smbaServices, IMachineLearningServices machineLearningServices, ISenderServices senderServices, ISqlServices sqlServices)
        {
            try
            {
                if (loggerFactory == null)
                {
                    throw new System.ArgumentNullException(nameof(loggerFactory));
                }

                var profile_slots = new List<SlotDetails>
                {
                    new SlotDetails("age", "text", MyConstant.EnterAge),
                    new SlotDetails("sex", "text", MyConstant.EnterSex),
                    new SlotDetails("height", "text", MyConstant.EnterHeight),
                    new SlotDetails("weight", "text", MyConstant.EnterWeight),
                    new SlotDetails("daysPerWeek", "text", MyConstant.EnterDaysPerWeek),
                    new SlotDetails("trainingTime", "text", MyConstant.EnterTrainingTime),
                    new SlotDetails("power", "text", MyConstant.EnterPower),
                    new SlotDetails("simulators", "text", MyConstant.EnterSimulators),
                };

                _logger = loggerFactory.CreateLogger<GymBot>();
                _logger.LogTrace("EchoBot turn start.");
                _smbaServices = smbaServices;
                _machineLearningServices = machineLearningServices;
                _senderServices = senderServices;
                _sqlServices = sqlServices;
                _dialogs = new DialogSet(accessors.ConversationDialogState);
                _dialogs.Add(new SlotFillingDialog("profile", profile_slots));
                _dialogs.Add(new WaterfallDialog("root", new WaterfallStep[] { StartDialogAsync, ProcessResultsAsync }));
                _accessors = accessors ?? throw new System.ArgumentNullException(nameof(accessors));
                _dialogs.Add(new TextPrompt("text"));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task OnTurnAsync(ITurnContext turnContext,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            try
            {
                var reply = turnContext.Activity.CreateReply();
                reply.Text = null;
                reply.Type = ActivityTypes.Typing;
                await turnContext.SendActivityAsync(reply, cancellationToken);
                if (turnContext.Activity.Type == ActivityTypes.ContactRelationUpdate)
                {
                    if (turnContext.Activity.Action.Equals("add", StringComparison.CurrentCultureIgnoreCase))
                    {
                        await _accessors.GymUser.DeleteAsync(turnContext, cancellationToken);
                        var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                        var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                        if (results.Status == DialogTurnStatus.Empty ||
                            dialogContext.ActiveDialog.Id.Equals("text"))
                        {
                            var name = turnContext.Activity.From.Name;
                            await _senderServices.SendMessageAsync(
                                $"Hi {name}, I'm Gymbo, I don't know you yet, let's get acquainted. Type anything to get started.",
                                turnContext, cancellationToken, _logger);
                            await dialogContext.BeginDialogAsync("root", null, cancellationToken);
                        }
                    }
                    else if (turnContext.Activity.Action.Equals("remove", StringComparison.CurrentCultureIgnoreCase))
                    {
                        _user = _accessors.GymUser.GetAsync(turnContext, () => null, cancellationToken).Result ??
                            _sqlServices.GetGymUser(turnContext.Activity.Conversation.Id);

                        if (_user != null)
                        {
                            _sqlServices.DeleteUser(_user.ChannelId);
                        }

                    }
                    else
                    {
                        // another action type
                    }
                }
                else if (turnContext.Activity.Type == ActivityTypes.Message)
                {
                    if (turnContext.Activity.Attachments != null)
                    {
                        if (turnContext.Activity.Attachments.Count > 0)
                        {
                            if (turnContext.Activity.Attachments.Select(x => x.ContentType.Equals("image")).Any())
                            {
                                var attachments =
                                    turnContext.Activity.Attachments.Where(x => x.ContentType.Equals("image"));
                                foreach (var attachment in attachments)
                                {
                                    await _senderServices.SendMessageAsync(
                                        "I see that you sent me something :) I will try to recognize what is in the photo :)",
                                        turnContext, cancellationToken, _logger);
                                    var uri = await _smbaServices.GetAttachmentAsync(attachment);
                                    TrainingApparatus trainingApparatus = new TrainingApparatus(uri.AbsoluteUri);
                                    var simulatorName = await _machineLearningServices.SendDataToMlAsync(trainingApparatus);
                                    await _senderServices.SendMessageAsync($"I am sure it is *{simulatorName}*! (flex)",
                                        turnContext, cancellationToken, _logger);
                                }
                            }
                        }
                    }
                    else
                    {
                        _user =
                            _accessors.GymUser.GetAsync(turnContext, () => null, cancellationToken).Result ??
                            _sqlServices.GetGymUser(turnContext.Activity.Conversation.Id);

                        if (_user == null)
                        {
                            var dialogContext = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                            var results = await dialogContext.ContinueDialogAsync(cancellationToken);
                            if (results.Status == DialogTurnStatus.Empty)
                            {
                                await dialogContext.BeginDialogAsync("root", null, cancellationToken);
                            }
                        }
                        else
                        {
                            var dc = await _dialogs.CreateContextAsync(turnContext, cancellationToken);
                            await dc.ContinueDialogAsync(cancellationToken);
                            await _accessors.ConversationState.SaveChangesAsync(turnContext,
                                cancellationToken: cancellationToken);
                            var responseMessage = $"Turn : You sent '{turnContext.Activity.Text}'\n";
                            await turnContext.SendActivityAsync(responseMessage, cancellationToken: cancellationToken);
                        }
                    }
                }
                else

                {
                    await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected",
                        cancellationToken: cancellationToken);
                }

                await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private async Task<DialogTurnResult> StartDialogAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Start the child dialog. This will run the top slot dialog than will complete when all the properties are gathered.
            return await stepContext.BeginDialogAsync("profile", null, cancellationToken);
        }

        private async Task<DialogTurnResult> ProcessResultsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            try
            {
                // To demonstrate that the slot dialog collected all the properties we will echo them back to the user.
                if (stepContext.Result is IDictionary<string, object> result && result.Count > 0)
                {
                    GymUser gymUser = new GymUser()
                    {
                        Age = Convert.ToInt32(result["age"]),
                        ChannelId = stepContext.Context.Activity.Conversation.Id,
                        DaysPerWeek = Convert.ToDouble(result["daysPerWeek"]),
                        GymUserId = 0,
                        Height = Convert.ToDouble(result["height"]),
                        Name = stepContext.Context.Activity.From.Name,
                        Power = Convert.ToDouble(result["power"]),
                        Sex = result["sex"].ToString(),
                        TrainingTime = Convert.ToDouble(result["trainingTime"]),
                        Weight = Convert.ToDouble(result["weight"]),
                    };
                    var sumulators = result["simulators"].ToString().Split(", ");

                    await _sqlServices.CreateUserAsync(gymUser);
                    await _sqlServices.AddSimulatorsToUserAsync(stepContext.Context.Activity.Conversation.Id, sumulators);

                    await stepContext.Context.SendActivityAsync(
                        MessageFactory.Text($"Nice to meet you, *{stepContext.Context.Activity.From.Name}*, now you can learn my abilities with the command: *'help'*."), cancellationToken);
                }

                return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

    }
}
