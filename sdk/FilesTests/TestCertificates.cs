using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace FilesTests
{
    // A certificate authority of a test's own, made when the test runs, and server certificates it issues for DNS names.
    // Clients trust it only through a custom trust store given to their own handler (CertificateChainPolicy), never
    // through the system's trusted roots.
    public sealed class TestCertificateAuthority : IDisposable
    {
        private readonly X509Certificate2 authority;

        public TestCertificateAuthority(string name)
        {
            using (RSA key = RSA.Create(2048))
            {
                var request = new CertificateRequest($"CN={name}", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
                request.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(request.PublicKey, false));
                authority = request.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddDays(2));
            }
#if NET9_0_OR_GREATER
            Certificate = X509CertificateLoader.LoadCertificate(authority.RawData);
#else
            Certificate = new X509Certificate2(authority.RawData);
#endif
        }

        // The authority's certificate, without its key, for a trust store.
        public X509Certificate2 Certificate { get; }

        // A chain policy that trusts this authority alone.
        public X509ChainPolicy TrustPolicy()
        {
            var policy = new X509ChainPolicy { TrustMode = X509ChainTrustMode.CustomRootTrust, RevocationMode = X509RevocationMode.NoCheck };
            policy.CustomTrustStore.Add(Certificate);
            return policy;
        }

        // A server certificate for names, with its key, loaded as SslStream on macOS needs it.
        public X509Certificate2 Issue(params string[] names)
        {
            return Issue(DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow.AddDays(1), names);
        }

        // The same, valid only from notBefore to notAfter, which must be within the authority's own day before and two
        // days after it was made: such as a certificate that has already expired.
        public X509Certificate2 Issue(DateTimeOffset notBefore, DateTimeOffset notAfter, params string[] names)
        {
            using (RSA key = RSA.Create(2048))
            {
                var request = new CertificateRequest($"CN={names[0]}", key, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                var alternativeNames = new SubjectAlternativeNameBuilder();
                foreach (string name in names)
                {
                    alternativeNames.AddDnsName(name);
                }
                request.CertificateExtensions.Add(alternativeNames.Build());
                request.CertificateExtensions.Add(new X509BasicConstraintsExtension(false, false, 0, true));
                request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
                request.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));
                byte[] serial = new byte[16];
                RandomNumberGenerator.Fill(serial);
                serial[0] &= 0x7F;
                using (X509Certificate2 issued = request.Create(authority, notBefore, notAfter, serial))
                using (X509Certificate2 withKey = issued.CopyWithPrivateKey(key))
                {
                    // A password: macOS cannot import a PKCS #12 blob with an empty one.
                    byte[] pfx = withKey.Export(X509ContentType.Pkcs12, "loopback");
#if NET9_0_OR_GREATER
                    return X509CertificateLoader.LoadPkcs12(pfx, "loopback");
#else
                    return new X509Certificate2(pfx, "loopback");
#endif
                }
            }
        }

        public void Dispose()
        {
            authority.Dispose();
            Certificate.Dispose();
        }
    }
}