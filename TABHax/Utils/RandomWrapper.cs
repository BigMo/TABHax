using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TABHax.Utils
{
    public class RandomWrapper
    {
        public const string ALPHABET_LOWER = "abcdefghijklmnopqrstuvwxyz";
        public const string ALPHABET_UPPER = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        public const string ALPHABET_NUMBERS = "0123456789";
        public const string ALPHABET_ALPHA_NUMERIC = ALPHABET_LOWER + ALPHABET_UPPER + ALPHABET_NUMBERS;

        public static string GenerateString(int length, string alphabet = ALPHABET_ALPHA_NUMERIC)
        {
            if (string.IsNullOrEmpty(alphabet))
                return "";

            StringBuilder b = new StringBuilder();
            for (int i = 0; i < length; i++)
                b.Append(alphabet[UnityEngine.Random.Range(0, alphabet.Length)]);
            return b.ToString();
        }

        public static ulong GenerateSteamID()
        {
            ulong id = 0L;
            var v1 = UnityEngine.Random.Range(0, int.MaxValue);
            var v2 = UnityEngine.Random.Range(0, int.MaxValue);
            id = (ulong)(((long)v1 << 32) | (long)v2);

            return id;
        }
    }
}
