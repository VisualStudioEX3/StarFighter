using System;
using System.Collections.Generic;
using System.IO;

namespace DIV2.Format.Exporter.Tests
{
    class SharedConstants
    {
        static readonly string BASE_PATH = $"Assets{Path.DirectorySeparatorChar}";

        public static readonly string FILENAME_SPACE_PAL = SharedConstants.BASE_PATH + "SPACE.PAL";

        public static readonly string FILENAME_PLAYER_PCX = SharedConstants.BASE_PATH + "PLAYER.PCX";
        public static readonly string FILENAME_PLAYER_BMP = SharedConstants.BASE_PATH + "PLAYER.BMP";
        public static readonly string FILENAME_PLAYER_PNG = SharedConstants.BASE_PATH + "PLAYER.PNG";
        public static readonly string FILENAME_PLAYER_MAP = SharedConstants.BASE_PATH + "PLAYER.MAP";
        public static readonly string FILENAME_PLAYER_FPG = SharedConstants.BASE_PATH + "PLAYER.FPG";

        public static string[] IMAGE_FILES = {
            SharedConstants.FILENAME_PLAYER_PCX,
            SharedConstants.FILENAME_PLAYER_BMP,
            SharedConstants.FILENAME_PLAYER_PNG,
            SharedConstants.FILENAME_PLAYER_MAP,
            SharedConstants.FILENAME_PLAYER_FPG
        };

    }
}
