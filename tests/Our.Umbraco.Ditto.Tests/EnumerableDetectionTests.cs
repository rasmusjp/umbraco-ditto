﻿using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Our.Umbraco.Ditto.Tests.Mocks;

namespace Our.Umbraco.Ditto.Tests
{
    [TestFixture]
    public class EnumerableDetectionTests
    {
        public class MyModel
        {
            [MyProcessor]
            public Dictionary<string, string> MyProperty { get; set; }

            public string EnumerableToSingle { get; set; }

            public IEnumerable<string> SingleToEnumerable { get; set; }
        }

        public class MyProcessorAttribute : DittoProcessorAttribute
        {
            public override object ProcessValue()
            {
                return new Dictionary<string, string>
                {
                    { "hello", "world" },
                    { "foo", "bar" }
                };
            }
        }

        [Test]
        public void GenericDictionaryPropertyIsNotDetectedAsCastableEnumerable()
        {
            var content = new PublishedContentMock
            {
                Properties = new[] { new PublishedContentPropertyMock("myProperty", "myValue") }
            };

            var result = content.As<MyModel>();

            Assert.NotNull(result.MyProperty);
            Assert.True(result.MyProperty.Any());
            Assert.AreEqual(result.MyProperty["hello"], "world");
        }

        [Test]
        public void EnumerablesCast()
        {
            var propertyValue = "myVal";

            var content = new PublishedContentMock
            {
                Properties = new[]
                {
                    new PublishedContentPropertyMock("enumerableToSingle",new[] { propertyValue, "myOtherVal" }),
                    new PublishedContentPropertyMock("singleToEnumerable", propertyValue)
                }
            };

            var result = content.As<MyModel>();

            Assert.NotNull(result.EnumerableToSingle);
            Assert.AreEqual(result.EnumerableToSingle, propertyValue);

            Assert.NotNull(result.SingleToEnumerable);
            Assert.IsTrue(result.SingleToEnumerable.Any());
            Assert.AreEqual(result.SingleToEnumerable.First(), propertyValue);
        }
    }
}