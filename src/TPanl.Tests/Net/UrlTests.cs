using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TPanl.Net;

namespace TPanl.Tests.Net
{
    [TestClass]
    public class UrlTests
    {
        private const string canonicalUrl1 = "http://www.wangdera.com:8080/foo?query=string&query=string&query2=value2";
        private const string canonicalUrl2 = "https://www.pluralsight.com/path/to/something/with.ext?a=b&b=c&c=d&c=e";

        [TestMethod]
        public void CanConvertImplicitlyFromString()
        {
            Url url = canonicalUrl1;

            Assert.AreEqual(canonicalUrl1, url.ToString());
        }

        [TestMethod]
        public void CanConvertImplicitlyToString()
        {
            Url url = new Url(canonicalUrl1);
            string url2 = url;

            Assert.AreEqual(url, url2);
        }

        [TestMethod]
        public void EqualsReturnsFalseForNull()
        {
            Assert.IsFalse(new Url(canonicalUrl1).Equals(null)); 
        }

        [TestMethod]
        public void EqualsReturnsTrueForIdenticalUrl()
        {
            var url = new Url(canonicalUrl1);

            Assert.IsTrue(url.Equals(url)); 
        }

        [TestMethod]
        public void EqualsReturnsTrueForEquivalentUrl()
        {
            var url1 = new Url(canonicalUrl1);
            var url2 = new Url(canonicalUrl1);

            Assert.IsTrue(url1.Equals(url2));
            Assert.AreNotSame(url1, url2); 
        }

        [TestMethod]
        public void EqualsReturnsFalseForInequivalentUrl()
        {
            var url1 = new Url(canonicalUrl1);
            var url2 = new Url(canonicalUrl2);

            Assert.IsFalse(url1.Equals(url2));
        }

        [TestMethod]
        public void EqualsReturnsTrueForEquivalentString()
        {
            var url = new Url(canonicalUrl1);

            Assert.IsTrue(url.Equals(canonicalUrl1)); 
        }

        private void CheckQueryStringParsing(Url url, Dictionary<string, List<string>> expectedParameters)
        {
            Assert.AreEqual(expectedParameters.Count, url.QueryParameters.Count());

            foreach (var expectedParameter in expectedParameters)
            {
                var name = expectedParameter.Key;
                var values = expectedParameter.Value;
                Assert.AreEqual(values.Count, url.QueryParameters[name].Count());

                for (int i = 0; i < values.Count; i++)
                {
                    Assert.AreEqual(values[i], url.QueryParameters[name].ElementAt(i));
                }
            }
        }

        [TestMethod]
        public void QueryStringParsesCorrectly()
        {
            CheckQueryStringParsing(canonicalUrl1, 
                new Dictionary<string, List<string>>
                {
                    { "query", new List<string> { "string", "string" } },
                    { "query2", new List<string> { "value2" } }
                });

        }

        [TestMethod]
        public void QueryStringParsesCorrectlyNoValue()
        {
            CheckQueryStringParsing("http://foobar.com/path/to/something?a&b&c",
                new Dictionary<string, List<string>>
                {
                    { "a", new List<string> { "" } },
                    { "b", new List<string> { "" } }, 
                    { "c", new List<string> { "" } },
                }); 
        }

        [TestMethod]
        public void QueryStringParsesCorrectlyNoQuery()
        {
            CheckQueryStringParsing("http://foobar.com/path/to/something?",
                new Dictionary<string, List<string>>
                {
                });

        }

        [TestMethod]
        public void PathParsesCorrectly()
        {
            var url = new Url("https://www.pluralsight.com/path/to/something/with.ext?a=b&b=c&c=d&c=e");
            Assert.AreEqual("/path/to/something/with.ext", url.Path);
        }

        [TestMethod]
        public void PathParsesCorrectlyNoQuery()
        {
            var url = new Url("http://wangdera.com:1234/path/to/something/with.ext");
            Assert.AreEqual("/path/to/something/with.ext", url.Path); 
        }

        [TestMethod]
        public void AbsolutePathAloneParsesCorrectly()
        {
            var url = new Url("/path/to/something/with.ext?a=b&c=d");
            Assert.AreEqual("/path/to/something/with.ext", url.Path); 
        }

    }
}

