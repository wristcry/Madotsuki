using System;
using System.Security.Cryptography;

namespace madotsuki {
    public class random {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz0123456789";

        static readonly RandomNumberGenerator rng = new RNGCryptoServiceProvider();
        static readonly Random r = new Random();

        public static int rand(int max) {
            return r.Next(0, max);
        }

        public static int rand(int min, int max) {
            if (min > max) throw new ArgumentOutOfRangeException();
            return r.Next(min, max);
        }

        public static int rand_s(int max) {
            byte[] data = new byte[sizeof(int)];
            rng.GetBytes(data);
            double u = BitConverter.ToUInt32(data, 0) / (uint.MaxValue + 1.0);
            return (int)Math.Floor((0 + ((double)max - 0) * u));
        }

        public static int rand_s(int min, int max) {
            if (min > max) throw new ArgumentOutOfRangeException();
            byte[] data = new byte[sizeof(int)];
            rng.GetBytes(data);
            double u = BitConverter.ToUInt32(data, 0) / (uint.MaxValue + 1.0);
            return (int)Math.Floor((min + ((double)max - min) * u));
        }

        public static string randstr(int length) {
            char[] c = new char[length];
            for (int i = 0; i < c.Length; i++)
                c[i] = chars[rand(chars.Length)];
            return new string(c);
        }

        public static string randstr_s(int length) {
            char[] c = new char[length];
            for (int i = 0; i < c.Length; i++)
                c[i] = chars[rand_s(chars.Length)];
            return new string(c);
        }
    }
}
