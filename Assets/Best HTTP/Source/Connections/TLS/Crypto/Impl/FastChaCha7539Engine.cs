﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Utilities;

namespace BestHTTP.Connections.TLS.Crypto.Impl
{
    /// <summary>
    /// Implementation of Daniel J. Bernstein's ChaCha stream cipher.
    /// </summary>
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.NullChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.ArrayBoundsChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppSetOption(BestHTTP.PlatformSupport.IL2CPP.Option.DivideByZeroChecks, false)]
    [BestHTTP.PlatformSupport.IL2CPP.Il2CppEagerStaticClassConstructionAttribute]
    public sealed class FastChaCha7539Engine
        : FastSalsa20Engine
    {
        /// <summary>
        /// Creates a 20 rounds ChaCha engine.
        /// </summary>
        public FastChaCha7539Engine()
            : base()
        {
        }

        public override string AlgorithmName
        {
            get { return "ChaCha7539"; }
        }

        protected override int NonceSize
        {
            get { return 12; }
        }

        protected override void AdvanceCounter()
        {
            if (++engineState[12] == 0)
                throw new InvalidOperationException("attempt to increase counter past 2^32.");
        }

        protected override void ResetCounter()
        {
            engineState[12] = 0;
        }

        protected override void SetKey(byte[] keyBytes, byte[] ivBytes)
        {
            if (keyBytes != null)
            {
                if (keyBytes.Length != 32)
                    throw new ArgumentException(AlgorithmName + " requires 256 bit key");

                PackTauOrSigma(keyBytes.Length, engineState, 0);

                // Key
                Pack.LE_To_UInt32(keyBytes, 0, engineState, 4, 8);
            }

            // IV
            Pack.LE_To_UInt32(ivBytes, 0, engineState, 13, 3);
        }

        protected override void GenerateKeyStream(byte[] output)
        {
            FastChaChaEngine.ChachaCore(rounds, engineState, x);
            Pack.UInt32_To_LE(x, output, 0);
        }
    }
}

#pragma warning restore
#endif
