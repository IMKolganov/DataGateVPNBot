using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace DataGateVPNBotV1.Handlers;

public partial class TelegramUpdateHandler
{

    private async Task<Message> GetMyFiles(Message msg)
    {
        _logger.LogInformation("GetMyFiles started for user: {TelegramId}", msg.From?.Id);

        try
        {
            _logger.LogInformation("Fetching client configurations...");
            var clientConfigFiles = await _openVpnClientService.GetAllClientConfigurations(msg.From!.Id);
            _logger.LogInformation("Fetched {Count} configuration files.", clientConfigFiles.FileInfo.Count);

            if (clientConfigFiles.FileInfo.Count <= 0)
            {
                return await _botClient.SendMessage(
                    chatId: msg.Chat.Id,
                    text: await GetLocalizationTextAsync("FilesNotFoundError", msg.From!.Id),
                    replyMarkup: new ReplyKeyboardRemove()
                );
            }

            if (clientConfigFiles.FileInfo.Count >= 2)
            {
                _logger.LogInformation("Multiple configuration files detected. Preparing media group...");
                var mediaGroup = new List<IAlbumInputMedia>();
                var openStreams = new List<FileStream>();

                try
                {
                    foreach (var fileInfo in clientConfigFiles.FileInfo)
                    {
                        _logger.LogInformation("Processing file: {FileName} at {FilePath}", fileInfo.Name, fileInfo.FullName);

                        var fileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read, FileShare.Read);
                        openStreams.Add(fileStream);

                        var inputFile = new InputFileStream(fileStream, fileInfo.Name);
                        var media = new InputMediaDocument(inputFile)
                        {
                            Caption = fileInfo.Name
                        };
                        mediaGroup.Add(media);
                    }

                    _logger.LogInformation("Sending media group...");
                    var m = await _botClient.SendMediaGroup(
                        chatId: msg.Chat.Id,
                        media: mediaGroup
                    );
                    _logger.LogInformation("Media group sent successfully.");

                    return m.FirstOrDefault() ?? throw new InvalidOperationException("No messages returned after sending media group.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending media group.");
                    throw;
                }
                finally
                {
                    foreach (var stream in openStreams)
                    {
                        stream.Close();
                        _logger.LogInformation("Closed stream for file: {StreamPath}", stream.Name);
                    }
                }
            }
            else
            {
                await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);
                _logger.LogInformation("Single configuration file detected.");
                var clientConfigFile = clientConfigFiles.FileInfo.FirstOrDefault()
                                       ?? throw new InvalidOperationException("No configuration file found.");

                _logger.LogInformation("Reading file: {FileName} from {FilePath}", clientConfigFile.Name,
                    clientConfigFile.FullName);
                await using var fileStream = new FileStream(clientConfigFile.FullName,
                    FileMode.Open, FileAccess.Read, FileShare.Read);

                _logger.LogInformation("Sending single document...");
                var sentMessage = await _botClient.SendDocument(
                    chatId: msg.Chat.Id,
                    document: InputFile.FromStream(fileStream, clientConfigFile.Name),
                    caption: clientConfigFiles.Message
                );
                _logger.LogInformation("Document sent successfully.");

                return sentMessage;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred in GetMyFiles for user: {TelegramId}", msg.From?.Id);
            throw;
        }
    }

    private async Task<Message> MakeNewVpnFile(Message msg)
    {
        // Generate the client configuration file
        var clientConfigFile = await _openVpnClientService.CreateClientConfiguration(msg.From!.Id);
        if (clientConfigFile.FileInfo != null)
        {
            _logger.LogInformation("Client configuration created successfully in UpdateHandler.");
            await _botClient.SendChatAction(msg.Chat.Id, ChatAction.UploadDocument);
            // Send the .ovpn file to the user
            await using var fileStream = new FileStream(clientConfigFile.FileInfo.FullName, FileMode.Open, FileAccess.Read,
                FileShare.Read);
            return await _botClient.SendDocument(
                chatId: msg.Chat.Id,
                document: InputFile.FromStream(fileStream, clientConfigFile.FileInfo.Name),
                caption: clientConfigFile.Message
            );
        }
        else
        {
            return await _botClient.SendMessage(
                chatId: msg.Chat.Id,
                text: clientConfigFile.Message,
                replyMarkup: new ReplyKeyboardRemove()
            );
        }
    }
    
    private async Task<Message> DeleteAllFiles(Message msg)
    {
        await _openVpnClientService.DeleteAllClientConfigurations(msg.From!.Id);
        return await _botClient.SendMessage(
            chatId: msg.Chat.Id,
            text: await GetLocalizationTextAsync("SuccessfullyDeletedAllFile", msg.From!.Id),
            replyMarkup: new ReplyKeyboardRemove()
        );
    }

    private async Task<Message> DeleteSelectedFile(Message msg)
    {
        var clientConfigFiles = await _openVpnClientService.GetAllClientConfigurations(msg.From!.Id);
        var rows = new List<InlineKeyboardButton[]>();

        var currentRow = new List<InlineKeyboardButton>();
        foreach (var fileInfo in clientConfigFiles.FileInfo)
        {
            currentRow.Add(InlineKeyboardButton.WithCallbackData(fileInfo.Name, $"/delete_file {fileInfo.Name}"));

            if (currentRow.Count == 2)
            {
                rows.Add(currentRow.ToArray());
                currentRow.Clear();
            }
        }
        
        if (currentRow.Count > 0)
        {
            rows.Add(currentRow.ToArray());
        }

        var inlineMarkup = new InlineKeyboardMarkup(rows);
        return await _botClient.SendMessage(
            msg.Chat,
            await GetLocalizationTextAsync("ChooseFileForDelete", msg.From!.Id),
            replyMarkup: inlineMarkup
        );
    }

    private async Task DeleteFile(long telegramId, string fileName)
    {
        await _openVpnClientService.DeleteClientConfiguration(telegramId, fileName);
        await _botClient.SendMessage(
            chatId: telegramId,
            text: await GetLocalizationTextAsync("SuccessfullyDeletedFile", telegramId),
            replyMarkup: new ReplyKeyboardRemove()
        );
    }
}