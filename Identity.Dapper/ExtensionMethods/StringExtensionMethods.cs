using System;

namespace Identity.Dapper.ExtensionMethods
{
    public static class StringExtensionMethods
    {
        /// <summary>
        /// Indicates whether the specified string is null or String.Empty
        /// </summary>
        /// <param name="value">The specified string</param>
        /// <returns>True if the string is null of empty. False otherwise</returns>
        public static bool IsNullOrEmpty(this string value)
        {
            return String.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Indicates whether the specified string is not null or String.Empty
        /// </summary>
        /// <param name="value">The specified string</param>
        /// <returns>True if the string is not null of empty. False otherwise</returns>
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !value.IsNullOrEmpty();
        }
    }
}
