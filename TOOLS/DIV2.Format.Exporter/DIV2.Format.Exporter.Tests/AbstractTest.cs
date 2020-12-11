using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
        public void Log(string message)
        {
            this.TestContext.WriteLine(message);
        }

        public string GetAssetPath(string filename)
        {
            return Path.Combine(SharedConstants.ASSET_FOLDERNAME, filename);
        }

        public string GetOutputPath(string filename)
        {
            return Path.Combine(SharedConstants.OUTPUT_FOLDERNAME, filename);
        }
        #endregion
    }
}
