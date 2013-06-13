using System;
using System.Globalization;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Converters;

namespace ClinSchd.Infrastructure.Tests.Converters
{
    /// <summary>
    /// Summary description for StringToNullableIntConverterFixture
    /// </summary>
    [TestClass]
    public class StringToNullableNumberConverterFixture
    {

        [TestMethod]
        public void ShouldConvertValidIntFromString()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = "123";

            object result = converter.ConvertBack(source, typeof(int?), null, null);

            Assert.IsInstanceOfType(result, typeof(int));
            Assert.AreEqual<int>(123, (int)result);
        }

        [TestMethod]
        public void ShouldReturnOriginalValueForNonNullableIntFromString()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = "123";

            object result = converter.ConvertBack(source, typeof(int), null, null);

            Assert.AreSame(source, result);
        }

        [TestMethod]
        public void ShouldReturnNullForInvalidInteger()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = "xxx";

            object result = converter.ConvertBack(source, typeof(int?), null, null);

            Assert.IsNull(result);
        }

        [TestMethod]
        public void ShouldConvertValidDecimalFromString()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = 10.05.ToString(CultureInfo.CurrentCulture);
            
            object result = converter.ConvertBack(source, typeof(decimal?), null, null);

            Assert.IsInstanceOfType(result, typeof(decimal));
            Assert.AreEqual<decimal>(10.05M, (decimal)result);
        }

        [TestMethod]
        public void ShouldReturnOriginalValueForNonNullableDecimalTarget()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = 10.05.ToString(CultureInfo.CurrentCulture);

            object result = converter.ConvertBack(source, typeof(decimal), null, null);

            Assert.AreSame(source, result);
        }

        [TestMethod]
        public void ShouldReturnNullForInvalidDecimal()
        {
            StringToNullableNumberConverter converter = new StringToNullableNumberConverter();

            string source = "xxx";

            object result = converter.ConvertBack(source, typeof(decimal?), null, null);

            Assert.IsNull(result);
        }
	

    }
}
