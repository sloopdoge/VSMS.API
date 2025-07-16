using MessageService.Domain.Models;

namespace MessageService.Infrastructure.Interfaces;

public interface IMessageServiceClient
{
    /// <summary>
    /// Sends a message to the specified recipients using the configured message service.
    /// </summary>
    /// <param name="messageModel">The message model containing recipients, subject, body, and message type.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result is <c>true</c> if the message was sent successfully; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> SendMessage(SendMessageModel  messageModel);
}