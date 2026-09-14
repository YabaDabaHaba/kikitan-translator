namespace KikitanTranslator.Utility;

/// <summary>
/// Word/reading pairs from the OS. Only the Windows build can supply these; the
/// plain build returns nothing so callers fall back to the unmodified text.
/// </summary>
internal static class JapaneseReadings
{
    public static IReadOnlyList<(string Display, string Yomi)> GetWords(string text)
    {
#if WINDOWS10_0_19041_0_OR_GREATER
        List<(string, string)> words = [];
        Exception? failure = null;

        // The analyser will not activate on an MTA thread and chatbox output runs on
        // the thread pool, so the call is marshalled onto a dedicated STA thread.
        var worker = new Thread(() =>
        {
            try
            {
                foreach (var word in Windows.Globalization.JapanesePhoneticAnalyzer.GetWords(text))
                    words.Add((word.DisplayText, word.YomiText));
            }
            catch (Exception exception)
            {
                failure = exception;
            }
        }) { IsBackground = true };

        worker.SetApartmentState(ApartmentState.STA);
        worker.Start();
        worker.Join();

        if (failure != null) throw failure;

        return words;
#else
        _ = text;
        return [];
#endif
    }
}
