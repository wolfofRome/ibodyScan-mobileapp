﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Tls
{
    public sealed class DigitallySigned
    {
        private readonly SignatureAndHashAlgorithm algorithm;
        private readonly byte[] signature;

        public DigitallySigned(SignatureAndHashAlgorithm algorithm, byte[] signature)
        {
            if (signature == null)
                throw new ArgumentNullException("signature");

            this.algorithm = algorithm;
            this.signature = signature;
        }

        /// <returns>a <see cref="SignatureAndHashAlgorithm"/> (or null before TLS 1.2).</returns>
        public SignatureAndHashAlgorithm Algorithm
        {
            get { return algorithm; }
        }

        public byte[] Signature
        {
            get { return signature; }
        }

        /// <summary>Encode this <see cref="DigitallySigned"/> to a <see cref="Stream"/>.</summary>
        /// <param name="output">the <see cref="Stream"/> to encode to.</param>
        /// <exception cref="IOException"/>
        public void Encode(Stream output)
        {
            if (algorithm != null)
            {
                algorithm.Encode(output);
            }
            TlsUtilities.WriteOpaque16(signature, output);
        }

        /// <summary>Parse a <see cref="DigitallySigned"/> from a <see cref="Stream"/>.</summary>
        /// <param name="context">the <see cref="TlsContext"/> of the current connection.</param>
        /// <param name="input">the <see cref="Stream"/> to parse from.</param>
        /// <returns>a <see cref="DigitallySigned"/> object.</returns>
        /// <exception cref="IOException"/>
        public static DigitallySigned Parse(TlsContext context, Stream input)
        {
            SignatureAndHashAlgorithm algorithm = null;
            if (TlsUtilities.IsTlsV12(context))
            {
                algorithm = SignatureAndHashAlgorithm.Parse(input);

                if (SignatureAlgorithm.anonymous == algorithm.Signature)
                    throw new TlsFatalAlert(AlertDescription.illegal_parameter);
            }
            byte[] signature = TlsUtilities.ReadOpaque16(input);
            return new DigitallySigned(algorithm, signature);
        }
    }
}
#pragma warning restore
#endif
