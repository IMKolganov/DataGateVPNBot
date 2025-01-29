﻿using DataGateVPNBot.Contexts;
using DataGateVPNBot.Models;
using DataGateVPNBot.Services.DataServices.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace DataGateVPNBot.Services.DataServices;

public class IncomingMessageLogService : IIncomingMessageLogService
{
    private readonly ApplicationDbContext _dbContext;

    public IncomingMessageLogService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Log(ITelegramBotClient botClient, Message msg)
    {
        var log = new IncomingMessageLog
        {
            TelegramId = msg.From?.Id ?? 0,
            Username = msg.From?.Username,
            FirstName = msg.From?.FirstName,
            LastName = msg.From?.LastName,
            MessageText = msg.Text ?? "",
            ReceivedAt = DateTime.UtcNow
        };

        try
        {
            if (msg.Document != null)
            {
                await ProcessFileAsync(botClient, msg.Document.FileId, msg.Document.FileName, msg.Document.FileSize, "Document", log);
            }
            else if (msg.Photo?.Any() == true)
            {
                var largestPhoto = msg.Photo.OrderByDescending(p => p.FileSize).First();
                await ProcessFileAsync(botClient, largestPhoto.FileId, $"photo_{log.TelegramId}_{DateTime.UtcNow.Ticks}.jpg", largestPhoto.FileSize, "Photo", log);
            }
        }
        catch (Exception ex)
        {
            log.MessageText += $"\n[Error processing file: {ex.Message}]";
        }

        _dbContext.IncomingMessageLog.Add(log);
        await _dbContext.SaveChangesAsync();
    }

    private async Task ProcessFileAsync(
        ITelegramBotClient botClient, 
        string fileId, 
        string? fileName, 
        long? fileSize, 
        string fileType, 
        IncomingMessageLog log)
    {
        if (fileSize > 10 * 1024 * 1024)
            throw new Exception("File size exceeds the 10MB limit.");

        var filePath = Path.Combine("SavedFiles", fileName ?? "file");
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        await using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await botClient.GetInfoAndDownloadFile(fileId, fileStream);
        }

        log.FileType = fileType;
        log.FileId = fileId;
        log.FileName = fileName;
        log.FileSize = fileSize;
        log.FilePath = filePath;
    }
}
