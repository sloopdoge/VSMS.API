using MessageService.Domain.Enums;

namespace MessageService.Domain.Models;

/// <summary>
/// Represents the data required to send a message.
/// </summary>
public class SendMessageModel
{
    /// <summary>
    /// Gets or sets the list of recipient email addresses.
    /// </summary>
    public List<string> Recipient { get; set; }

    /// <summary>
    /// Gets or sets the subject of the message.
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the body content of the message.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the type of the message (e.g., Email, SMS).
    /// </summary>
    public MessageType Type { get; set; }
}
