# aiOla CSharp SDKs

Welcome to the **aiOla CSharp SDKs** repository. This repository contains examples and documentation for various SDKs that integrate with aiOla's Text-to-Speech (TTS) and streaming services.

---

## Examples Overview

### aiOla Streaming SDK

#### 1. [Transcript and Events Example](https://github.com/aiola-lab/aiola-csharp-sdk/tree/main/examples/transcript_events_example/README.md)
This example demonstrates how to use the aiOla Streaming SDK to capture live transcripts and handle backend-triggered events.

- **Key Features**:
  - Real-time transcription.
  - Event-driven callbacks.
  - Internal / External Microphone.

#### 2.[Keyword Spotting Example](https://github.com/aiola-lab/aiola-csharp-sdk/tree/main/examples/keywords_spotting_example/README.md)

This example shows how to set up keyword spotting using the aiOla Streaming SDK.

- **Key Features**:
  - Spot predefined keywords in live streams.
  - Event-driven keyword matching.
 
#### 3. Supported Languages 
 en-EN, de-DE, fr-FR, zh-ZH, es-ES, pt-PT

---

### aiOla TTS SDK

#### 3. [Synthesize Speech Example](https://github.com/aiola-lab/aiola-csharp-sdk/tree/main/examples/synthesizeSpeech_example/README.md)
This example demonstrates how to convert text into speech and download the resulting audio file using the aiOla TTS SDK.

- **Key Features**:
  - Converts text into `.wav` audio files.
  - Supports voice selection.

#### 4. [Stream Speech Example](https://github.com/aiola-lab/aiola-csharp-sdk/tree/main/examples/streamSpeech_example/README.md)
This example shows how to stream text-to-speech in real-time, enabling audio playback before the entire text is processed.

- **Key Features**:
  - Real-time TTS streaming.
  - Immediate audio playback.

---

### Recorder App Tool

#### [Synthesize Speech Example](https://github.com/aiola-lab/aiola-csharp-sdk/tree/main/examples/utils/recorder)
The Recorder App Tool will simulate a streaming audio to the SDK. You can use this code or replace it with your own audio streaming mechnisem.

---

## Get Started

Clone the repository:
```bash
git clone https://github.com/aiola-lab/aiola-csharp-sdk.git
cd aiola-csharp-sdk
```

---

## Build 

To Build the Solution run
```bash
dotnet build
```

## Products 

The build will generate both:
- **aiola_streaming_sdk.dll**: Streaming SDK
- **aiola_tts_sdk.dll**: TTS SDK

## Dependencies

- NAudio: 
```bash
dotnet add package NAudio --version 2.2.1
```
- SharperPortAudio (RecorderApp)
```bash
dotnet add package SharperPortAudio --version 1.0.3
```