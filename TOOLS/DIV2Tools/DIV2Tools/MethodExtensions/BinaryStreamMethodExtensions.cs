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

        /// <summary>
        /// Avance the write cursor n positions.
        /// </summary>
        /// <param name="stream">This <see cref="BinaryWriter"/> instance.</param>
        /// <param name="bytes">Number of <see cref="byte"/>s to move the cursor.</param>
        public static void AdvanceWritePosition(this BinaryWriter stream, int bytes)
        {
            stream.BaseStream.Position += bytes;
        }

        /// <summary>
        /// Shortcut to get the length of BaseStream.
        /// </summary>
        /// <param name="stream">This <see cref="BinaryReader"/> instance.</param>
        /// <returns>Returns the <see cref="long"/> value of BaseStream.Length.</returns>
        public static long GetLength(this BinaryReader stream)
        {
            return stream.BaseStream.Length;
        }

        /// <summary>
        /// Shortcut to get position of BaseStream.
        /// </summary>
        /// <param name="stream">This <see cref="BinaryReader"/> instance.</param>
        /// <returns>Returns the current position of reading.</returns>
        public static long GetCurrentPosition(this BinaryReader stream)
        {
            return stream.BaseStream.Position;
        }

        /// <summary>
        /// Shortcut to get position of BaseStream.
        /// </summary>
        /// <param name="stream">This <see cref="BinaryWriter"/> instance.</param>
        /// <returns>Returns the current position of writing.</returns>
        public static long GetCurrentPosition(this BinaryWriter stream)
        {
            return stream.BaseStream.Position;
        }

        public static bool EOF(this BinaryReader stream)
        {
            return stream.GetCurrentPosition() >= stream.GetLength();
        }
        #endregion
    }
}
