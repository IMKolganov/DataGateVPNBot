using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DataGateVPNBotV1.Handlers;

public partial class TelegramUpdateHandler
{
    private async Task<Message> GetLogs(Message msg, int linesToRead = 100)
    {
        if (!File.Exists(_pathBotLog))
            throw new FileNotFoundException($"Log file not found: {_pathBotLog}");

        var lines = new LinkedList<string>();

        await using (var fileStream = new FileStream(_pathBotLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
        {
            fileStream.Seek(0, SeekOrigin.End);

            long position = fileStream.Position;
            int bufferSize = 1024;
            var buffer = new char[bufferSize];
            var currentLine = new LinkedList<char>();

            using (var streamReader = new StreamReader(fileStream))
            {
                while (position > 0 && lines.Count < linesToRead)
                {
                    position -= bufferSize;
                    if (position < 0)
                    {
                        bufferSize += (int)position;
                        position = 0;
                    }

                    fileStream.Seek(position, SeekOrigin.Begin);
                    var bytesRead = await streamReader.ReadAsync(buffer, 0, bufferSize);

                    for (int i = bytesRead - 1; i >= 0; i--)
                    {
                        if (buffer[i] == '\n')
                        {
                            lines.AddFirst(new string(currentLine.ToArray()));
                            currentLine.Clear();

                            if (lines.Count >= linesToRead)
                                break;
                        }
                        else
                        {
                            currentLine.AddFirst(buffer[i]);
                        }
                    }
                }

                if (currentLine.Count > 0)
                    lines.AddFirst(new string(currentLine.ToArray()));
            }
        }

        var logText = string.Join(Environment.NewLine, lines);

        if (logText.Length > 4096)
        {
            logText = logText.Substring(logText.Length - 4093) + "...";
        }

        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: logText,
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    private async Task<Message> SendFileLog(Message msg)
    {
        if (!File.Exists(_pathBotLog))
            throw new FileNotFoundException($"Log file not found: {_pathBotLog}");

        await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);

        await using var fileStream = new FileStream(_pathBotLog, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        try
        {
            return await _botClient.SendDocument(
                chatId: msg.Chat.Id,
                document: InputFile.FromStream(fileStream, Path.GetFileName(_pathBotLog)),
                caption: "Read https://github.com/IMKolganov/DataGateVPNBot"
            );
        }
        catch (Telegram.Bot.Exceptions.RequestException ex)
        {
            Console.WriteLine($"Telegram API Error: {ex.Message}");
            throw;
        }
    }
}