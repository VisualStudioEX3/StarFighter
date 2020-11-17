using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public abstract class AbstractTest
    {
        #region Properties
        public TestContext TestContext { get; set; }
        public string Class => this.TestContext.FullyQualifiedTestClassName;
        public string Method => this.TestContext.TestName;
        #endregion

        #region Methods & Functions
        protected void Log(string message)
        {
            this.TestContext.WriteLine(message);
        }
        #endregion
    }
}
