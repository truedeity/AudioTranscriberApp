# AudioTranscriberApp

AudioTranscriberApp is a real-time speech-to-text application built in C# using the Vosk speech recognition toolkit and NAudio for capturing microphone input. The transcriptions are saved to a local SQL Server database (LocalDB). The application also supports automatic downloading and extraction of the Vosk model if it is not present.

## Features

- **Real-time speech-to-text transcription** using Vosk and NAudio.
- **Automatic model download**: The required Vosk model is automatically downloaded and extracted if not available.
- **Local transcription storage**: Transcriptions are stored in a local SQL Server (LocalDB) database.
- **Dynamic model path detection**: The application handles different extraction folder structures and finds the correct model folder.

## Prerequisites

1. **.NET SDK**: Ensure you have .NET SDK installed. You can download it from [here](https://dotnet.microsoft.com/download).
2. **SQL Server LocalDB**: SQL Server LocalDB should be installed, which is bundled with Visual Studio. If you don't have it installed, you can download and install it [here](https://docs.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb).
3. **Internet connection**: The Vosk model will be downloaded from the internet on first run.

## How to Run

### Step 1: Clone the Repository

```bash
git clone ...
cd AudioTranscriberApp
