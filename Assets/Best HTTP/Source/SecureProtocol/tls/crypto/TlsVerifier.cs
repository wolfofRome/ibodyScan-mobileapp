﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto
{
    /// <summary>Base interface for a TLS verifier that works with signatures and either raw message digests, or entire
    /// messages.</summary>
    public interface TlsVerifier
    {
        /// <exception cref="IOException"/>
        TlsStreamVerifier GetStreamVerifier(DigitallySigned signature);

        /// <summary>Return true if the passed in signature and hash represent a real signature.</summary>
        /// <param name="signature">the signature object containing the signature to be verified.</param>
        /// <param name="hash">the hash calculated for the signature.</param>
        /// <returns>true if signature verifies, false otherwise.</returns>
        /// <exception cref="IOException">in case of an exception verifying signature.</exception>
        bool VerifyRawSignature(DigitallySigned signature, byte[] hash);
    }
}
#pragma warning restore
#endif
