using ToolGood.Words.Pinyin;

namespace KikitanTranslator.Utility;

/// <summary>
/// Han character readings. This is the only file that knows which pinyin engine is in
/// use, so replacing ToolGood with a segmentation/part-of-speech based engine means
/// changing this file alone - the formatter, the chatbox output and the UI are unaware.
/// </summary>
internal static class ChineseReadings
{
    /// <summary>
    /// Pairs every character of the text with its reading. Characters that are not Han
    /// get an empty reading and must be emitted unchanged by callers, which is what
    /// keeps latin text, digits, punctuation and emoji intact.
    /// </summary>
    public static IReadOnlyList<(string Source, string Reading)> GetReadings(string text, bool toneMarks)
    {
        // The engine returns exactly one entry per input character, so entries map
        // straight back onto the original text by index.
        var readings = WordsHelper.GetPinyinList(text, toneMarks);
        var result = new List<(string, string)>(text.Length);

        for (var index = 0; index < text.Length; index++)
        {
            var reading = IsHan(text[index]) && index < readings.Length
                ? readings[index].ToLowerInvariant()
                : "";

            result.Add((text[index].ToString(), reading));
        }

        return result;
    }

    public static bool ContainsHan(string text) => text.Any(IsHan);

    public static bool IsHan(char character) =>
        (character >= '一' && character <= '鿿') ||
        (character >= '㐀' && character <= '䶿') ||
        (character >= '豈' && character <= '﫿');
}
