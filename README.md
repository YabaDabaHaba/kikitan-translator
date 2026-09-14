# Kikitan Translator

<div align="center">
<a href="https://sergiomarquina.booth.pm/items/6073050">
<img src="https://media.buyee.jp/guide/addtobuyee/assets/img/store-logo-booth.png" alt="drawing" width="150" align="center">
</a>
<a href="https://buymeacoffee.com/sergiomarquina">
<img src="https://i.imgur.com/l7NBjqk.png" alt="drawing" width="150" height="45" align="center">
</a>
<br><br>
<img width=500 src="https://i.imgur.com/C9fSR9O.png" />
</div>

### A VRChat translator built for helping you and the person/people you're talking with to understand each other regardless of language differences.

- **Translation:** Translation of your speech to the chatbox in more than 10 major languages (6 accents of English and 6 dialects of Spanish along with languages such as Japanese, Korean, Chinese, Italian, French, Turkish, Russian, Polish, Portugal, German, French, Arabic, Swedish and so on)
- **Transcription (Just Speech to Text):** If you don't want to translate, there is a transcription mode that sends whatever you say directly to the chatbox. Perfect for people that prefer to not speak in VRChat but be able to communicate with the convenience of speaking.

## Build and run (Windows)

The maintained desktop application is the .NET/Photino project in `KikitanTranslator.Photino`. The Tauri files under `KikitanTranslator.Photino/UserInterface/src-tauri` are legacy and are not used by this build.

### Prerequisites

- .NET 9 SDK
- Node.js 22 or newer
- Microsoft Edge WebView2 Runtime (included with current Windows 10/11 installations)

### Development

From the repository root, install the UI dependencies once:

```powershell
cd .\KikitanTranslator.Photino\UserInterface
npm.cmd ci
```

Start the UI development server in that terminal:

```powershell
npm.cmd run dev
```

In a second terminal at the repository root, run the desktop app:

```powershell
dotnet run --project .\KikitanTranslator.Photino\KikitanTranslator.Photino.csproj
```

### Release package

From the repository root:

```powershell
dotnet build .\KikitanTranslator.Photino\KikitanTranslator.Photino.csproj -c Release
```

The build restores the pinned, local `vpk` packaging tool automatically. The installer is written below `KikitanTranslator.Photino\bin\Release\net9.0-windows10.0.19041.0\win-x64\Release`.

The Windows build targets `net9.0-windows10.0.19041.0` so it can use the OS Japanese phonetic analyser for furigana; the Linux build (`-r linux-x64`) stays on plain `net9.0` and sends Japanese text unchanged.

## License

[Check the LICENSE.md for details](https://github.com/YusufOzmen01/kikitan-translator/blob/main/LICENSE.md)
