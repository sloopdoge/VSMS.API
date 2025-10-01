namespace MessageService.Domain.Enums;

/// <summary>
/// Defines the type of sender channel for the message.
/// </summary>
public enum SenderType
{
    /// <summary>
    /// Message will be sent via email.
    /// </summary>
    Email,

    /// <summary>
    /// Message will be sent via SMS.
    /// </summary>
    Sms,

    /// <summary>
    /// Message will be sent via Telegram.
    /// </summary>
    Telegram,
}