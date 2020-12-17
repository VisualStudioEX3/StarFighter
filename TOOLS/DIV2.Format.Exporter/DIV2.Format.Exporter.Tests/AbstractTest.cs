using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
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
        public string ResultFolder { get; private set; }
        #endregion

        #region Methods & Functions
        public void InitializeResultFolder(string folderName)
        {
            this.ResultFolder = Path.Combine(SharedConstants.BASE_RESULT_FOLDERNAME, folderName);

            if (Directory.Exists(this.ResultFolder))
            {
                IEnumerable<string> files = Directory.EnumerateFiles(this.ResultFolder, "*.*", SearchOption.AllDirectories);
                foreach (var file in files)
                    File.Delete(file);
            }
            else
                Directory.CreateDirectory(this.ResultFolder);
        }

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
            return Path.Combine(SharedConstants.BASE_RESULT_FOLDERNAME, filename);
        }
        #endregion
    }
}
