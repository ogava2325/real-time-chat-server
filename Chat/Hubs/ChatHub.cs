using Chat.Data;
using Chat.Interfaces;
using Chat.Models;
using Microsoft.AspNetCore.SignalR;

namespace Chat.Hubs;

/// <summary>
/// SignalR hub for real-time chat functionality.
/// Manages user connections, messages, and sentiment analysis.
/// </summary>
/// <param name="textAnalyticsService"></param>
/// <param name="dbContext"></param>
public class ChatHub(
    ILogger<ChatHub> logger,
    ApplicationDbContext dbContext,
    IChatMessageService chatMessageService,
    ITextAnalyticsService textAnalyticsService,
    IChatConnectionService chatConnectionService)
    : Hub<IChatClient>
{
    /// <summary>
    /// Allows user to join a chat room.
    /// </summary>
    /// <param name="userConnection">Details about user's connection</param>
    public async Task JoinChat(UserConnection userConnection)
    {
        try
        {
            logger.LogInformation("User {UserName} is joining chat room {ChatRoom}.", userConnection.UserName,
                userConnection.ChatRoom);

            // Add user to the specified SignalR group
            await Groups.AddToGroupAsync(Context.ConnectionId, userConnection.ChatRoom);

            // Save user's connection details to the database
            await chatConnectionService.SaveConnectionAsync(Context.ConnectionId, userConnection);

            // Get recent messages from the database
            var recentMessages = await chatMessageService.GetRecentMessagesAsync(userConnection.ChatRoom);

            // Send the recent messages to the caller
            foreach (var message in recentMessages)
            {
                await Clients.Caller.ReceiveMessageWithSentiment(
                    message.UserName,
                    message.MessageText,
                    message.Sentiment
                );
            }

            logger.LogInformation("User {UserName} successfully joined chat room {ChatRoom}.", userConnection.UserName,
                userConnection.ChatRoom);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while user {UserName} was joining chat room {ChatRoom}.",
                userConnection.UserName, userConnection.ChatRoom);
            await Clients.Caller.ReceiveMessage("System", "An error occurred while joining the chat.");
        }
    }

    /// <summary>
    /// Allows user to send a message to the chat room.
    /// </summary>
    /// <param name="message">Message that user want to send</param>
    public async Task SendMessage(string message)
    {
        try
        {
            var connection = await chatConnectionService.GetConnectionAsync(Context.ConnectionId);

            if (connection is null)
            {
                logger.LogWarning("Connection ID {ConnectionId} not found.", Context.ConnectionId);
                await Clients.Caller.ReceiveMessage("System", "Unable to send message. Connection not found.");
            }

            logger.LogInformation("User {UserName} is sending a message in chat room {ChatRoom}.", connection.UserName,
                connection.ChatRoom);

            // Analyze sentiment
            var sentiment = await textAnalyticsService.AnalyzeSentimentAsync(message);

            // Save message
            await chatMessageService.SaveMessageAsync(connection, message, sentiment);

            // Broadcast message
            await Clients.Group(connection.ChatRoom).ReceiveMessageWithSentiment(
                connection.UserName,
                message,
                sentiment
            );

            logger.LogInformation("Message from {UserName} in chat room {ChatRoom} successfully sent.",
                connection.UserName, connection.ChatRoom);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while sending a message.");
            await Clients.Caller.ReceiveMessage("System", "An error occurred while sending the message.");
        }
    }
}