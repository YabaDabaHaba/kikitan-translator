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

            if (!final)
            {
                SendPartial(recognized);

                return;
            }

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

    // VRChat rate limits the chatbox, and recognition emits partials far faster than it
    // will accept them, so partials are capped well below the final message rate.
    private static readonly TimeSpan PartialInterval = TimeSpan.FromMilliseconds(1200);
    private static DateTime _lastPartial = DateTime.MinValue;

    /// <summary>
    /// Nothing has been translated while speech is still in progress, so the recognised
    /// text is shown instead of an empty line. The final message then replaces it.
    /// </summary>
    private static void SendPartial(string recognized)
    {
        if (DateTime.UtcNow - _lastPartial < PartialInterval) return;

        var text = Annotate(recognized, AppConfig.ConfigObject.SourceLanguage);
        if (text.Trim().Length == 0) return;

        _lastPartial = DateTime.UtcNow;
        OscChatbox.SendMessage(text, true);
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
