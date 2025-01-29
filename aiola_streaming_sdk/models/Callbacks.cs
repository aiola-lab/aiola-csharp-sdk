public class StreamingCallbacks
{
    public Func<Task> OnConnect { get; set; } = async () => await Task.CompletedTask;
    public Func<long, long, Task> OnDisconnect { get; set; } = async (_, __) => await Task.CompletedTask;
    public Func<Dictionary<string, string>, Task> OnError { get; set; } = async _ => await Task.CompletedTask;
    public Func<string, Task> OnTranscript { get; set; } = async _ => await Task.CompletedTask;
    public Func<string, Task> OnEvents { get; set; } = async _ => await Task.CompletedTask;
}