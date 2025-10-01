using MessageService.Domain.Enums;

namespace MessageService.Domain.Models;

/// <summary>
/// Represents the data required to send a message.
/// </summary>
public class SendMessageModel
{
    /// <summary>
    /// Gets or sets the list of recipient addresses (e.g., email, phone, or username depending on sender type).
    /// </summary>
    public List<string> Recipient { get; set; }

    /// <summary>
    /// Gets or sets the subject of the message (mainly used for email).
    /// </summary>
    public string? Subject { get; set; }

    /// <summary>
    /// Gets or sets the body content of the message.
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// Gets or sets the type of the message (instant or delayed).
    /// </summary>
    public MessageType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the scheduled date and time for sending the message (only used when delayed).
    /// </summary>
    public DateTime? SendAt { get; set; }
    
    /// <summary>
    /// Gets or sets the channel through which the message will be sent (Email, SMS, Telegram).
    /// </summary>
    public SenderType SenderType { get; set; }
}
