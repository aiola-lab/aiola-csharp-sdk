public class StreamingConfig
{
    public string FlowId { get; set; }
    public string ExecutionId { get; set; }
    public string LangCode { get; set; }
    public string TimeZone { get; set; }
    public string Endpoint { get; set; }
    public string Namespace { get; set; }
    public string Transports { get; set; } = "polling";
    public string AuthType { get; set; }
    public Dictionary<string, string>  AuthCredentials { get; set; }
    public StreamingCallbacks Callbacks { get; set; }
}