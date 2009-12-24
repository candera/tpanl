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
        private const string canonicalUrl1 = "http://www.wangdera.com:8080/foo?query=string#fragment";
        private const string canonicalUrl2 = "https://www.pluralsight.com/";

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
    }
}
