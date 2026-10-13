using FilesCom.Http;
using System;
using System.Text;

namespace FilesCom.Util
{
    // Prepares text from a response or another exception for an exception message or an ordinary log: every URL in it
    // keeps its scheme, host, port and path, and loses the parts where it can carry credentials, which are its user
    // information, its query, where signed URLs keep their signatures, and its fragment. Only URLs, written with a
    // scheme and "://", are recognized; other secrets in the text are not. DEBUG logs keep the original text.
    //
    // The text is read once, from each "://" in turn, so its length alone never makes this slow.
    internal static class DiagnosticText
    {
        private const string Redacted = "[redacted]";
        private static readonly char[] AuthorityEnds = { '/', '?', '#' };
        private static readonly char[] PathEnds = { '?', '#' };
        private static readonly char[] FragmentStart = { '#' };

        // An API request for ordinary logs and messages: its method and the resource its path starts with, such as
        // "PATCH /users", never the rest of the path, which can name a user's files, or its query.
        internal static string Endpoint(System.Net.Http.HttpMethod verb, string path)
        {
            string resource = path.TrimStart('/');
            int end = resource.IndexOfAny(AuthorityEnds);
            return $"{verb} /{(end < 0 ? resource : resource.Substring(0, end))}";
        }

        // Logs that cleanup failed, such as disposing a handler or cancelling operations when a client closes: at WARN
        // only failure, a fixed description, and the types of the exceptions, since what a custom handler or callback
        // throws can quote URLs and credentials; the exception itself at DEBUG.
        internal static void WarnCleanupFailed(SdkLog log, string failure, Exception exception)
        {
            log.Warn($"{failure} ({TypeNames(exception)}); the details are logged at DEBUG.");
            log.Debug(failure, exception);
        }

        // Logs that a request failed with exception, which is thrown on to the caller as it is: at ERROR only failure, a
        // fixed description, the exception's type and its HTTP status, if it has one; the exception at DEBUG, with
        // details, such as the path, as its message when given. A custom handler, factory or stream can throw any
        // exception, the SDK's own types included, and its message or inner exceptions can quote URLs and credentials.
        internal static void ErrorFailed(SdkLog log, string failure, Exception exception, string details = null)
        {
            int? status = RetryPolicy.StatusOf(exception);
            log.Error(status > 0 ? $"{failure} ({TypeNames(exception)}, HTTP {status})" : $"{failure} ({TypeNames(exception)})");
            log.Debug(details ?? failure, exception);
        }

        // The exception's type, and those of the exceptions an AggregateException holds.
        private static string TypeNames(Exception exception)
        {
            if (exception is AggregateException aggregate)
            {
                var inner = new StringBuilder();
                foreach (Exception cause in aggregate.Flatten().InnerExceptions)
                {
                    inner.Append(inner.Length == 0 ? "" : ", ").Append(cause.GetType().Name);
                }
                return $"{nameof(AggregateException)}: {inner}";
            }
            return exception.GetType().Name;
        }

        // The JSON string members of a request or response body whose names say they hold a credential: a password or
        // passphrase, a one-time password, a secret, a token, a private key, an API key or session ID, an access key or
        // SAS token, or a member named key, as an API key's is. Only string values are matched; the name is kept.
        private static readonly System.Text.RegularExpressions.Regex SecretMembers = new System.Text.RegularExpressions.Regex(
            "\"(?<name>(?:[^\"\\\\]*(?:password|passphrase|secret|token|private_key|api_key|session_id|access_key|otp)[^\"\\\\]*)|key)\"\\s*:\\s*\"(?:[^\"\\\\]|\\\\.)*\"",
            System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        // A session's id member, which is the session credential, in the answer to the request that creates a session.
        private static readonly System.Text.RegularExpressions.Regex SessionId = new System.Text.RegularExpressions.Regex(
            "\"(?<name>id)\"\\s*:\\s*\"(?:[^\"\\\\]|\\\\.)*\"",
            System.Text.RegularExpressions.RegexOptions.CultureInvariant);

        // A request or response body for a DEBUG log, with the values of the members that hold credentials replaced by
        // Redacted: the password and one-time password of a sign-in, the key an API key is created with, a secret key
        // given to a remote server, and, when the body answers the request that creates a session, the session's id.
        // Everything else in the body is kept, as DEBUG is where the request and response are recorded in full.
        internal static string WithoutSecretValues(string text, bool createsSession = false)
        {
            if (string.IsNullOrEmpty(text))
            {
                return text;
            }
            string redacted = SecretMembers.Replace(text, "\"${name}\":\"" + Redacted + "\"");
            return createsSession ? SessionId.Replace(redacted, "\"${name}\":\"" + Redacted + "\"") : redacted;
        }

        internal static string WithoutUrlCredentials(string text)
        {
            if (string.IsNullOrEmpty(text) || text.IndexOf("://", StringComparison.Ordinal) < 0)
            {
                return text;
            }
            var result = new StringBuilder(text.Length);
            int copied = 0;
            int search = 0;
            int separator;
            while ((separator = text.IndexOf("://", search, StringComparison.Ordinal)) >= 0)
            {
                int start = SchemeStart(text, separator, copied);
                if (start < 0)
                {
                    search = separator + 3;
                    continue;
                }
                int end = UrlEnd(text, start, separator + 3);
                result.Append(text, copied, start - copied);
                AppendWithoutCredentials(result, text, start, separator + 3, end);
                copied = search = end;
            }
            return result.Append(text, copied, text.Length - copied).ToString();
        }

        // Where the scheme before the "://" at separator starts, not before from: at its first letter, followed by
        // letters, digits, "+", "-" and ".". It is -1 when there is no letter there, and so no URL.
        private static int SchemeStart(string text, int separator, int from)
        {
            int start = separator;
            while (start > from && IsSchemeCharacter(text[start - 1]))
            {
                start--;
            }
            while (start < separator && !IsLetter(text[start]))
            {
                start++;
            }
            return start < separator ? start : -1;
        }

        // Where the URL that starts at start ends: at white space, a double quote or an angle bracket, which a URL
        // cannot hold, or at the scheme of another URL in its path. An apostrophe can be part of a URL, as in its user
        // information or path, so it ends one only when the URL follows an apostrophe: then the last apostrophe that
        // only closing punctuation follows is the closing quote.
        private static int UrlEnd(string text, int start, int bodyStart)
        {
            int end = bodyStart;
            bool inQueryOrFragment = false;
            while (end < text.Length && !IsTerminator(text[end]))
            {
                char c = text[end];
                if (c == '?' || c == '#')
                {
                    inQueryOrFragment = true;
                }
                else if (!inQueryOrFragment && c == ':' && string.CompareOrdinal(text, end, "://", 0, 3) == 0)
                {
                    int nextStart = SchemeStart(text, end, bodyStart);
                    if (nextStart >= 0)
                    {
                        return nextStart;
                    }
                }
                end++;
            }
            if (start > 0 && text[start - 1] == '\'')
            {
                int closing = end;
                while (closing > bodyStart && IsClosingPunctuation(text[closing - 1]))
                {
                    closing--;
                }
                if (closing > bodyStart && text[closing - 1] == '\'')
                {
                    return closing - 1;
                }
            }
            return end;
        }

        // Appends the URL text[start, end) with its user information, query and fragment replaced by Redacted.
        private static void AppendWithoutCredentials(StringBuilder result, string text, int start, int bodyStart, int end)
        {
            int authorityEnd = IndexOfAny(text, bodyStart, end, AuthorityEnds);
            int userInformationEnd = authorityEnd > bodyStart ? text.LastIndexOf('@', authorityEnd - 1, authorityEnd - bodyStart) : -1;
            int pathEnd = IndexOfAny(text, authorityEnd, end, PathEnds);

            result.Append(text, start, bodyStart - start);
            if (userInformationEnd >= 0)
            {
                result.Append(Redacted).Append('@');
            }
            int hostStart = userInformationEnd >= 0 ? userInformationEnd + 1 : bodyStart;
            result.Append(text, hostStart, pathEnd - hostStart);
            if (pathEnd < end && text[pathEnd] == '?')
            {
                result.Append('?').Append(Redacted);
            }
            if (IndexOfAny(text, pathEnd, end, FragmentStart) < end)
            {
                result.Append('#').Append(Redacted);
            }
        }

        // The first of characters in text[from, end), or end when there is none.
        private static int IndexOfAny(string text, int from, int end, char[] characters)
        {
            int index = from < end ? text.IndexOfAny(characters, from, end - from) : -1;
            return index >= 0 ? index : end;
        }

        private static bool IsLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private static bool IsSchemeCharacter(char c)
        {
            return IsLetter(c) || (c >= '0' && c <= '9') || c == '+' || c == '-' || c == '.';
        }

        private static bool IsTerminator(char c)
        {
            return char.IsWhiteSpace(c) || c == '"' || c == '<' || c == '>';
        }

        private static bool IsClosingPunctuation(char c)
        {
            return c == '.' || c == ',' || c == ';' || c == ':' || c == '!' || c == '?' || c == ')' || c == ']' || c == '}';
        }
    }
}