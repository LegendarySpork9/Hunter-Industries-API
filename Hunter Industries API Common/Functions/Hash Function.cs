// Copyright © - Unpublished - Toby Hunter
using System.Security.Cryptography;
using System.Text;

namespace HunterIndustriesAPICommon.Functions
{
    /// <summary>
    /// </summary>
    public static class HashFunction
    {
        /// <summary>
        /// Takes the given value and hashes it.
        /// </summary>
        public static string HashString(string value)
        {
            string hashString = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                StringBuilder hashedValue = new();
                SHA512 shaHash = SHA512.Create();

                byte[] hashBytes = shaHash.ComputeHash(Encoding.UTF8.GetBytes(value));

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashedValue.Append(hashBytes[i].ToString("x2"));
                }

                hashString = hashedValue.ToString();
            }

            return hashString;
        }
    }
}