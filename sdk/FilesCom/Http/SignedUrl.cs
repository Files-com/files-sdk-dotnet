using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;

namespace FilesCom.Http
{
    // A storage URL that Files.com provided for a transfer, with the time its signature stops working, when that can
    // be told. The URL is only read, never rebuilt: requests send Text exactly as Files.com gave it.
    //
    // The expiry is the earlier of the one Files.com negotiated alongside the URL, if any, and the one its query
    // states. Queries are understood in these forms:
    //
    //   Amazon S3      X-Amz-Date and X-Amz-Expires      a signing time (yyyyMMddTHHmmssZ) and a lifetime in seconds
    //   Files.com      X-Files-Date and X-Files-Expires  the same
    //   Google         X-Goog-Date and X-Goog-Expires    the same
    //   Azure SAS      se, with sig                      an ISO 8601 UTC time, such as 2026-09-25T12:00:00Z
    //
    // and the form sp=se=<yyyyMMddTHHmmssZ> of the SDKs' shared test data. The permissions an Azure signature grants
    // (sp) are not read: nothing here decides what a URL allows. A form that is present but repeats a parameter or
    // has a value that does not parse, or a query that cannot be read as name=value pairs, leaves the expiry unknown,
    // and an unknown expiry is never treated as expired.
    internal sealed class SignedUrl
    {
        // Renew a URL that expires sooner than this before sending to it.
        internal static readonly TimeSpan RenewalMargin = TimeSpan.FromSeconds(30);

        // How many times one part, range or request gets a new URL because storage answered that its URL had expired
        // (RefusedAsExpired) before the transfer fails with that answer.
        internal const int MaxExpiredRenewals = 2;

        // How much of a refusal's body is looked at for why storage refused it. An expiry answer is far smaller.
        private const int RefusalBytes = 4096;

        // What storage services answer, in their XML error, when a signed URL has expired, even though its expiry, as
        // this computer's clock reads it, is still to come: Amazon S3 and the storage compatible with it ("Request has
        // expired", with AccessDenied), Amazon and Google (ExpiredToken), and Azure ("Signature not valid in the
        // specified time frame", with AuthenticationFailed). Matched without regard to case.
        private static readonly string[] ExpiryAnswers =
        {
            "request has expired",
            "<code>expiredtoken</code>",
            "signature not valid in the specified time frame",
        };

        // The types of Files.com's own JSON error, as a transfer URL it serves answers when the URL has expired. Matched
        // exactly.
        private static readonly string[] ExpiryTypes =
        {
            "download_request_expired",
            "upload_request_expired",
        };

        private const char ByteOrderMark = '\uFEFF';

        private const string SigningTimeFormat = "yyyyMMdd'T'HHmmss'Z'";

        private static readonly string[] AzureExpiryFormats =
        {
            "yyyy-MM-dd",
            "yyyy-MM-dd'T'HH:mm'Z'",
            "yyyy-MM-dd'T'HH:mm:ss'Z'",
            "yyyy-MM-dd'T'HH:mm:ss.FFFFFFF'Z'",
        };

        private static readonly (string Date, string Lifetime)[] SigningTimeForms =
        {
            ("X-Amz-Date", "X-Amz-Expires"),
            ("X-Files-Date", "X-Files-Expires"),
            ("X-Goog-Date", "X-Goog-Expires"),
        };

        private SignedUrl(string text, DateTimeOffset? expiresAt)
        {
            Text = text;
            ExpiresAt = expiresAt;
        }

        internal string Text { get; }

        // Whether a storage server's answer says that the signed URL the request was sent to has expired, though the
        // URL's own expiry may still be to come. Only a 400 or 403 can. text is the body as its charset decodes it, as
        // the SDK reads every error answer; body is its bytes, of which whole tells whether they are all of it. A text
        // that starts as JSON does ({, [ or ", after any byte order mark and white space) says so only as exactly one
        // JSON object, the whole body, whose top-level type is one of ExpiryTypes, whatever its other members are: an
        // array, a string, malformed JSON or the start of a longer body never does. Any other body says so when its
        // first RefusalBytes bytes name an expiry (ExpiryAnswers), however long it is. Nothing of the body is kept.
        internal static bool RefusedAsExpired(int status, byte[] body, string text, bool whole)
        {
            if ((status != 400 && status != 403) || body == null || text == null)
            {
                return false;
            }
            if (StartsAsJson(text))
            {
                return whole && IsExpiryObject(text);
            }
            string start = System.Text.Encoding.UTF8.GetString(body, 0, Math.Min(body.Length, RefusalBytes));
            foreach (string answer in ExpiryAnswers)
            {
                if (start.IndexOf(answer, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool StartsAsJson(string text)
        {
            foreach (char c in text)
            {
                if (c == '{' || c == '[' || c == '"')
                {
                    return true;
                }
                if (c != ByteOrderMark && c != ' ' && c != '\t' && c != '\r' && c != '\n')
                {
                    return false;
                }
            }
            return false;
        }

        // Whether text is one JSON object and nothing more, whose type is one of ExpiryTypes.
        private static bool IsExpiryObject(string text)
        {
            try
            {
                using (JsonDocument json = JsonDocument.Parse(text.TrimStart(ByteOrderMark)))
                {
                    JsonElement root = json.RootElement;
                    return root.ValueKind == JsonValueKind.Object && root.TryGetProperty("type", out JsonElement type)
                        && type.ValueKind == JsonValueKind.String && Array.IndexOf(ExpiryTypes, type.GetString()) >= 0;
                }
            }
            catch (JsonException)
            {
                return false;
            }
        }

        // Null when unknown.
        internal DateTimeOffset? ExpiresAt { get; }

        internal static SignedUrl Of(string text, DateTimeOffset? negotiatedExpiry = null)
        {
            DateTimeOffset? stated = QueryExpiry(text);
            DateTimeOffset? earliest = !stated.HasValue || (negotiatedExpiry.HasValue && negotiatedExpiry.Value < stated.Value) ? negotiatedExpiry : stated;
            return new SignedUrl(text, earliest);
        }

        // Whether the URL is known to stop working within time from now.
        internal bool ExpiresWithin(TimeSpan time)
        {
            return ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow + time;
        }

        // The expiry the query states, or null when it states none or cannot be read.
        internal static DateTimeOffset? QueryExpiry(string url)
        {
            Dictionary<string, List<string>> query = url == null ? null : Query(url);
            if (query == null)
            {
                return null;
            }

            var expiries = new List<DateTimeOffset>();
            foreach ((string date, string lifetime) in SigningTimeForms)
            {
                if (!query.ContainsKey(date))
                {
                    continue;
                }
                DateTimeOffset? expiry = SigningTimeExpiry(query, date, lifetime);
                if (!expiry.HasValue)
                {
                    return null;
                }
                expiries.Add(expiry.Value);
            }
            if (query.ContainsKey("se") && query.ContainsKey("sig"))
            {
                DateTimeOffset? expiry = Single(query, "se") is string se && TryParseAzureExpiry(se, out DateTimeOffset parsed) ? parsed : (DateTimeOffset?)null;
                if (!expiry.HasValue)
                {
                    return null;
                }
                expiries.Add(expiry.Value);
            }
            if (query.TryGetValue("sp", out List<string> permissions) && permissions.Exists(value => value.StartsWith("se=", StringComparison.Ordinal)))
            {
                DateTimeOffset? expiry = Single(query, "sp") is string sp && TryParseSigningTime(sp.Substring(3), out DateTimeOffset parsed) ? parsed : (DateTimeOffset?)null;
                if (!expiry.HasValue)
                {
                    return null;
                }
                expiries.Add(expiry.Value);
            }

            DateTimeOffset? earliest = null;
            foreach (DateTimeOffset expiry in expiries)
            {
                if (!earliest.HasValue || expiry < earliest.Value)
                {
                    earliest = expiry;
                }
            }
            return earliest;
        }

        private static DateTimeOffset? SigningTimeExpiry(Dictionary<string, List<string>> query, string date, string lifetime)
        {
            if (!(Single(query, date) is string signed) || !TryParseSigningTime(signed, out DateTimeOffset signedAt))
            {
                return null;
            }
            if (!(Single(query, lifetime) is string seconds) || seconds.Length == 0 || seconds.Length > 9 || !IsDigits(seconds))
            {
                return null;
            }
            return signedAt.AddSeconds(int.Parse(seconds, CultureInfo.InvariantCulture));
        }

        private static bool TryParseSigningTime(string text, out DateTimeOffset time)
        {
            return DateTimeOffset.TryParseExact(text, SigningTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time);
        }

        private static bool TryParseAzureExpiry(string text, out DateTimeOffset time)
        {
            return DateTimeOffset.TryParseExact(text, AzureExpiryFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out time);
        }

        // The parameter's value when it appears exactly once, else null.
        private static string Single(Dictionary<string, List<string>> query, string name)
        {
            return query.TryGetValue(name, out List<string> values) && values.Count == 1 ? values[0] : null;
        }

        // The decoded parameters of the URL's query, by name, or null when a part of it is not a name=value pair.
        private static Dictionary<string, List<string>> Query(string url)
        {
            int start = url.IndexOf('?');
            if (start < 0)
            {
                return new Dictionary<string, List<string>>();
            }
            int end = url.IndexOf('#', start);
            string query = url.Substring(start + 1, (end < 0 ? url.Length : end) - start - 1);

            var parameters = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (string pair in query.Split('&'))
            {
                int equals = pair.IndexOf('=');
                if (equals <= 0)
                {
                    return null;
                }
                string name = Uri.UnescapeDataString(pair.Substring(0, equals));
                string value = Uri.UnescapeDataString(pair.Substring(equals + 1));
                if (!parameters.TryGetValue(name, out List<string> values))
                {
                    parameters[name] = values = new List<string>();
                }
                values.Add(value);
            }
            return parameters;
        }

        private static bool IsDigits(string text)
        {
            foreach (char c in text)
            {
                if (c < '0' || c > '9')
                {
                    return false;
                }
            }
            return true;
        }
    }
}