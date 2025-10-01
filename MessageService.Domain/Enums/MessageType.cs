namespace MessageService.Domain.Enums;

/// <summary>
/// Defines the type of message delivery (immediate or scheduled).
/// </summary>
public enum MessageType
{
    /// <summary>
    /// Message should be sent instantly.
    /// </summary>
    Instant,

    /// <summary>
    /// Message is scheduled to be sent at a later time.
    /// </summary>
    Delayed,
}