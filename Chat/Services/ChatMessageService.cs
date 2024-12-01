using Chat.Data;
using Chat.Interfaces;
using Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Services;

public class ChatMessageService(ApplicationDbContext dbContext) : IChatMessageService
{
    public async Task<List<ChatMessage>> GetRecentMessagesAsync(string chatRoom)
    {
        // Retrieve the 50 most recent messages from the specified chat room
        return await dbContext.ChatMessages
            .Where(m => m.ChatRoom == chatRoom)
            .OrderBy(m => m.SentAt)
            .Take(50)
            .ToListAsync();
    }

    public async Task<ChatMessage> SaveMessageAsync(ChatConnection connection, string message, string sentiment)
    {
        // Create and save the message to the database
        var chatMessage = new ChatMessage
        {
            ChatRoom = connection.ChatRoom,
            UserName = connection.UserName,
            MessageText = message,
            Sentiment = sentiment,
            SentAt = DateTimeOffset.UtcNow
        };

        dbContext.ChatMessages.Add(chatMessage);
        await dbContext.SaveChangesAsync();

        return chatMessage;
    }
}