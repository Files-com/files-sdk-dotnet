using FilesCom.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace FilesTests
{
    // When a storage URL's signature stops working, as its query states it: the forms of the SDKs' shared test data,
    // and real Azure shared access signatures. A query that repeats a parameter or cannot be read gives no expiry,
    // and the URL itself is never rewritten.
    [TestClass]
    public class SignedUrlTest
    {
        private static readonly TimeSpan Margin = SignedUrl.RenewalMargin;

        [TestMethod]
        public void SharedUrlsExpireWhenTheirSignaturesSay()
        {
            foreach (string url in SharedUrls("substitute_urls"))
            {
                SignedUrl valid = SignedUrl.Of(Substitute(url, DateTime.UtcNow.AddSeconds(60), 3 * 60));
                SignedUrl expired = SignedUrl.Of(Substitute(url, DateTime.UtcNow.AddSeconds(-3 * 60), 60));

                Assert.IsTrue(valid.ExpiresAt.HasValue, url);
                Assert.IsFalse(valid.ExpiresWithin(Margin), url);
                Assert.IsTrue(expired.ExpiresWithin(TimeSpan.Zero), url);
            }
        }

        [TestMethod]
        public void SharedUnreadableUrlsHaveNoKnownExpiry()
        {
            foreach (string url in SharedUrls("error_urls"))
            {
                Assert.IsNull(SignedUrl.Of(url).ExpiresAt, url);
                Assert.IsFalse(SignedUrl.Of(url).ExpiresWithin(TimeSpan.FromDays(3650)), url);
            }
        }

        [TestMethod]
        [DataRow("2026-09-25T12%3A34%3A56Z", "2026-09-25T12:34:56Z", DisplayName = "seconds, colons escaped")]
        [DataRow("2026-09-25T12:34:56Z", "2026-09-25T12:34:56Z", DisplayName = "seconds")]
        [DataRow("2026-09-25T12:34:56.1234567Z", "2026-09-25T12:34:56.1234567Z", DisplayName = "fractional seconds")]
        [DataRow("2026-09-25T12%3A34Z", "2026-09-25T12:34:00Z", DisplayName = "minutes")]
        [DataRow("2026-09-25", "2026-09-25T00:00:00Z", DisplayName = "a date")]
        public void AzureSignaturesExpireAtTheirSignedExpiry(string se, string expected)
        {
            string url = $"https://account.blob.core.windows.net/container/file.bin?sv=2022-11-02&sr=b&sp=r&st=2026-09-24T12%3A00%3A00Z&se={se}&spr=https&sig=c2lnbmF0dXJl%2B%3D";

            Assert.AreEqual(DateTimeOffset.Parse(expected, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal), SignedUrl.Of(url).ExpiresAt);
        }

        [TestMethod]
        [DataRow("https://account.blob.core.windows.net/c/f?sp=r&se=2026-09-25T12%3A34%3A56Z", DisplayName = "se without a signature")]
        [DataRow("https://account.blob.core.windows.net/c/f?sp=r&se=2026-09-25T12%3A34%3A56%2B02%3A00&sig=x", DisplayName = "se not in UTC")]
        [DataRow("https://account.blob.core.windows.net/c/f?se=2026-09-25&se=2026-09-26&sig=x", DisplayName = "se twice")]
        [DataRow("https://s3.amazonaws.com/b/k?X-Amz-Date=20260925T123456Z&X-Amz-Date=20260925T123456Z&X-Amz-Expires=60", DisplayName = "date twice")]
        [DataRow("https://s3.amazonaws.com/b/k?X-Amz-Date=20260925T123456Z&X-Amz-Expires=-60", DisplayName = "negative lifetime")]
        [DataRow("https://s3.amazonaws.com/b/k?X-Amz-Date=20260925T123456Z&X-Amz-Expires=6000000000", DisplayName = "lifetime too long to be real")]
        [DataRow("https://s3.amazonaws.com/b/k?X-Amz-Date=20260925T123456Z", DisplayName = "no lifetime")]
        [DataRow("https://s3.amazonaws.com/b/k?X-Amz-Date=20260925T123456Z&X-Amz-Expires=60&X-Goog-Date=bad&X-Goog-Expires=60", DisplayName = "one of two forms unreadable")]
        [DataRow("https://files.example.test/path", DisplayName = "no query")]
        public void AmbiguousOrUnreadableSignaturesHaveNoKnownExpiry(string url)
        {
            Assert.IsNull(SignedUrl.Of(url).ExpiresAt);
        }

        [TestMethod]
        public void ExpiryIsTheEarlierOfTheNegotiatedOneAndTheSignedOne()
        {
            DateTime signed = new DateTime(2026, 9, 25, 12, 0, 0, DateTimeKind.Utc);
            string url = $"https://files.example.test/part?X-Files-Date={signed:yyyyMMdd'T'HHmmss'Z'}&X-Files-Expires=600";
            var sooner = new DateTimeOffset(signed.AddSeconds(60));
            var later = new DateTimeOffset(signed.AddHours(1));

            Assert.AreEqual(new DateTimeOffset(signed.AddSeconds(600)), SignedUrl.Of(url).ExpiresAt);
            Assert.AreEqual(sooner, SignedUrl.Of(url, sooner).ExpiresAt);
            Assert.AreEqual(new DateTimeOffset(signed.AddSeconds(600)), SignedUrl.Of(url, later).ExpiresAt);
            Assert.AreEqual(later, SignedUrl.Of("https://files.example.test/unsigned", later).ExpiresAt);
        }

        [TestMethod]
        public void ExpiringWithinTheMarginCountsAsExpiring()
        {
            var nearly = SignedUrl.Of("https://files.example.test/part", DateTimeOffset.UtcNow + Margin - TimeSpan.FromSeconds(1));
            var later = SignedUrl.Of("https://files.example.test/part", DateTimeOffset.UtcNow + Margin + TimeSpan.FromSeconds(5));

            Assert.IsTrue(nearly.ExpiresWithin(Margin));
            Assert.IsFalse(nearly.ExpiresWithin(TimeSpan.Zero));
            Assert.IsFalse(later.ExpiresWithin(Margin));
        }

        [TestMethod]
        public void TheUrlIsKeptExactlyAsGiven()
        {
            string url = "https://bucket.s3.amazonaws.com/a%2Fb/c+d~e?X-Amz-Credential=AKIA%2F20260925%2Fus-east-1&X-Amz-Date=20260925T120000Z&X-Amz-Expires=900&X-Amz-Signature=5ig%2bnature%3D&uploadId=up~1";

            Assert.AreSame(url, SignedUrl.Of(url).Text);
        }

        private static string[] SharedUrls(string name)
        {
            string path = Path.GetFullPath("../../../../../shared/url_test_data.json");
            using (JsonDocument data = JsonDocument.Parse(File.ReadAllText(path)))
            {
                return data.RootElement.GetProperty(name).EnumerateArray().Select(url => url.GetString()).ToArray();
            }
        }

        // The shared URLs take the signing time, then the lifetime in seconds, as their first and second %s.
        private static string Substitute(string url, DateTime signed, int seconds)
        {
            string time = signed.ToString("yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture);
            int first = url.IndexOf("%s", StringComparison.Ordinal);
            url = url.Substring(0, first) + time + url.Substring(first + 2);
            int second = url.IndexOf("%s", StringComparison.Ordinal);
            return second < 0 ? url : url.Substring(0, second) + seconds.ToString(CultureInfo.InvariantCulture) + url.Substring(second + 2);
        }
    }
}