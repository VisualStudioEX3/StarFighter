namespace DIV2.Format.Exporter.MethodExtensions
{
    public static class ByteExtensions
    {
        public static byte ClearBits(this byte value, byte mask)
        {
            int i = value; // .NET bit operations works in Int32 values. A conversion is needed to work with bytes.
            return (byte)(i & mask);
        }
    }
}
