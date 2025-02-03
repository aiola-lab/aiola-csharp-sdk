# Aiola Streaming SDK

**Version**: `0.1.0`  
**Description**: A CSharp SDK for interacting with the Aiola Streaming Service, enabling real-time audio streaming and processing.

---

## Features

- **Real-Time Audio Streaming**: Stream audio to the Aiola service with configurable settings.
- **Built-in and External Microphone Support**: Choose between built-in and external microphones for streaming.
- **Customizable Audio Configuration**: Define sample rate, channels, and chunk size for your audio streams.
- **Type Safety**: Developed in TypeScript with type definitions for all core components.

---
## Build 

To Build the Solution run
```bash
dotnet build
```

## Products 

The build will generate both:
- **aiola_streaming_sdk.dll**: Streaming SDK.
Use the next dll in you code


---

## Requirements

- **.NET Core**: net8.0 or higher
- **Dependencies**:
  - `SocketIOClient`
  - `System.Net.WebSockets`
---

## What in the Project

- **`aiola_streaming_sdk`**: Core SDK logic for connecting and interacting with Aiola’s API.
- **`recorder`**: Handles audio streaming configuration and recording logic.

---

## Audio Configuration

The `recorder` uses a configurable schema defined as follows:

| Property     | Type    | Default  | Description                              |
|--------------|---------|----------|------------------------------------------|
| `DeviceNumber` | `number`| `0`  | Device ID                        |
| `SampleRate` | `number`| `16000`  | Sample rate in Hz                        |
| `Channels`   | `number`| `1`      | Number of audio channels (Mono = 1)      |
| `ChunkSize`  | `number`| `4096`   | Size of each audio chunk in bytes        |
| `AudioType`  | `string`| `wav`   | Audio Type (.wav)        |
| `Dtype`      | `string`| `'int16'`| Data type for audio samples              |

---

<br><br>


# Example Application: Using the SDK and RecorderApp

## Example :
This example demonstrates how to stream audio from the **recorder** to the Aiola service. The application connects to the Aiola API, streams audio data, and receives real-time transcripts and events.

```csharp

const string bearer_token = "< your-bearer-token >"; // The Bearer token, obtained upon registration with Aiola

var sdk = new AiolaStreamingClient(new StreamingConfig
{
    Endpoint = "< your-api-base-url >", // The URL of the Aiola server
    Namespace = "/events",  // Namespace for subscription: /transcript (for transcription) or /events (for transcription + LLM solution)
    FlowId = "<your-flow-id>", // One of the IDs from the flows created for the user
    ExecutionId = "1", // Unique identifier to trace execution
    LangCode = "en_US",
    TimeZone = "UTC",
    AuthType = "Bearer", // Supported authentication for the API
    Transports = "websocket", // Communication method: 'websocket' for L4 or 'polling' for L7
    AuthCredentials = new Dictionary<string, string> { { "token", bearer_token } },

    // 🔹 Register Callbacks Here
    Callbacks = new StreamingCallbacks
    {
        OnConnect = () => { Console.WriteLine("✅ Connected to Aiola Streaming API."); return Task.CompletedTask; },
        OnDisconnect = async (duration, totalAudioSent) =>
        {
            Console.WriteLine($"❌ Disconnected after {duration} ms. Total Audio Sent: {totalAudioSent} bytes.");
            await Task.CompletedTask;
        },

        OnTranscript = async transcript =>
        {
            Console.WriteLine($"📝 Transcript Received: {transcript}");
            await Task.CompletedTask;
        },

        OnEvents = async events =>
        {
            Console.WriteLine($"📢 Event Received: {events}");
            await Task.CompletedTask;
        },

        OnError = async error =>
        {
            Console.WriteLine($"❌ Error: {string.Join(", ", error.Select(kv => $"{kv.Key}: {kv.Value}"))}");
            await Task.CompletedTask;
        }
    }
});

var recorder = new RecorderApp(sdk,
    error => Console.WriteLine($"Streaming error: {error["audio_status"]}"),
    AudioConfig.Default()
);

Console.WriteLine("Starting audio streaming...");

...
```


### Explanation of Configuration Parameters

| Parameter	                  | Type	     | Description                                                                                                                          |
|-----------------------------|--------------|--------------------------------------------------------------------------------------------------------------------------------------|
| `Endpoint`	              | `string`     | The base URL of the Aiola server                                                                                                     |
| `AuthType`	              | `string`     | The authentication type, currently supporting "Bearer".                                                                           |
| `AuthCredentials` 	      | `object`     | An object containing credentials required for authentication.                                                                        |
| `FlowId`	                  |	`string`     | A unique identifier for the specific flow created for the user.                                                                      |
| `Namespace`	              |	`string`     | The namespace to subscribe to. Use /transcript for transcription or /events for transcription + LLM integration.                     |
| `Transports`	              |	`string[]`   | The communication method. Use ['websocket'] for Layer 4 (faster, low-level) or ['polling'] for Layer 7 (supports HTTP2, WAF, etc.).  |
| `ExecutionId`	              |	`string`     | A unique identifier to trace the execution of the session. Defaults to "1".                                                          |
| `LangCode`	              |	`string`	 | The language code for transcription. For example, "en_US" for US English.                                                            |
| `TimeZone`	              |	`string`	 | The time zone for aligning timestamps. Use "UTC" or any valid IANA time zone identifier.                                             |
| `Callbacks`	              |	`object`	 | An object containing the event handlers (callbacks) for managing real-time data and connection states                                |

<br>

### Supported Callbacks

| Callback	| Description |
|-----------|-------------|
|`onTranscript` |	Invoked when a transcript is received from the Aiola server. |
|`onError` |	Triggered when an error occurs during the streaming session. |
|`onEvents` |	Called when events (e.g., LLM responses or processing events) are received. |
|`OnConnect` |	Fired when the connection to the Aiola server is successfully established. |
|`OnDisconnect` |	Fired when the connection to the server is terminated. Includes session details such as duration and total audio streamed. |

---

### How It Works

1.	Endpoint:
    -	This is the base URL of the Aiola server where the client will connect.
2.	Authentication:
    -	The SDK uses Bearer for authentication. The Bearer token must be obtained during registration with Aiola.
3.	Namespace:
    -	Determines the type of data you want to subscribe to:
    -	`/transcript`: For transcription data only.
    -	`/events`: For transcription combined with LLM solution events.
4.	Transport Methods:
    -	Choose between:
        -	`'websocket'`: For **Layer 4** communication with lower latency.
        -	`'polling'`: For **Layer 7** communication, useful for environments with firewalls or HTTP2 support.
5.	Callbacks:
    -	These are functions provided by the user to handle various types of events or data received during the streaming session.
6.	Execution ID:
    -	Useful for tracing specific execution flows or debugging sessions.
7.	Language Code and Time Zone:
    -	Ensure the transcription aligns with the required language and time zone.