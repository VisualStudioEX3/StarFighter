using System;
using System.IO;

namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads a 4-byte unsigned integer from the current stream and advances the position of the stream by four bytes.
        /// </summary>
        /// <param name="stream"><see cref="BinaryReader"/> instance.</param>
        /// <param name="bigEndian">Read value as Big Endian?</param>
        /// <returns>A 4-byte unsigned integer read from this stream.</returns>
        /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
        /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
        /// <exception cref="IOException">An I/O error occurs.</exception>
        public static UInt32 ReadUInt32(this BinaryReader stream, bool bigEndian = false)
        {
            if (bigEndian)
            {
                byte[] data = stream.ReadBytes(sizeof(UInt32));
                Array.Reverse(data);
                return BitConverter.ToUInt32(data, 0); 
            }
            else
            {
                return stream.ReadUInt32();
            }
        }

        /// <summary>
        /// Reads the next 4 bytes as <see cref="Int32"/> value and compare with a 4 bytes signature.
        /// </summary>
        /// <param name="stream"><see cref="BinaryReader"/> instance.</param>
        /// <param name="signature">The signature to compare.</param>
        /// <returns>Returns true if the both signatures are equals.</returns>
        public static bool CompareSignatures(this BinaryReader stream, byte[] signature)
        {
            int input = stream.ReadInt32();
            int model = BitConverter.ToInt32(signature, 0);

            return input == model;
        }
    }
}
