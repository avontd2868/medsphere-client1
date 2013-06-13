using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClinSchd.Infrastructure.Converters;

namespace ClinSchd.Infrastructure.Tests.Converters
{
    [TestClass]
    public class EnumToBooleanConverterFixture
    {
        [TestMethod]
        public void EnumToBooleanConverterConverts()
        {
            EnumToBooleanConverter converter = new EnumToBooleanConverter();
            object value = converter.Convert(TransactionType.Buy, typeof(TransactionType), TransactionType.Buy, null);

            Assert.IsTrue((bool)value);
        }

        [TestMethod]
        public void EnumToBooleanConverterConvertsBack()
        {
            EnumToBooleanConverter converter = new EnumToBooleanConverter();
            object value = converter.ConvertBack(true, typeof(TransactionType), TransactionType.Sell, null);

            Assert.AreEqual(TransactionType.Sell, value);
        }
    }
}
