# aiOla TTS Synthesize Speech Example

**Version**: `0.1.0`

This example demonstrates how to use the aiOla TTS SDK to convert text into speech and save the resulting audio as a `.wav` file.

---

## How It Works

- Converts text into a `.wav` audio file using the aiOla TTS `/synthesize/stream` endpoint.
- Allows voice selection for speech synthesis.
- Saves the synthesized audio locally for playback or further processing.

---

## Prerequisites

- **.NET Core**: net8.0 or higher
- **Dependencies**:
  - `NAudio`

## What in the Project

- **`aiola_tts_sdk`**: Core SDK logic for connecting and interacting with Aiola’s API.

---

## Code Highlights

### Initialize the TTS Client

```csharp
string ttsUrl = "<your-api-base-url>/api/tts";  // Replace with your API base URL
string bearerToken = "<your-bearer-token>";  // Replace with your Bearer token
string voice = "af_bella";

var ttsClient = new AiolaTTSClient(ttsUrl, bearerToken);

```

### Audio Format Options

The SDK supports multiple audio formats for the synthesized speech. You can specify the format when initializing the client:

```csharp
ttsClient = AiolaTTSClient(string baseUrl, string bearerToken, AudioFormat audioFormat = AudioFormat.LINEAR16)
```

Supported formats:
- LINEAR16
- PCM

### Stream Speech

```csharp
string text = "Hello, this is a test of the aiOla TTS synthesis feature. You can download the audio after processing.";
byte[] audioData = await ttsClient.SynthesizeStreamAsync(text, voice);
```
