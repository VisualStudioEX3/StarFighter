using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DIV2Tools.MethodExtensions
{
    public static class BinaryReaderMethodExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Avance the read cursor n positions.
        /// </summary>
        /// <param name="stream">This <see cref="BinaryReader"/> instance.</param>
        /// <param name="bytes">Number of <see cref="byte"/>s to move the cursor.</param>
        public static void AdvanceReadPosition(this BinaryReader stream, int bytes)
        {
            stream.BaseStream.Position += bytes;
        } 
        #endregion
    }
}
