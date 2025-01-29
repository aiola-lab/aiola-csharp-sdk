public class AiolaStreamingError : Exception
{
    public Dictionary<string, object>? Details { get; }

    public AiolaStreamingError(string message, Dictionary<string, object>? details = null)
        : base(message)
    {
        Details = details;
    }
}