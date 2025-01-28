using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBotV1.Services.DataServices.Interfaces;

public interface IIncomingMessageLogService
{
    Task Log(ITelegramBotClient botClient, Message msg);
}