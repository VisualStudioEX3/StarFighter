using System.IO;

namespace DIV2.Format.Exporter.Processors.Palettes
{
    class DIVPaletteProcessor : IPaletteProcessor
    {
        #region Internal vars
        int _offset;
        #endregion

        #region Properties
        public static DIVPaletteProcessor Instance { get; } = new DIVPaletteProcessor();
        #endregion

        #region Methods & Functions
        public bool CheckFormat(byte[] buffer)
        {
            if (MAP.Instance.CheckHeader(buffer))
            {
                this._offset = MAP.HEADER_LENGTH;
                return true;
            }
            else if (FPG.Instance.CheckHeader(buffer))
            {
                this._offset = FPG.BASE_HEADER_LENGTH;
                return true;
            }

            return false;
        }

        public PAL Process(byte[] buffer)
        {
            using (var file = new BinaryReader(new MemoryStream(buffer)))
            {
                file.BaseStream.Position = this._offset;

                return new PAL(file.ReadBytes(PAL.COLOR_TABLE_LENGTH), file.ReadBytes(PAL.RANGE_TABLE_LENGHT));
            }
        } 
        #endregion
    }
}
