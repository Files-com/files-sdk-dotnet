using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel;
using System;
using System.IO;
using System.Text.Json;
using FilesCom;
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
            Assert.IsTrue(PathUtil.same(a, b));
        }
    }
}
