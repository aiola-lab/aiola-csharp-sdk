public class AuthenticationError : AiolaStreamingError
{
    public AuthenticationError(string message, Dictionary<string, object>? details = null)
        : base(message, details) { }
}