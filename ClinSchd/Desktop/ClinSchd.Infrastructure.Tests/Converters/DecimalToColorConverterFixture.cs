using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Converters;

namespace ClinSchd.Infrastructure.Tests.Converters
{
    [TestClass]
    public class DecimalToColorConverterFixture
    {
        [TestMethod]
        public void ShouldConvertFromDecimalToColorString()
        {
            DecimalToColorConverter converter = new DecimalToColorConverter();

            var convertedValue = converter.Convert(20m, null, null, null) as string;
            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("#ff00cc00", convertedValue);

            convertedValue = converter.Convert(-20m, null, null, null) as string;
            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("#ffff0000", convertedValue);
        }

        [TestMethod]
        public void ShouldReturnNullIfValueToConvertIsNullOrNotDecimal()
        {
            DecimalToColorConverter converter = new DecimalToColorConverter();

            var convertedValue = converter.Convert(null, null, null, null) as string;
            Assert.IsNull(convertedValue);

            convertedValue = converter.Convert("NotADecimal", null, null, null) as string;
            Assert.IsNull(convertedValue);
        }
    }
}
