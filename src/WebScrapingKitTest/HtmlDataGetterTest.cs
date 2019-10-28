using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebScrapingKit;

namespace WebScrapingKitTest
{
    [TestClass]
    public class HtmlDataGetterTest
    {
        [TestMethod]
        public void GetDataFromHtmlNormalTest()
        {
            HtmlDataGetter getter = new HtmlDataGetter();
            string[] result = getter.GetDataFromHtml(@"https://slash-mochi.net/", "//h1[@id=\"sitename\"]/a/span");
            Assert.AreEqual("// もちぶろ", result[0]);
        }

        [TestMethod]
        public void GetAttributeFromHtmlNormalTest1()
        {
            HtmlDataGetter getter = new HtmlDataGetter();
            string[] result = getter.GetAttributeFromHtml(@"https://slash-mochi.net/", "//h1[@id=\"sitename\"]/a/span", "itemprop");
            Assert.AreEqual("name about", result[0]);
        }

        [TestMethod]
        public void GetAttributeFromHtmlNormalTest2()
        {
            HtmlDataGetter getter = new HtmlDataGetter();
            string[] result = getter.GetAttributeFromHtml(@"https://slash-mochi.net/", "//h1[@id=\"sitename\"]/a/img", "width");
            Assert.AreEqual("60", result[0]);
        }
    }
}
