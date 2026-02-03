using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using FilesCom.Util;

namespace FilesTests
{
    [TestClass]
    public class DecimalJsonConverterTest
    {
        private class Payload
        {
            public decimal amount { get; set; }
            public double ratio { get; set; }
        }

        [TestMethod]
        public void SerializesDecimalAsStringAndDoubleAsNumber()
        {
            var payload = new Payload
            {
                amount = 1.23m,
                ratio = 1.23d,
            };

            var json = JsonSerializer.Serialize(payload, JsonUtil.Options);

            StringAssert.Contains(json, "\"amount\":\"1.23\"");
            StringAssert.Contains(json, "\"ratio\":1.23");
        }

        [TestMethod]
        public void DeserializesDecimalFromString()
        {
            var json = "{\"amount\":\"1.23\",\"ratio\":1.23}";
            var payload = JsonSerializer.Deserialize<Payload>(json, JsonUtil.Options);

            Assert.IsNotNull(payload);
            Assert.AreEqual(1.23m, payload.amount);
            Assert.AreEqual(1.23d, payload.ratio);
        }
    }
}
