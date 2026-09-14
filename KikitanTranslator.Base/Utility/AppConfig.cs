using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Serilog;

namespace KikitanTranslator.Utility;

public class ConfigObject : INotifyPropertyChanged
{
    [JsonProperty("quickstart_viewed")] private bool _quickstartViewed;

    [JsonIgnore]
    public bool QuickstartViewed
    {
        get => _quickstartViewed;
        set
        {
            if (_quickstartViewed != value)
            {
                _quickstartViewed = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("language")] private string _language = "en";

    [JsonIgnore]
    public string Language
    {
        get => _language;
        set
        {
            if (_language != value)
            {
                _language = value;
                OnPropertyChanged();
            }
        }
    }
    [JsonProperty("last_version")] private string _lastVersion = "en";

    [JsonIgnore]
    public string LastVersion
    {
        get => _lastVersion;
        set
        {
            if (_lastVersion != value)
            {
                _lastVersion = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("source_language")] private string _sourceLanguage = "en";

    [JsonIgnore]
    public string SourceLanguage
    {
        get => _sourceLanguage;
        set
        {
            if (_sourceLanguage != value)
            {
                _sourceLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("target_language")] private string _targetLanguage = "ja";

    [JsonIgnore]
    public string TargetLanguage
    {
        get => _targetLanguage;
        set
        {
            if (_targetLanguage != value)
            {
                _targetLanguage = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("light_mode")] private bool _lightMode = false;

    [JsonIgnore]
    public bool LightMode
    {
        get => _lightMode;
        set
        {
            if (_lightMode != value)
            {
                _lightMode = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("speech_to_text_only")] private bool _speechToTextOnly = false;

    [JsonIgnore]
    public bool SpeechToTextOnly
    {
        get => _speechToTextOnly;
        set
        {
            if (_speechToTextOnly != value)
            {
                _speechToTextOnly = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("microphone")] private string _microphone = string.Empty;

    [JsonIgnore]
    public string Microphone
    {
        get => _microphone;
        set
        {
            if (_microphone != value)
            {
                _microphone = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("translation_only")] private bool _translationOnly = false;

    [JsonIgnore]
    public bool TranslationOnly
    {
        get => _translationOnly;
        set
        {
            if (_translationOnly != value)
            {
                _translationOnly = value;
                OnPropertyChanged();
            }
        }
    }

    // Controls how Japanese text is shown in the VRChat chatbox.
    // 0 = original, 1 = hiragana, 2 = furigana per kanji, 3 = furigana per word.
    [JsonProperty("japanese_reading_mode")] private int _japaneseReadingMode;

    [JsonIgnore]
    public int JapaneseReadingMode
    {
        get => _japaneseReadingMode;
        set
        {
            if (_japaneseReadingMode != value)
            {
                _japaneseReadingMode = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("chatbox_separate_lines")] private bool _chatboxSeparateLines;

    [JsonIgnore]
    public bool ChatboxSeparateLines
    {
        get => _chatboxSeparateLines;
        set
        {
            if (_chatboxSeparateLines != value)
            {
                _chatboxSeparateLines = value;
                OnPropertyChanged();
            }
        }
    }

    // 0 = translation first, 1 = original first.
    [JsonProperty("chatbox_order")] private int _chatboxOrder;

    [JsonIgnore]
    public int ChatboxOrder
    {
        get => _chatboxOrder;
        set
        {
            if (_chatboxOrder != value)
            {
                _chatboxOrder = value;
                OnPropertyChanged();
            }
        }
    }

    // Controls how Chinese is shown in the VRChat chatbox.
    // 0 = hanzi only, 1 = pinyin only, 2 = hanzi with pinyin after each character.
    [JsonProperty("chinese_reading_mode")] private int _chineseReadingMode;

    [JsonIgnore]
    public int ChineseReadingMode
    {
        get => _chineseReadingMode;
        set
        {
            if (_chineseReadingMode != value)
            {
                _chineseReadingMode = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("pinyin_tone_marks")] private bool _pinyinToneMarks = true;

    [JsonIgnore]
    public bool PinyinToneMarks
    {
        get => _pinyinToneMarks;
        set
        {
            if (_pinyinToneMarks != value)
            {
                _pinyinToneMarks = value;
                OnPropertyChanged();
            }
        }
    }

    // Blank lines inserted between the translation and the original.
    [JsonProperty("chatbox_line_gap")] private int _chatboxLineGap;

    [JsonIgnore]
    public int ChatboxLineGap
    {
        get => _chatboxLineGap;
        set
        {
            if (_chatboxLineGap != value)
            {
                _chatboxLineGap = value;
                OnPropertyChanged();
            }
        }
    }

    // Readings on their own line. 0 = off, 1 = list of readings, 2 = the whole line in hiragana.
    [JsonProperty("furigana_line")] private int _furiganaLine;

    [JsonIgnore]
    public int FuriganaLine
    {
        get => _furiganaLine;
        set
        {
            if (_furiganaLine != value)
            {
                _furiganaLine = value;
                OnPropertyChanged();
            }
        }
    }

    // 0 = directly under the Japanese, 1 = at the bottom.
    [JsonProperty("furigana_line_position")] private int _furiganaLinePosition;

    [JsonIgnore]
    public int FuriganaLinePosition
    {
        get => _furiganaLinePosition;
        set
        {
            if (_furiganaLinePosition != value)
            {
                _furiganaLinePosition = value;
                OnPropertyChanged();
            }
        }
    }

    // Keeps the main window above other windows, so it can be used as a heads up display
    // over a game running borderless.
    [JsonProperty("always_on_top")] private bool _alwaysOnTop;

    [JsonIgnore]
    public bool AlwaysOnTop
    {
        get => _alwaysOnTop;
        set
        {
            if (_alwaysOnTop != value)
            {
                _alwaysOnTop = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("disable_when_muted")] private bool _disableWhenMuted = false;

    [JsonIgnore]
    public bool DisableWhenMuted
    {
        get => _disableWhenMuted;
        set
        {
            if (_disableWhenMuted != value)
            {
                _disableWhenMuted = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("chatbox_wait_per_char_ms")] private int _chatboxWaitPerCharMs = 30;

    [JsonIgnore]
    public int ChatboxWaitPerCharMs
    {
        get => _chatboxWaitPerCharMs;
        set
        {
            if (_chatboxWaitPerCharMs != value)
            {
                _chatboxWaitPerCharMs = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("osc_port")] private int _oscPort = 9000;

    [JsonIgnore]
    public int OscPort
    {
        get => _oscPort;
        set
        {
            if (_oscPort != value)
            {
                _oscPort = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("send_to_chatbox")] private bool _sendToChatbox = true;

    [JsonIgnore]
    public bool SendToChatbox
    {
        get => _sendToChatbox;
        set
        {
            if (_sendToChatbox != value)
            {
                _sendToChatbox = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("send_user_data")] private bool _sendUserData = false;

    [JsonIgnore]
    public bool SendUserData
    {
        get => _sendUserData;
        set
        {
            if (_sendUserData != value)
            {
                _sendUserData = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("recognizer")] private int _recognizer = 0;

    [JsonIgnore]
    public int Recognizer
    {
        get => _recognizer;
        set
        {
            if (_recognizer != value)
            {
                _recognizer = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("translator")] private int _translator = 0;

    [JsonIgnore]
    public int Translator
    {
        get => _translator;
        set
        {
            if (_translator != value)
            {
                _translator = value;
                OnPropertyChanged();
            }
        }
    }

    [JsonProperty("desktop_translation")] private bool _desktopTranslation = false;

    [JsonIgnore]
    public bool DesktopTranslation
    {
        get => _desktopTranslation;
        set
        {
            if (_desktopTranslation != value)
            {
                _desktopTranslation = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("send_without_waiting_for_finish")] private bool _sendWithoutWaitingForFinish = false;

    [JsonIgnore]
    public bool SendWithoutWaitingForFinish
    {
        get => _sendWithoutWaitingForFinish;
        set
        {
            if (_sendWithoutWaitingForFinish != value)
            {
                _sendWithoutWaitingForFinish = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("groq_api_key")] private string _groqApiKey = "";

    [JsonIgnore]
    public string GroqApiKey
    {
        get => _groqApiKey;
        set
        {
            if (_groqApiKey != value)
            {
                _groqApiKey = value;
                OnPropertyChanged();
            }
        }
    }
    
    [JsonProperty("gemini_api_key")] private string _geminiApiKey = "";

    [JsonIgnore]
    public string GeminiApiKey
    {
        get => _geminiApiKey;
        set
        {
            if (_geminiApiKey != value)
            {
                _geminiApiKey = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public delegate void OnConfigUpdate();

public static class AppConfig
{
    public static ConfigObject ConfigObject;
    public static event OnConfigUpdate? OnUpdate;
    private static string _currentConfigPath;

    public static string GetAppFolder()
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string appFolder = Path.Combine(basePath, "Kikitan Translator");
        Directory.CreateDirectory(appFolder);

        return appFolder;
    }

    public static void Load() => Load(Path.Join(GetAppFolder(), "config.json"));

    public static void Load(string configPath)
    {
        if (!Path.Exists(configPath))
        {
            Log.Warning("[CFG]  Specified path is nonexistent (perhaps first launch?). Using the default configuration");

            ConfigObject = new ConfigObject();
            ConfigObject.PropertyChanged += OnConfigPropertyChanged;
            _currentConfigPath = configPath;
            
            SaveConfig();

            return;
        }

        try
        {
            ConfigObject? cfg = JsonConvert.DeserializeObject<ConfigObject>(File.ReadAllText(configPath));
            if (cfg == null)
            {
                Log.Error("[CFG]  Deserialization result returned null!");

                return;
            }

            ConfigObject = cfg;
            ConfigObject.PropertyChanged += OnConfigPropertyChanged;
            _currentConfigPath = configPath;
            
            Log.Information($"[CFG]  Loaded from {configPath}");
        }
        catch (Exception e)
        {
            Log.Error($"[CFG]  Error occured while trying to load the config file!: {e}");

            return;
        }
    }
    
    public static void SaveConfig() {
        File.WriteAllText(_currentConfigPath, JsonConvert.SerializeObject(ConfigObject, Formatting.Indented));
        
        Log.Verbose($"[CFG]  Saved to {_currentConfigPath}");
    }

    private static void OnConfigPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        SaveConfig();
        
        OnUpdate?.Invoke();   
    }
}
