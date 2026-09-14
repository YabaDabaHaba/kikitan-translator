using System.Text;

namespace KikitanTranslator.Utility;

/// <summary>
/// Formats Chinese for the chatbox. Readings come from <see cref="ChineseReadings"/>;
/// no correction is applied on top of them, so an ambiguous character is shown with
/// whatever reading the engine chose rather than a guess made here.
/// </summary>
public static class ChineseTextFormatter
{
    public static string FormatForChatbox(string text)
    {
        var mode = AppConfig.ConfigObject.ChineseReadingMode;
        if (mode == 0 || !ChineseReadings.ContainsHan(text)) return text;

        var readings = ChineseReadings.GetReadings(text, AppConfig.ConfigObject.PinyinToneMarks);

        return mode == 1 ? Transcribe(readings) : Annotate(readings);
    }

    /// <summary>
    /// The standalone pinyin line, wrapped in brackets. Empty when there is nothing to
    /// transcribe so callers can drop the line entirely.
    /// </summary>
    public static string BuildReadingLine(string text, int format)
    {
        if (format == 0 || string.IsNullOrEmpty(text) || !ChineseReadings.ContainsHan(text)) return "";

        var line = Transcribe(ChineseReadings.GetReadings(text, AppConfig.ConfigObject.PinyinToneMarks));

        return line.Length == 0 ? "" : $"[{line}]";
    }

    /// <summary>
    /// Replaces Han characters with their syllables, spacing syllables from whatever
    /// surrounds them so latin text and digits do not run into the pinyin.
    /// </summary>
    private static string Transcribe(IReadOnlyList<(string Source, string Reading)> readings)
    {
        var output = new StringBuilder();
        var previousWasSyllable = false;

        foreach (var (source, reading) in readings)
        {
            if (reading.Length > 0)
            {
                if (output.Length > 0 && !char.IsWhiteSpace(output[^1])) output.Append(' ');
                output.Append(reading);
                previousWasSyllable = true;

                continue;
            }

            var character = source[0];
            if (previousWasSyllable && !char.IsWhiteSpace(character) && !char.IsPunctuation(character))
                output.Append(' ');

            output.Append(source);
            previousWasSyllable = false;
        }

        return output.ToString();
    }

    /// <summary>
    /// Keeps the hanzi and puts each reading after it. The engine exposes no word
    /// boundaries, so this annotates per character rather than per word.
    /// </summary>
    private static string Annotate(IReadOnlyList<(string Source, string Reading)> readings)
    {
        var output = new StringBuilder();

        foreach (var (source, reading) in readings)
            output.Append(reading.Length > 0 ? $"{source}[{reading}]" : source);

        return output.ToString();
    }
}
