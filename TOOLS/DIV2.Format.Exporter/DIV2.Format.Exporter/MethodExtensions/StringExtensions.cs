using DIV2.Format.Exporter.Cryptographic;
namespace DIV2.Format.Exporter.MethodExtensions
{
    /// <summary>
    /// Method extensions for <see cref="string"/> instances.
    /// </summary>
    public static class StringExtensions
    {
        #region Methods & Functions
        /// <summary>
        /// Returns a secure hash code for this string.
        /// </summary>
        /// <param name="text"><see cref="string"/> instance.</param>
        /// <returns>Returns a <see cref="int"/> hash value.</returns>
        /// <remarks>Use this function instead of default <see cref="string.GetHashCode"/> to unsure that gets a unmutable hash code.</remarks>
        public static int GetSecureHashCode(this string text)
        {
            return HashGenerator.CalculateHashCode(text);
        } 
        #endregion
    }
}
