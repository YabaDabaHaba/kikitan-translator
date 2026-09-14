using KikitanTranslator.Utility;
using Xunit;

namespace KikitanTranslator.Tests;

/// <summary>
/// Guards the Japanese output against the language routing added for Chinese.
/// </summary>
public class JapaneseUnaffectedTests
{
    private static string Format(string text, int mode)
    {
        AppConfig.ConfigObject = new ConfigObject { JapaneseReadingMode = mode };

        return JapaneseTextFormatter.FormatForChatbox(text);
    }

    [Fact]
    public void HiraganaModeStillConvertsKanji()
    {
        Assert.Equal("にほんごをべんきょうしています",
            Format("日本語を勉強しています", 1));
    }

    [Fact]
    public void FuriganaPerKanjiKeepsOkurigana()
    {
        // 食べ物を食べる - the okurigana must survive, not be swallowed by the reading.
        Assert.Equal("食[た]べ物[もの]を食[た]べる",
            Format("食べ物を食べる", 2));
    }

    [Fact]
    public void FuriganaPerWordGroupsTheWord()
    {
        Assert.Equal("食べ物[たべもの]を食べ[たべ]る",
            Format("食べ物を食べる", 3));
    }

    [Fact]
    public void LatinTextIsNotWidened()
    {
        // VRChat must not become ＶＲＣｈａｔ.
        Assert.StartsWith("VRChat", Format("VRChatで遊びます！", 2));
    }

    [Fact]
    public void ReadingLineIsUnchanged()
    {
        AppConfig.ConfigObject = new ConfigObject();

        Assert.Equal("[おまえ, げんき]",
            JapaneseTextFormatter.BuildReadingLine("お前は元気ですか", 1));
        Assert.Equal("[おまえはげんきですか]",
            JapaneseTextFormatter.BuildReadingLine("お前は元気ですか", 2));
    }
}
