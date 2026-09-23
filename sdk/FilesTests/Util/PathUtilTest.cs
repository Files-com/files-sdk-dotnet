using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.IO;
using System.Globalization;
using System.Text.Json;
using FilesCom;
using FilesCom.Models;
using FilesCom.Util;

namespace FilesTests.PathUtilTest
{
    [TestClass]
    public class PathUtilTest
    {
        public static IEnumerable<string[]> GetComparisons()
        {
            dynamic source;
            string filePath = System.IO.Path.GetFullPath("../../../../../shared/normalization_for_comparison_test_data.json");
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                source = JsonSerializer.Deserialize<dynamic>(json);
            }

            foreach (var item in source.EnumerateArray())
            {
                yield return new string[] { item[0].GetString(), item[1].GetString() };
            }
        }


        [TestMethod, DynamicData(nameof(GetComparisons), DynamicDataSourceType.Method)]
        public void TestSame(string a, string b)
        {
            Assert.AreEqual(b, PathUtil.normalize_for_comparison(a));
            Assert.AreEqual(b, PathUtil.normalize_for_comparison(b));
            Assert.IsTrue(PathUtil.same(a, b));
        }

        [TestMethod]
        public void TestServerComparisonExamplesAreLocaleIndependent()
        {
            var previous = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
                Assert.AreEqual("ii", PathUtil.normalize_for_comparison("Iİ"));
                string json = File.ReadAllText("../../../../../shared/comparison_examples.json");
                foreach (var pair in JsonSerializer.Deserialize<string[][]>(json))
                {
                    Assert.AreEqual(pair[1], PathUtil.normalize_for_comparison(pair[0]));
                }
            }
            finally
            {
                CultureInfo.CurrentCulture = previous;
            }
        }

        [TestMethod]
        public void TestNormalizePreservesPathIdentity()
        {
            Assert.AreEqual("remote/path/to/file.txt", PathUtil.normalize("/../../remote\\path//./to/file.txt"));
            Assert.AreEqual("remote/path/to/file.txt", PathUtil.normalize("remote/../path/to/file.txt"));
            Assert.AreEqual("_/RemoteServers/42/remote/path/to/file.txt",
                PathUtil.normalize("_", "RemoteServers", "42", "/../../remote\\path//./to/file.txt"));
            Assert.AreEqual("_/RemoteServers/42/file.txt",
                RemoteFile.UnderscoreUploadDestinationPath("RemoteServers", 42, "local/path/to/file.txt"));
        }
    }
}
