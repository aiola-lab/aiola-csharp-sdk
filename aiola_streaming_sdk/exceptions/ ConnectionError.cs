public class ConnectionError : AiolaStreamingError
{
    public ConnectionError(string message, Dictionary<string, object>? details = null)
        : base(message, details) { }
}