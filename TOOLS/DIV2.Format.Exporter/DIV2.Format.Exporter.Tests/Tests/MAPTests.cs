using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
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
        const short TEST_WIDTH = 8;
        const short TEST_HEIGHT = 16;
        const int TEST_GRAPH_ID = 256;
        const string TEST_DESCRIPTION = "Test description...";
        #endregion

        #region Intenral vars
        PAL _palette;
        Random _random;
        #endregion

        #region HelperFunctions
        MAP CreateTestMap(out byte[] bitmap)
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);

            bitmap = new byte[map.Count];
            this._random.NextBytes(bitmap);
            map.SetBitmapArray(bitmap);

            return map;
        }

        int CalculateSize(MAP map)
        {
            return (sizeof(short) * 2) + // Width + Height sizes
                    sizeof(int) + // GraphId size
                    MAP.DESCRIPTION_LENGTH + // Description size
                    ColorPalette.SIZE + // ColorPalette size
                    ColorRangeTable.SIZE + // ColorRangeTable size
                    sizeof(short) + // ControlPoints counter size
                    (ControlPoint.SIZE * map.ControlPoints.Count) + // ControlPoints total size
                    (map.Width * map.Height); // Bitmap size
        }
        #endregion

        #region Initializer
        [TestInitialize]
        public void Initialize()
        {
            this.InitializeResultFolder(RESULT_FOLDER_NAME);
            this._palette = new PAL(this.GetAssetPath(SharedConstants.FILENAME_PAL_DIV));
            this._random = new Random();
        }
        #endregion

        #region Test methods
        [TestMethod]
        public void CreateDefaultInstance()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);

            Assert.AreEqual(TEST_WIDTH, map.Width);
            Assert.AreEqual(TEST_HEIGHT, map.Height);
            Assert.AreEqual(MAP.MIN_GRAPH_ID, map.GraphId);
            Assert.IsTrue(string.IsNullOrEmpty(map.Description));
            Assert.IsTrue(map.ControlPoints.Count == 0);
        }

        [TestMethod]
        public void CreateMapWithGraphId()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT, TEST_GRAPH_ID);
            Assert.AreEqual(TEST_GRAPH_ID, map.GraphId);
        }

        [TestMethod]
        public void CreateMapWithDescription()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT, MAP.MIN_GRAPH_ID, TEST_DESCRIPTION);
            Assert.AreEqual(TEST_DESCRIPTION, map.Description);
        }

        [TestMethod]
        public void TryCreateMapWithInvalidSizes()
        {
            try
            {
                new MAP(this._palette, MAP.MIN_PIXEL_SIZE - 1, MAP.MIN_PIXEL_SIZE - 1);
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
                new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT, MAP.MIN_GRAPH_ID - 1);
                Assert.Fail();
            }
            catch
            {
                try
                {
                    new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT, MAP.MAX_GRAPH_ID + 1);
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

        [DataTestMethod]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PCX)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_BMP)]
        [DataRow(SharedConstants.FILENAME_IMG_PLAYER_PNG)]
        public void CreateFromImage(string file)
        {
            var map = MAP.FromImage(this.GetAssetPath(file), this._palette);
            string saveFilename = $"{Path.GetExtension(file)[1..4]}.MAP";
            map.Save(this.GetOutputPath(saveFilename));
        }

        [TestMethod]
        public void AreEqual()
        {
            var a = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);
            var b = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);
            Assert.AreEqual(a, b);
        }

        [TestMethod]
        public void AreNotEqual()
        {
            var a = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);
            var b = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT, MAP.MIN_GRAPH_ID, TEST_DESCRIPTION);
            Assert.AreNotEqual(a, b);
        }

        [TestMethod]
        public void ReadByIndex()
        {
            var map = this.CreateTestMap(out byte[] bitmap);

            for (int i = 0; i < map.Count; i++)
                Assert.AreEqual(bitmap[i], map[i]);
        }

        [TestMethod]
        public void FailReadByIndex()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);

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
            var map = this.CreateTestMap(out byte[] bitmap);

            for (int i = 0; i < map.Count; i++)
                map[i] = bitmap[i];

            for (int i = 0; i < map.Count; i++)
                Assert.AreEqual(bitmap[i], map[i]);
        }

        [TestMethod]
        public void FailWriteByIndex()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);

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
            var map = this.CreateTestMap(out byte[] bitmap);

            int index = 0;
            foreach (var pixel in map)
                Assert.AreEqual(bitmap[index++], pixel);
        }

        [TestMethod]
        public void GetBitmapArray()
        {
            var map = this.CreateTestMap(out byte[] a);
            byte[] b = map.GetBitmapArray();

            Assert.AreEqual(a.Length, b.Length);

            for (int i = 0; i < map.Count; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void SetBitmapArray()
        {
            var b = this.CreateTestMap(out byte[] a);
            for (int i = 0; i < b.Count; i++)
                Assert.AreEqual(a[i], b[i]);
        }

        [TestMethod]
        public void FailSetBitmapArray()
        {
            var map = new MAP(this._palette, TEST_WIDTH, TEST_HEIGHT);

            try
            {
                var buffer = new byte[map.Count - 1];
                map.SetBitmapArray(buffer);
                Assert.Fail();
            }
            catch
            {
            }
        }

        [TestMethod]
        public void Serialize()
        {
            var map = this.CreateTestMap(out _);
            map.GraphId = TEST_GRAPH_ID;
            map.Description = TEST_DESCRIPTION;
            for (int i = 0; i < 32; i++)
                map.ControlPoints.Add(new ControlPoint(this._random.Next(0, ushort.MaxValue),
                                                       this._random.Next(0, ushort.MaxValue)));

            byte[] serialized = map.Serialize();
            int expectedSize = this.CalculateSize(map);

            Assert.AreEqual(expectedSize, serialized.Length);
        }

        [TestMethod]
        public void Write()
        {
            var map = new MAP(this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP));
            using (var stream = new BinaryWriter(new MemoryStream()))
            {
                map.Write(stream);

                int expectedSize = this.CalculateSize(map);
                int streamSize = (stream.BaseStream as MemoryStream).ToArray().Length;

                Assert.AreEqual(expectedSize, streamSize);
            }
        }

        [TestMethod]
        public void Validate()
        {
            string assetPath = this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_MAP);

            Assert.IsTrue(MAP.ValidateFormat(assetPath));

            byte[] buffer = File.ReadAllBytes(assetPath);
            Assert.IsTrue(MAP.ValidateFormat(buffer));
        }

        [TestMethod]
        public void FailValidate()
        {
            string assetPath = this.GetAssetPath(SharedConstants.FILENAME_IMG_PLAYER_BMP);

            Assert.IsFalse(MAP.ValidateFormat(assetPath));

            byte[] buffer = File.ReadAllBytes(assetPath);
            Assert.IsFalse(MAP.ValidateFormat(buffer));
        }

        [TestMethod]
        public void Save()
        {
            string assetPath = this.GetOutputPath("TEST.MAP");
            this.CreateTestMap(out _).Save(assetPath);
            Assert.IsTrue(MAP.ValidateFormat(assetPath));
        }

        [TestMethod]
        public void GetTexture()
        {
            var bitmap = new byte[TEST_WIDTH * TEST_HEIGHT];
            var a = new Color[bitmap.Length];

            this._random.NextBytes(bitmap);

            for (int i = 0; i < a.Length; i++)
                a[i] = this._palette[bitmap[i]].ToRGB();

            var b = this.CreateTestMap(out _).GetRGBTexture();

            Assert.AreEqual(a.Length, b.Length);

            for (int i = 0; i < a.Length; i++)
                Assert.AreEqual(a[i], b[i]);
        }
        #endregion
    }
}
