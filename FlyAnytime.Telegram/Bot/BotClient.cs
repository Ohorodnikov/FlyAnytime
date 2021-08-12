using FlyAnytime.Telegram.Bot.Commands;
using FlyAnytime.Telegram.Bot.InlineKeyboardButtons;
using FlyAnytime.Telegram.EF;
using FlyAnytime.Tools;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace FlyAnytime.Telegram.Bot
{
    public class BotClient
    {
        private ITelegramBotClient _botClient;
        IServiceProvider _serviceProvider;
        IBotHelper _botHelper;
        public BotClient(ITelegramBotClient botClient, IServiceProvider serviceProvider, IBotHelper botHelper)
        {
            _serviceProvider = serviceProvider;
            _botClient = botClient;
            _botHelper = botHelper;
        }

        public async Task ProcessUpdate(Update update)
        {
            Task handler = update.Type switch
            {
                UpdateType.Message => OnMessageReceive(update.Message),
                UpdateType.InlineQuery => OnInlineQuery(update.InlineQuery),
                UpdateType.ChosenInlineResult => OnChosenInlineResult(update.ChosenInlineResult),
                UpdateType.CallbackQuery => OnCallbackQuery(update.CallbackQuery),
                UpdateType.EditedMessage => OnEditedMessage(update.EditedMessage),
                UpdateType.ChannelPost => OnChannelPost(update.ChannelPost),
                UpdateType.EditedChannelPost => OnEditedChannelPost(update.EditedChannelPost),
                UpdateType.ShippingQuery => OnShippingQuery(update.ShippingQuery),
                UpdateType.PreCheckoutQuery => OnPreCheckoutQuery(update.PreCheckoutQuery),
                UpdateType.Poll => OnPoll(update.Poll),
                UpdateType.PollAnswer => OnPollAnswer(update.PollAnswer),
                UpdateType.MyChatMember => OnMyChatMember(update.MyChatMember),
                UpdateType.ChatMember => OnChatMember(update.ChatMember),
                _ => OnUnknown(),
            };
            try
            {
                await handler;
            }
            catch (Exception exception)
            {
                await HandleErrorAsync(exception);
            }
        }

        private async Task OnMessageReceive(Message message)
        {
            if (message.Type == MessageType.Text)
            {
                await SimpleMsgReceive(message);
                return;
            }

            if (message.Type == MessageType.ChatMembersAdded)
            {
                await ChatMembersAdded(message);
                return;
            }

            if (message.Type == MessageType.ChatMemberLeft)
            {
                await ChatMemberLeft(message);
                return;
            }
        }

        private async Task SimpleMsgReceive(Message message)
        {
            if (message.Type != MessageType.Text)
            {
                return;
            }

            var allCommands = _serviceProvider.GetServices<IBotCommand>();
            var messageCommand = message.Text.Split(" ")[0];
            foreach (var cmd in allCommands)
            {
                if (cmd.CanBeExecuted(messageCommand))
                {
                    await cmd.ExecuteAsync(message);
                    return;
                }
            }
        }

        private async Task ChatMembersAdded(Message message)
        {
            var bot = message.NewChatMembers.FirstOrDefault(x => x.Id == _botClient.BotId);

            if (bot != null)
            {
                await _botHelper.OnAddToGroup(message.Chat.Id);
                //bot was added
            }

            var user2join = message.NewChatMembers.Select(x => $"{x.FirstName} {x.LastName} (@{x.Username})");
            await _botClient.SendTextMessageAsync(message.Chat.Id, $"Welcome {string.Join(", ", user2join)}");
        }
        private async Task ChatMemberLeft(Message message)
        {
            if (message.LeftChatMember.Id == _botClient.BotId)
            {
                await _botHelper.OnRemoveFromGroup(message.Chat.Id);
                //bot was removed
            }
            await _botClient.SendTextMessageAsync(message.Chat.Id, $"Chat left {message.LeftChatMember.FirstName} {message.LeftChatMember.FirstName} (@{message.LeftChatMember.Username})");
        }

        private async Task OnEditedMessage(Message message)
        {

        }
        private async Task OnChannelPost(Message message)
        {

        }

        private async Task OnEditedChannelPost(Message message)
        {

        }
        private async Task OnInlineQuery(InlineQuery inlineQuery)
        {
            //https://core.telegram.org/bots/inline
        }

        private async Task OnChosenInlineResult(ChosenInlineResult chosenInlineResult)
        {
            //https://core.telegram.org/bots/inline
        }

        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            var pressedButton = _serviceProvider
                .GetServices<IInlineKeyboardButtonWithAction>()
                .FirstOrDefault(x => x.IsPressed(callbackQuery.Data))
                ;

            if (pressedButton == null)
                throw new Exception("Unknown button was pressed");

            await pressedButton.OnButtonPress(_botClient, callbackQuery.Message);
        }

        private async Task OnShippingQuery(ShippingQuery shippingQuery)
        {

        }

        private async Task OnPreCheckoutQuery(PreCheckoutQuery preCheckoutQuery)
        {

        }

        private async Task OnPoll(Poll poll)
        {

        }

        private async Task OnPollAnswer(PollAnswer pollAnswer)
        {

        }

        private async Task OnMyChatMember(ChatMemberUpdated cmu)
        {
            var currentStatus = cmu.NewChatMember.Status;
            var oldStatus = cmu.OldChatMember.Status;
            var userId = cmu.NewChatMember.User.Id;

            if (userId == _botClient.BotId)
            {
                if (oldStatus == ChatMemberStatus.Kicked)
                {
                    await _botHelper.OnReStartPrivateChat(cmu.Chat.Id);
                }
                if (oldStatus == ChatMemberStatus.Left)
                {
                    await _botHelper.OnAddToGroup(cmu.Chat.Id);
                }

                if (oldStatus == ChatMemberStatus.Administrator)
                {
                    await _botHelper.OnSetGroupAdmin(cmu.Chat.Id);
                }

                if (currentStatus == ChatMemberStatus.Kicked)
                {
                    await _botHelper.OnKickFromPrivateChat(cmu.Chat.Id);
                }
                if (currentStatus == ChatMemberStatus.Left)
                {
                    await _botHelper.OnRemoveFromGroup(cmu.Chat.Id);
                }

                if (currentStatus == ChatMemberStatus.Administrator)
                {
                    await _botHelper.OnRemoveFromGroupAdmin(cmu.Chat.Id);
                }
            }


            //if (cmu.NewChatMember.Status == ChatMemberStatus.Left)
            //    return;
            //var chat = await _botClient.GetChatAsync(cmu.Chat.Id);
            //var q2 = await _botClient.GetChatMembersCountAsync(cmu.Chat.Id);

            //await _botClient.SendTextMessageAsync(cmu.Chat.Id, $"OnMyChatMember: chatName = {chat.Title}, type = {chat.Type}, count = {q2}, action = {cmu.OldChatMember.Status} - {cmu.NewChatMember.Status}");
        }

        private async Task OnChatMember(ChatMemberUpdated cmu)
        {

        }

        private async Task OnUnknown()
        {

        }

        private async Task HandleErrorAsync(Exception e)
        {

        }
    }
}
