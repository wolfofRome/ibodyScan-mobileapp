﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Text;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Engines;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Generators;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Modes;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Paddings;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Security;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Encoders;

namespace crypto
{
    public class Security
    {
        // USAGE
        //var key = Security.GenerateText(32);
        //var iv = Security.GenerateText(16);
        //var encrypted = Security.Encrypt("MY SECRET", key, iv);
        //var decrypted = Security.Decrypt(encrypted, key, iv);

        /// <summary>
        /// Return a salted hash based on PBKDF2 for the UTF-8 encoding of the argument text.
        /// </summary>
        /// <param name="text">Provided key text</param>
        /// <param name="salt">Base64 encoded string representing the salt</param>
        /// <returns></returns>
        public static string ComputeHash(string text, string salt)
        {
            byte[] data = Encoding.UTF8.GetBytes(text);
            Sha512Digest sha = new Sha512Digest();
            Pkcs5S2ParametersGenerator gen = new Pkcs5S2ParametersGenerator(sha);

            gen.Init(data, Base64.Decode(salt), 2048);

            return Base64.ToBase64String(((KeyParameter)gen.GenerateDerivedParameters(sha.GetDigestSize() * 8)).GetKey());
        }

        public static string Decrypt(string cipherText, string key, string iv)
        {
            IBufferedCipher cipher = CreateCipher(false, key, iv);
            byte[] textAsBytes = cipher.DoFinal(Base64.Decode(cipherText));

            return Encoding.UTF8.GetString(textAsBytes, 0, textAsBytes.Length);
        }

        public static string Encrypt(string plainText, string key, string iv)
        {
            IBufferedCipher cipher = CreateCipher(true, key, iv);

            return Base64.ToBase64String(cipher.DoFinal(Encoding.UTF8.GetBytes(plainText)));
        }

        public static string GenerateText(int size)
        {
            byte[] textAsBytes = new byte[size];
            SecureRandom secureRandom = SecureRandom.GetInstance("SHA256PRNG", true);

            secureRandom.NextBytes(textAsBytes);
            return Base64.ToBase64String(textAsBytes);
        }

        private static IBufferedCipher CreateCipher(bool isEncryption, string key, string iv)
        {
            IBufferedCipher cipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(new RijndaelEngine()), new ISO10126d2Padding());
            KeyParameter keyParam = new KeyParameter(Base64.Decode(key));
            ICipherParameters cipherParams = (null == iv || iv.Length < 1)
                ? (ICipherParameters)keyParam
                : new ParametersWithIV(keyParam, Base64.Decode(iv));
            cipher.Init(isEncryption, cipherParams);
            return cipher;
        }
    }
}
#pragma warning restore
#endif
