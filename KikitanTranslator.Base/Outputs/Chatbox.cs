using BuildSoft.VRChat.Osc;
using BuildSoft.VRChat.Osc.Chatbox;
using KikitanTranslator.Utility;
using Serilog;

namespace KikitanTranslator.Base.Outputs;

public class Chatbox : IOutput
{
    public Chatbox() => OscConnectionSettings.SendPort = AppConfig.ConfigObject.OscPort;
    
    public void Send(string recognized, string translated, bool final)
    {
        try
        {
            OscChatbox.SetIsTyping(!final);
            if (!final && !AppConfig.ConfigObject.SendWithoutWaitingForFinish) return;
            
            var recognizedForChatbox = Annotate(recognized, AppConfig.ConfigObject.SourceLanguage);
            var translatedForChatbox = Annotate(translated, AppConfig.ConfigObject.TargetLanguage);

            OscChatbox.SendMessage(Compose(recognized, translated, recognizedForChatbox, translatedForChatbox), true);
        } catch (Exception e)
        {
            Log.Error($"[OSC]  Error sending OSC message! Reason: {e}");
        }
    }

    private static string Compose(string recognized, string translated, string recognizedForChatbox,
        string translatedForChatbox)
    {
        var config = AppConfig.ConfigObject;

        // Readings come from the untouched text, not from an already annotated line.
        var readingLine = HasReadings(config.TargetLanguage)
            ? BuildReadingLine(translated, config.TargetLanguage, config.FuriganaLine)
            : BuildReadingLine(recognized, config.SourceLanguage, config.FuriganaLine);

        if (config.SpeechToTextOnly) return Attach(recognizedForChatbox, readingLine);
        if (config.TranslationOnly) return Attach(translatedForChatbox, readingLine);

        var originalFirst = config.ChatboxOrder == 1;
        var first = originalFirst ? recognizedForChatbox : translatedForChatbox;
        var second = originalFirst ? translatedForChatbox : recognizedForChatbox;

        if (!config.ChatboxSeparateLines) return Attach($"{first} ({second})", readingLine);

        var gap = new string('\n', Math.Clamp(config.ChatboxLineGap, 0, 5) + 1);
        var firstHasReadings = HasReadings(originalFirst ? config.SourceLanguage : config.TargetLanguage);

        // Position 0 keeps the readings directly under the annotated line; otherwise they go last.
        if (readingLine.Length > 0 && config.FuriganaLinePosition == 0 && firstHasReadings)
            return $"{first}\n{readingLine}{gap}{second}";

        return Attach($"{first}{gap}{second}", readingLine);
    }

    private static bool HasReadings(string language) => language is "ja" or "zh";

    private static string Annotate(string text, string language) => language switch
    {
        "ja" => JapaneseTextFormatter.FormatForChatbox(text),
        "zh" => ChineseTextFormatter.FormatForChatbox(text),
        _ => text
    };

    private static string BuildReadingLine(string text, string language, int format) => language switch
    {
        "ja" => JapaneseTextFormatter.BuildReadingLine(text, format),
        "zh" => ChineseTextFormatter.BuildReadingLine(text, format),
        _ => ""
    };

    private static string Attach(string text, string readingLine) =>
        readingLine.Length == 0 ? text : $"{text}\n{readingLine}";

    public bool IsDelayed() => true;
}
