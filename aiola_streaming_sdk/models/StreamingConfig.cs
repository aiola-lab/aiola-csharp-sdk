public class StreamingConfig
{
    public required string FlowId { get; set; }
    public required string ExecutionId { get; set; }
    public required string LangCode { get; set; }
    public required string TimeZone { get; set; }
    public required string Endpoint { get; set; }
    public required string Namespace { get; set; }
    public string Transports { get; set; } = "polling";
    public required string AuthType { get; set; }
    public required Dictionary<string, string>  AuthCredentials { get; set; }
    public required StreamingCallbacks Callbacks { get; set; }
}