using Chat.Data;
using Chat.Interfaces;
using Chat.Models;
using Microsoft.EntityFrameworkCore;

namespace Chat.Services;

public class ChatConnectionService(ApplicationDbContext dbContext) : IChatConnectionService
{
    public async Task SaveConnectionAsync(string connectionId, UserConnection userConnection)
    {
        // Create and save the connection to the database
        var connection = new ChatConnection
        {
            ConnectionId = connectionId,
            UserName = userConnection.UserName,
            ChatRoom = userConnection.ChatRoom,
            ConnectedAt = DateTimeOffset.UtcNow
        };

        dbContext.ChatConnections.Add(connection);
        await dbContext.SaveChangesAsync();
    }

    public async Task<ChatConnection> GetConnectionAsync(string connectionId)
    {
        return await dbContext.ChatConnections.FirstOrDefaultAsync(c => c.ConnectionId == connectionId);
    }
}