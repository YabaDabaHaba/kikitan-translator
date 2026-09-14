using System.Text.RegularExpressions;
using KikitanTranslator.Utility;
using Xunit;

namespace KikitanTranslator.Tests;

/// <summary>
/// Pins the behaviour of the pinyin engine so a future engine swap shows up as a
/// failing test rather than silently changing what players see.
/// </summary>
public class ChineseTextFormatterTests
{
    private const int PinyinOnly = 1;
    private const int AfterEachCharacter = 2;

    private static string Format(string text, int mode, bool toneMarks = true)
    {
        AppConfig.ConfigObject = new ConfigObject
        {
            ChineseReadingMode = mode,
            PinyinToneMarks = toneMarks
        };

        return ChineseTextFormatter.FormatForChatbox(text);
    }

    [Theory]
    // Phrase-aware readings: the whole reason this engine was chosen over character-only ones.
    [InlineData("我要去银行取钱", "háng")]   // 银行  not xíng
    [InlineData("我们重新开始", "chóng")]       // 重新  not zhòng
    [InlineData("我真的很喜欢听音乐", "yuè")] // 音乐  not lè
    [InlineData("我了解他", "liǎo")]                     // 了解  not le
    [InlineData("这样做的目的是什么？", "dì")] // 目的  not de
    [InlineData("他在沙发上睡着了", "zháo")] // 睡着  not zhe
    public void PhraseAwareReadingsAreUsed(string sentence, string expectedSyllable)
    {
        Assert.Contains(expectedSyllable, Format(sentence, PinyinOnly));
    }

    [Theory]
    // Documented engine limitations. These are wrong Chinese, recorded deliberately so the
    // day an engine gets them right, the test fails and we notice rather than guessing.
    // 我还没吃饭 - wants hái (adverb), engine gives huán (the verb reading).
    [InlineData("我还没吃饭", "huán", "hái")]
    // 我得走了 - wants děi (must); unresolved by every library tested, including jieba's POS.
    [InlineData("我得走了", "dé", "děi")]
    // 他笑着说 - wants zhe (aspect particle), engine picks the rare zhuó.
    [InlineData("他笑着说", "zhuó", "zhe")]
    // 他慢慢地走 - wants de (adverbial particle), engine gives the noun reading dì.
    [InlineData("他慢慢地走", "dì", "de")]
    public void KnownLimitationsStayVisible(string sentence, string actualSyllable, string linguisticallyCorrect)
    {
        var output = Format(sentence, PinyinOnly);

        Assert.Contains(actualSyllable, output);
        Assert.DoesNotContain(linguisticallyCorrect, output.Split(' '));
    }

    [Theory]
    [InlineData("我每天和朋友一起玩 VRChat")] // latin with a space
    [InlineData("VRChat很好玩！100%")]                            // latin, punctuation, digits
    [InlineData("我买了3个苹果和2个橙子")] // digits inline
    [InlineData("你好！你今天怎么样？")]   // full width punctuation
    [InlineData("你好😀🌸")]                          // emoji surrogate pairs
    [InlineData("Hello, no Chinese here")]                                        // nothing to annotate
    public void AnnotatingNeverLosesTheOriginalText(string text)
    {
        var annotated = Format(text, AfterEachCharacter);

        Assert.Equal(text, Regex.Replace(annotated, @"\[[^\]]*\]", ""));
    }

    [Fact]
    public void NonHanCharactersSurviveTranscription()
    {
        // Latin, digits and emoji must come through untouched, and must not be run
        // together with the syllable next to them.
        Assert.Equal("VRChat hěn hǎo wán！100%",
            Format("VRChat很好玩！100%", PinyinOnly));

        Assert.Equal("wǒ mǎi le 3 gè", Format("我买了3个", PinyinOnly));

        Assert.Equal("nǐ hǎo 😀🌸", Format("你好😀🌸", PinyinOnly));
    }

    [Fact]
    public void NeutralTonesCarryNoToneMark()
    {
        // 的 and 了 are neutral here and must not gain a mark.
        Assert.Equal("wǒ de shū", Format("我的书", PinyinOnly));
        Assert.Equal("wǒ chī le", Format("我吃了", PinyinOnly));
    }

    [Fact]
    public void ToneMarksCanBeTurnedOff()
    {
        Assert.Equal("nǐ hǎo", Format("你好", PinyinOnly));
        Assert.Equal("ni hao", Format("你好", PinyinOnly, toneMarks: false));
    }

    [Fact]
    public void ReadingsAreLowerCasedNotTitleCased()
    {
        var output = Format("你好", PinyinOnly);

        Assert.Equal(output.ToLowerInvariant(), output);
    }

    [Fact]
    public void ModeZeroLeavesTheTextAlone()
    {
        const string text = "你好吗";

        Assert.Equal(text, Format(text, 0));
    }

    [Fact]
    public void AnnotationIsPerCharacterBecauseTheEngineHasNoWordBoundaries()
    {
        // Documents the trade-off of not depending on a segmenter: 银行 is annotated as
        // two characters, not as one word.
        Assert.Equal("银[yín]行[háng]", Format("银行", AfterEachCharacter));
    }
}
