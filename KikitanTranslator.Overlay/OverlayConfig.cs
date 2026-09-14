using Newtonsoft.Json.Linq;

namespace KikitanTranslator.Overlay;

/// <summary>
/// The overlay runs as its own process and does not reference the main app, so the few
/// appearance settings it needs are read straight from the shared config file. Reads are
/// cheap and happen per caption, which is how a changed setting takes effect immediately.
/// </summary>
public static class OverlayConfig
{
    private const string DefaultFont = "Ink Free";

    private static readonly string Path = System.IO.Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "Kikitan Translator", "config.json");

    public static string Font { get; private set; } = DefaultFont;
    public static int PanelOpacity { get; private set; } = 55;

    public static void Reload()
    {
        try
        {
            if (!File.Exists(Path)) return;

            var config = JObject.Parse(File.ReadAllText(Path));
            var font = (string?)config["overlay_font"];

            Font = string.IsNullOrWhiteSpace(font) ? DefaultFont : font;
            PanelOpacity = Math.Clamp((int?)config["overlay_panel_opacity"] ?? 55, 0, 100);
        }
        catch
        {
            // A missing or half written config is not worth failing a caption over.
        }
    }
}
