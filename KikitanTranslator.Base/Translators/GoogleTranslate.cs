using System.Net;
using KikitanTranslator.Utility;
using Newtonsoft.Json;
using Serilog;

namespace KikitanTranslator.Base.Translators;

internal class Sentence
{
    [JsonProperty("trans")] public string Translation;
}

internal class Response : IDisposable
{
    [JsonProperty("sentences")] public Sentence[] Sentences;


    public void Dispose()
    {
        
    }
}

public class GoogleTranslate : ITranslator
{
    private CurlImpersonate _curlImpersonate = new();

    // The plain endpoint throttles by IP. Once it starts refusing, every direct attempt
    // costs a wasted round trip, so it is skipped for a while in favour of the fallback.
    private static DateTime _directThrottledUntil = DateTime.MinValue;
    private static readonly TimeSpan ThrottleBackoff = TimeSpan.FromMinutes(2);

    private static string BuildUrl(string text, string source, string target) =>
        $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={source}&tl={target}&dt=t&dt=bd&dj=1&q={Uri.EscapeDataString(text)}";

    public string? Translate(string text, string source, string target)
    {
        if (DateTime.UtcNow < _directThrottledUntil) return TranslateWithCurlImpersonate(text, source, target);

        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BuildUrl(text, source, target));
        request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        HttpWebResponse response;

        try
        {
            response = (HttpWebResponse)request.GetResponse();
        }
        catch (WebException exception)
        {
            // A rejected request throws instead of returning a status, so this is the
            // only place a throttled or blocked endpoint can be caught.
            Log.Warning("[GT]   Direct request failed ({Status}), retrying with curl_impersonate", exception.Status);
            _directThrottledUntil = DateTime.UtcNow + ThrottleBackoff;

            return TranslateWithCurlImpersonate(text, source, target);
        }

        using (response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _directThrottledUntil = DateTime.UtcNow + ThrottleBackoff;

                return TranslateWithCurlImpersonate(text, source, target);
            }

            using(Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            using (Response? resp = JsonConvert.DeserializeObject<Response>(reader.ReadToEnd()))
            {
                if (resp == null)
                {
                    Log.Error($"[GT]   Response deserialization returned null");
                    
                    return null;
                }

                var final = "";
                foreach (var sentence in resp.Sentences)
                {
                    final += $" {Uri.UnescapeDataString(sentence.Translation)}";
                }

                return final.Trim();
            }
        }
    }
    
    
    private string? TranslateWithCurlImpersonate(string text, string source, string target)
    {
        string? resp = _curlImpersonate.DoGet(BuildUrl(text, source, target));
        if (resp == null)
        {
            Log.Error($"[GT]   Google Translate failed (via curl_impersonate)!");

            return null;
        }

        Response? r = JsonConvert.DeserializeObject<Response>(resp);
        if (r == null)
        {
            Log.Error($"[GT]   Response deserialization returned null");
                    
            return null;
        }

        var final = "";
        foreach (var sentence in r.Sentences)
        {
            final += $" {Uri.UnescapeDataString(sentence.Translation)}";
        }

        return final.Trim();
    }
    
    public void Dispose()
    {
        
    }
}