using System.Text;
using Serilog;

namespace KikitanTranslator.Utility;

/// <summary>
/// Formats Japanese using Windows' built-in Japanese phonetic analyser. It runs
/// locally and does not call a translation or dictionary service.
/// </summary>
public static class JapaneseTextFormatter
{
    private static bool _reportedUnavailable;

    public static string FormatForChatbox(string text)
    {
        var mode = AppConfig.ConfigObject.JapaneseReadingMode;
        if (mode == 0 || !ContainsKanji(text)) return text;

        var words = Analyse(text);
        if (words.Count == 0) return text;

        var output = new StringBuilder();

        foreach (var (source, reading) in words)
        {
            var isJapanese = source.Any(character => IsKanji(character) || IsKana(character));

            output.Append(mode switch
            {
                1 => isJapanese && reading.Length > 0 ? reading : source,
                3 => ContainsKanji(source) && reading.Length > 0 ? $"{source}[{reading}]" : source,
                _ => AddFurigana(source, reading)
            });
        }

        return output.Length == 0 ? text : output.ToString();
    }

    /// <summary>
    /// Builds the standalone readings line. Format 1 lists the readings of the kanji
    /// words, format 2 rewrites the whole line in hiragana. Empty when there is nothing
    /// to show, so callers can skip the line entirely.
    /// </summary>
    public static string BuildReadingLine(string text, int format)
    {
        if (format == 0 || string.IsNullOrEmpty(text) || !ContainsKanji(text)) return "";

        var words = Analyse(text);
        if (words.Count == 0) return "";

        if (format == 1)
        {
            var readings = words
                .Where(word => ContainsKanji(word.Source) && word.Reading.Length > 0)
                .Select(word => word.Reading)
                .ToList();

            return readings.Count == 0 ? "" : $"[{string.Join(", ", readings)}]";
        }

        var output = new StringBuilder();
        foreach (var (source, reading) in words)
        {
            var isJapanese = source.Any(character => IsKanji(character) || IsKana(character));
            output.Append(isJapanese && reading.Length > 0 ? reading : source);
        }

        return output.Length == 0 ? "" : $"[{output}]";
    }

    /// <summary>
    /// Segments the text and pairs each segment with its hiragana reading. The analyser
    /// rewrites latin letters and digits to full width, so its segmentation drives the
    /// slicing but the original characters are kept.
    /// </summary>
    private static List<(string Source, string Reading)> Analyse(string text)
    {
        try
        {
            var words = JapaneseReadings.GetWords(text);
            var result = new List<(string, string)>(words.Count);
            var offset = 0;

            foreach (var (display, yomi) in words)
            {
                var source = offset + display.Length <= text.Length
                    ? text.Substring(offset, display.Length)
                    : display;
                offset += display.Length;

                result.Add((source, ToHiragana(yomi)));
            }

            return result;
        }
        catch (Exception exception)
        {
            if (!_reportedUnavailable)
            {
                Log.Warning("[JP]  Japanese readings are unavailable, sending the text unchanged. Reason: {Reason}", exception.Message);
                _reportedUnavailable = true;
            }

            return [];
        }
    }

    private static string AddFurigana(string display, string reading)
    {
        if (!ContainsKanji(display) || string.IsNullOrEmpty(reading)) return display;

        var output = new StringBuilder();
        var readingOffset = 0;

        for (var index = 0; index < display.Length;)
        {
            if (!IsKanji(display[index]))
            {
                output.Append(display[index]);
                var character = ToHiragana(display[index].ToString());
                if (reading.AsSpan(readingOffset).StartsWith(character, StringComparison.Ordinal)) readingOffset += character.Length;
                index++;
                continue;
            }

            var kanjiStart = index;
            while (index < display.Length && IsKanji(display[index])) index++;
            var kanji = display[kanjiStart..index];

            // The following kana (okurigana) anchors the end of this kanji reading. It is
            // only looked ahead at here: index stays put so the branch above still emits it.
            var scan = index;
            while (scan < display.Length && !IsKanji(display[scan])) scan++;
            var followingKana = ToHiragana(display[index..scan]);
            var nextKanaOffset = followingKana.Length == 0
                ? reading.Length
                : reading.IndexOf(followingKana, readingOffset, StringComparison.Ordinal);

            if (nextKanaOffset < readingOffset) nextKanaOffset = reading.Length;
            var kanjiReading = reading[readingOffset..nextKanaOffset];
            output.Append(kanji).Append('[').Append(kanjiReading).Append(']');
            readingOffset = nextKanaOffset;
        }

        return output.ToString();
    }

    private static bool ContainsKanji(string text) => text.Any(IsKanji);

    private static bool IsKana(char character) => character >= 'ぁ' && character <= 'ヿ';

    private static bool IsKanji(char character) =>
        (character >= '㐀' && character <= '䶿') ||
        (character >= '一' && character <= '鿿') ||
        (character >= '豈' && character <= '﫿');

    private static string ToHiragana(string text)
    {
        var output = new StringBuilder(text.Length);
        foreach (var character in text)
        {
            output.Append(character >= 'ァ' && character <= 'ヶ'
                ? (char)(character - 0x60)
                : character);
        }

        return output.ToString();
    }
}
