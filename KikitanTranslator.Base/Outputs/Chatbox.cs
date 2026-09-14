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
            
            var recognizedForChatbox = AppConfig.ConfigObject.SourceLanguage == "ja"
                ? JapaneseTextFormatter.FormatForChatbox(recognized)
                : recognized;
            var translatedForChatbox = AppConfig.ConfigObject.TargetLanguage == "ja"
                ? JapaneseTextFormatter.FormatForChatbox(translated)
                : translated;

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

        // Readings come from the untouched Japanese, not from an already annotated line.
        var japanese = config.TargetLanguage == "ja" ? translated
            : config.SourceLanguage == "ja" ? recognized
            : "";
        var readingLine = JapaneseTextFormatter.BuildReadingLine(japanese, config.FuriganaLine);

        if (config.SpeechToTextOnly) return Attach(recognizedForChatbox, readingLine);
        if (config.TranslationOnly) return Attach(translatedForChatbox, readingLine);

        var originalFirst = config.ChatboxOrder == 1;
        var first = originalFirst ? recognizedForChatbox : translatedForChatbox;
        var second = originalFirst ? translatedForChatbox : recognizedForChatbox;

        if (!config.ChatboxSeparateLines) return Attach($"{first} ({second})", readingLine);

        var gap = new string('\n', Math.Clamp(config.ChatboxLineGap, 0, 5) + 1);
        var firstIsJapanese = originalFirst ? config.SourceLanguage == "ja" : config.TargetLanguage == "ja";

        // Position 0 keeps the readings directly under the Japanese; otherwise they go last.
        if (readingLine.Length > 0 && config.FuriganaLinePosition == 0 && firstIsJapanese)
            return $"{first}\n{readingLine}{gap}{second}";

        return Attach($"{first}{gap}{second}", readingLine);
    }

    private static string Attach(string text, string readingLine) =>
        readingLine.Length == 0 ? text : $"{text}\n{readingLine}";

    public bool IsDelayed() => true;
}
