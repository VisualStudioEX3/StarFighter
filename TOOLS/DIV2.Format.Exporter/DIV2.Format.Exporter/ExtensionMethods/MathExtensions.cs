namespace DIV2.Format.Exporter.ExtensionMethods
{
    /// <summary>
    /// Math helper extension methods.
    /// </summary>
    public static class MathExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Is this value in range?
        /// </summary>
        /// <param name="value"><see cref="int"/> value instance.</param>
        /// <param name="min">Min value of the range.</param>
        /// <param name="max">Max value of the range.</param>
        /// <returns>Returns true if the value is in range.</returns>
        public static bool IsClamped(this int value, int min, int max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Is this value in range?
        /// </summary>
        /// <param name="value"><see cref="uint"/> value instance.</param>
        /// <param name="min">Min value of the range.</param>
        /// <param name="max">Max value of the range.</param>
        /// <returns>Returns true if the value is in range.</returns>
        public static bool IsClamped(this uint value, uint min, uint max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Is this value in range?
        /// </summary>
        /// <param name="value"><see cref="short"/> value instance.</param>
        /// <param name="min">Min value of the range.</param>
        /// <param name="max">Max value of the range.</param>
        /// <returns>Returns true if the value is in range.</returns>
        public static bool IsClamped(this short value, short min, short max)
        {
            return value >= min && value <= max;
        }

        /// <summary>
        /// Is this value in range?
        /// </summary>
        /// <param name="value"><see cref="byte"/> value instance.</param>
        /// <param name="min">Min value of the range.</param>
        /// <param name="max">Max value of the range.</param>
        /// <returns>Returns true if the value is in range.</returns>
        public static bool IsClamped(this byte value, byte min, byte max)
        {
            return value >= min && value <= max;
        } 
        #endregion
    }
}
