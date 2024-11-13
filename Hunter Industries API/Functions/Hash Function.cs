using System.Security.Cryptography;
using System.Text;

namespace HunterIndustriesAPI.Functions
{
    /// <summary>
    /// </summary>
    public class HashFunction
    {
        /// <summary>
        /// Takes the given value and hashes it.
        /// </summary>
        public string HashString(string value)
        {
            StringBuilder hashedValue = new StringBuilder();
            SHA512 shaHash = SHA512.Create();

            byte[] hashBytes = shaHash.ComputeHash(Encoding.UTF8.GetBytes(value));

            for (int i = 0; i < hashBytes.Length; i++)
            {
                hashedValue.Append(hashBytes[i].ToString("x2"));
            }

            return hashedValue.ToString();
        }
    }
}