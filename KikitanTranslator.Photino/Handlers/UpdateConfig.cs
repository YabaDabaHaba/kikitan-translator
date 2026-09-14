using KikitanTranslator.Utility;
using Newtonsoft.Json;
using Serilog;

namespace KikitanTranslator.Photino.Handlers;

public class ConfigUpdate
{
    [JsonProperty("field")] public string Field;
    [JsonProperty("value")] public object Value;
}

public class UpdateConfig(Manager manager) : IHandler
{
    public async Task<string?> OnDataReceived(string data)
    {
        bool doNotRestart = false;
        
        var d = JsonConvert.DeserializeObject<ConfigUpdate>(data);
        if (d == null) return "";

        switch (d.Field)
        {
            case "language":
                AppConfig.ConfigObject.Language = (string) d.Value;
                doNotRestart = true;
                
                break;
            case "source_language":
                AppConfig.ConfigObject.SourceLanguage = (string) d.Value;
                
                break;
            case "target_language":
                AppConfig.ConfigObject.TargetLanguage = (string) d.Value;
                
                break;
            case "light_mode":
                AppConfig.ConfigObject.LightMode = (bool) d.Value;
                doNotRestart = true;
                
                break;
            case "speech_to_text_only":
                AppConfig.ConfigObject.SpeechToTextOnly = (bool) d.Value;
                
                break;
            case "microphone":
                AppConfig.ConfigObject.Microphone = (string) d.Value;
                
                break;
            case "translation_only":
                AppConfig.ConfigObject.TranslationOnly = (bool) d.Value;
                
                break;
            case "japanese_reading_mode":
                AppConfig.ConfigObject.JapaneseReadingMode = Convert.ToInt32((long) d.Value);

                break;
            case "chatbox_separate_lines":
                AppConfig.ConfigObject.ChatboxSeparateLines = (bool) d.Value;

                break;
            case "chatbox_order":
                AppConfig.ConfigObject.ChatboxOrder = Convert.ToInt32((long) d.Value);

                break;
            case "always_on_top":
                AppConfig.ConfigObject.AlwaysOnTop = (bool) d.Value;

                break;
            case "chinese_reading_mode":
                AppConfig.ConfigObject.ChineseReadingMode = Convert.ToInt32((long) d.Value);

                break;
            case "pinyin_tone_marks":
                AppConfig.ConfigObject.PinyinToneMarks = (bool) d.Value;

                break;
            case "chatbox_line_gap":
                AppConfig.ConfigObject.ChatboxLineGap = Convert.ToInt32((long) d.Value);

                break;
            case "furigana_line":
                AppConfig.ConfigObject.FuriganaLine = Convert.ToInt32((long) d.Value);

                break;
            case "furigana_line_position":
                AppConfig.ConfigObject.FuriganaLinePosition = Convert.ToInt32((long) d.Value);

                break;
            case "disable_when_muted":
                AppConfig.ConfigObject.DisableWhenMuted = (bool) d.Value;
                
                break;
            case "send_without_waiting_for_finish":
                AppConfig.ConfigObject.SendWithoutWaitingForFinish = (bool) d.Value;
                
                break;
            case "chatbox_wait_per_char_ms":
                AppConfig.ConfigObject.ChatboxWaitPerCharMs = Convert.ToInt32((long) d.Value);
                
                break;
            case "osc_port":
                AppConfig.ConfigObject.OscPort = Convert.ToInt32((long) d.Value);
                
                break;
            case "send_to_chatbox":
                AppConfig.ConfigObject.SendToChatbox = (bool) d.Value;
                
                break;
            case "send_user_data":
                AppConfig.ConfigObject.SendUserData = (bool) d.Value;
                
                break;
            case "recognizer":
                AppConfig.ConfigObject.Recognizer = Convert.ToInt32((long) d.Value);
                
                if (AppConfig.ConfigObject.Recognizer == 2) AppConfig.ConfigObject.Translator = 2;
                else if (AppConfig.ConfigObject.Translator == 2) AppConfig.ConfigObject.Translator = 0;
                
                break;
            case "translator":
                AppConfig.ConfigObject.Translator = Convert.ToInt32((long) d.Value);
                
                if (AppConfig.ConfigObject.Translator == 2) AppConfig.ConfigObject.Recognizer = 2;
                else if (AppConfig.ConfigObject.Recognizer == 2) AppConfig.ConfigObject.Recognizer = 0;
                
                break;
            case "desktop_translation":
                AppConfig.ConfigObject.DesktopTranslation = (bool) d.Value;
                
                break;
            case "quickstart_viewed":
                AppConfig.ConfigObject.QuickstartViewed = (bool) d.Value;
                doNotRestart = true;
                
                break;
            case "groq_api_key":
                AppConfig.ConfigObject.GroqApiKey = (string) d.Value;
                
                break;
            case "gemini_api_key":
                AppConfig.ConfigObject.GeminiApiKey = (string) d.Value;
                
                break;
            case "last_version":
                AppConfig.ConfigObject.LastVersion = (string) d.Value;
                doNotRestart = true;
                
                break;
            default:
                Log.Warning($"[CFG]  Received an unknown field {d.Field} with value {d.Value} while trying to update the config!");
                
                break;
        }
        
        if (!doNotRestart) manager.RestartIfRunning();

        return "";
    }
}
