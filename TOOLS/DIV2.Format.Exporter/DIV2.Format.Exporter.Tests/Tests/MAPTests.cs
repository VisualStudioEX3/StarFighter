using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using DIV2.Format.Exporter.MethodExtensions;
using System;

namespace DIV2.Format.Exporter.Tests
{
    [TestClass]
    public class MAPTests 
        : AbstractTest,
        IDefaultConstructorsTests,
        IEqualityTests,
        ISerializableAssetTests,
        IIterableReadTests,
        IIterableWriteTests,
        IEnumerableTests,
        IFormatValidableTests,
        IAssetFileTests
    {
        #region Constants
        const string RESULT_FOLDER_NAME = "MAP";
        const int TEST_SIZE = 3;
        const int DEFAULT_WIDTH = 320;
        const int DEFAULT_HEIGHT = 200;
        const string DEFAULT_DESCRIPTION = "Test description...";
        #endregion

        #region Intenral vars
        PAL _defaultPalette;
        MAP _defaultMap;
        MAP _defaultLoadedMap;
        #endregion

        #region HelperFunctions
        MAP CreateDefaultMap()
        {
            if (this._defaultMap is null)
                this._defaultMap = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            return this._defaultMap;
        }

        MAP LoadDefaultMap()
        {
            if (this._defaultLoadedMap is null)
                this._defaultLoadedMap = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));

            return this._defaultLoadedMap;
        }
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this._defaultPalette = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var map = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            
            Assert.AreEqual(DEFAULT_WIDTH, map.Width);
            Assert.AreEqual(DEFAULT_HEIGHT, map.Height);
            Assert.AreEqual(MAP.MIN_GRAPH_ID, map.GraphId);
            Assert.IsTrue(string.IsNullOrEmpty(map.Description));
            Assert.IsTrue(map.ControlPoints.Count == 0);
        }

        [TestMethod]
        public void CreateMapWithGraphId()
        {
            const int TEST_GRAPH_ID = 256;
            var map = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT, TEST_GRAPH_ID);
            Assert.AreEqual(TEST_GRAPH_ID, map.GraphId);
        }

        [TestMethod]
        public void CreateMapWithDescription()
        {
            var map = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT, MAP.MIN_GRAPH_ID, DEFAULT_DESCRIPTION);
            Assert.AreEqual(DEFAULT_DESCRIPTION, map.Description);
        }

        [TestMethod]
        public void TryCreateMapWithInvalidSizes()
        {
            try
            {
                new MAP(this._defaultPalette, MAP.MIN_SIZE - 1, MAP.MIN_SIZE - 1);
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void TryCreateMapWithInvalidGraphId()
        {
            try
            {
                new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT, MAP.MIN_GRAPH_ID - 1);
                Assert.Fail();
            }
            catch
            {
                try
                {
                    new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT, MAP.MAX_GRAPH_ID + 1);
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void CreateInstanceFromBuffer()
        {
            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            new MAP(buffer);
        }

        [TestMethod]
        public void CreateInstanceFromFile()
        {
            new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            var b = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);
            var b = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT, MAP.MIN_GRAPH_ID, DEFAULT_DESCRIPTION);
            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var map = new MAP(this._defaultPalette, 3, 3);
            var bitmap = new byte[map.Count];
            new Random().NextBytes(bitmap);
            map.SetBitmapArray(bitmap);

            for (int i = 0; i < map.Count; i++)
                Assert.AreEqual(bitmap[i], map[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            var map = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            try
            {
                _ = map[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = map[PAL.LENGTH + 1];
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void WriteByIndex()
        {
            var map = new MAP(this._defaultPalette, 3, 3);
            var bitmap = new byte[map.Count];
            new Random().NextBytes(bitmap);

            for (int i = 0; i < map.Count; i++)
                map[i] = bitmap[i];

            for (int i = 0; i < map.Count; i++)
                Assert.AreEqual(bitmap[i], map[i]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            var map = new MAP(this._defaultPalette, DEFAULT_WIDTH, DEFAULT_HEIGHT);

            try
            {
                _ = map[-1];
                Assert.Fail();
            }
            catch
            {
                try
                {
                    _ = map[PAL.LENGTH + 1];
                    Assert.Fail();
                }
                catch
                {
                }
            }
        }

        [TestMethod]
        public void ReadByForEach()
        {
            throw new System.NotImplementedException();
        }

        [TestMethod]
        public void Serialize()
        {
            var map = this.LoadDefaultMap();
            byte[] serialized = map.Serialize();
            int expectedSize = (sizeof(short) * 2) +
                                sizeof(int) +
                                MAP.DESCRIPTION_LENGTH +
                                sizeof(short) +
                                (ControlPoint.SIZE * map.ControlPoints.Count) +
                                (map.Width * map.Height);

            Assert.AreEqual(expectedSize, serialized.Length);
        }

        [TestMethod]
        public void Write()
        {
            var map = this.LoadDefaultMap();
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                map.Write(stream);
            }
        }

        [TestMethod]
        public void Validate()
        {
            Assert.IsTrue(MAP.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP)));

            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            Assert.IsTrue(MAP.ValidateFormat(buffer));
        }

        [TestMethod]
        public void FailValidate()
        {
            Assert.IsFalse(MAP.ValidateFormat(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP)));

            byte[] buffer = File.ReadAllBytes(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP));
            Assert.IsFalse(MAP.ValidateFormat(buffer));
        }

        [TestMethod]
        public void Save()
        {

        }
        #endregion
    }
}
