﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Math.EC.Rfc8032;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.IO;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters
{
    public sealed class Ed448PublicKeyParameters
        : AsymmetricKeyParameter
    {
        public static readonly int KeySize = Ed448.PublicKeySize;

        private readonly byte[] data = new byte[KeySize];

        public Ed448PublicKeyParameters(byte[] buf)
            : this(Validate(buf), 0)
        {
        }

        public Ed448PublicKeyParameters(byte[] buf, int off)
            : base(false)
        {
            Array.Copy(buf, off, data, 0, KeySize);
        }

        public Ed448PublicKeyParameters(Stream input)
            : base(false)
        {
            if (KeySize != Streams.ReadFully(input, data))
                throw new EndOfStreamException("EOF encountered in middle of Ed448 public key");
        }

        public void Encode(byte[] buf, int off)
        {
            Array.Copy(data, 0, buf, off, KeySize);
        }

        public byte[] GetEncoded()
        {
            return Arrays.Clone(data);
        }

        private static byte[] Validate(byte[] buf)
        {
            if (buf.Length != KeySize)
                throw new ArgumentException("must have length " + KeySize, "buf");

            return buf;
        }
    }
}
#pragma warning restore
#endif
