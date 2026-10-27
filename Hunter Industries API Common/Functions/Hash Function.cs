// Copyright © - 11/06/2026 - Toby Hunter
using System.Security.Cryptography;
using System.Text;

namespace HunterIndustriesAPICommon.Functions
{
    public static class HashFunction
    {
        /// <summary>
        /// Takes the given value and hashes it.
        /// </summary>
        public static string HashString(
            string value,
            string salt = null)
        {
            string hashString = null;

            if (!string.IsNullOrWhiteSpace(value))
            {
                string input = string.IsNullOrWhiteSpace(salt) ? value : salt + value;

                StringBuilder hashedValue = new();
                SHA512 shaHash = SHA512.Create();

                byte[] hashBytes = shaHash.ComputeHash(Encoding.UTF8.GetBytes(input));

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    hashedValue.Append(hashBytes[i].ToString("x2"));
                }

                hashString = hashedValue.ToString();
            }

            return hashString;
        }

        /// <summary>
        /// Generates a random salt value.
        /// </summary>
        public static string GenerateSalt()
        {
            byte[] saltBytes = new byte[32];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }

            return Convert.ToBase64String(saltBytes);
        }
    }
}