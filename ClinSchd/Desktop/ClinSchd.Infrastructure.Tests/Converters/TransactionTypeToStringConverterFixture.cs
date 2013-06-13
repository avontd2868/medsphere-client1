using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Converters;

namespace ClinSchd.Infrastructure.Tests.Converters
{
    [TestClass]
    public class TransactionTypeToStringConverterFixture
    {
        [TestMethod]
        public void ShouldConvertTransactionTypeValuesToString()
        {
            TransactionTypeToStringConverter converter = new TransactionTypeToStringConverter();

            var convertedValue = converter.Convert(TransactionType.Buy, null, null, null) as string;

            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("BUY ", convertedValue);

            convertedValue = converter.Convert(TransactionType.Sell, null, null, null) as string;

            Assert.IsNotNull(convertedValue);
            Assert.AreEqual("SELL ", convertedValue);
        }

        [TestMethod]
        public void ShouldReturnNullIfValueToConvertIsNullOrNotTransactionType()
        {
            TransactionTypeToStringConverter converter = new TransactionTypeToStringConverter();

            var convertedValue = converter.Convert(null, null, null, null) as string;
            Assert.IsNull(convertedValue);

            convertedValue = converter.Convert("NotATransactionType", null, null, null) as string;
            Assert.IsNull(convertedValue);
        }
    }
}
