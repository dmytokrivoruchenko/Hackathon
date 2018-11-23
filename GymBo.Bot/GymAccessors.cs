using System;
using GymBo.Bot.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace GymBo.Bot
{
    public class GymAccessors
    {
        public GymAccessors(ConversationState conversationState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
        }

        public ConversationState ConversationState { get; }

        public IStatePropertyAccessor<DialogState> ConversationDialogState { get; set; }

        public IStatePropertyAccessor<GymUser> GymUser { get; set; }
    }
}
