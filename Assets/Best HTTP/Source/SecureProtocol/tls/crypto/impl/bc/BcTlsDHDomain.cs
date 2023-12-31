﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.IO;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Agreement;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Generators;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Parameters;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Math;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto.Impl.BC
{
    /// <summary>BC light-weight support class for Diffie-Hellman key pair generation and key agreement over a
    /// specified Diffie-Hellman configuration.</summary>
    public class BcTlsDHDomain
        : TlsDHDomain
    {
        private static byte[] EncodeValue(DHParameters dh, bool padded, BigInteger x)
        {
            return padded
                ? BigIntegers.AsUnsignedByteArray(GetValueLength(dh), x)
                : BigIntegers.AsUnsignedByteArray(x);
        }

        private static int GetValueLength(DHParameters dh)
        {
            return (dh.P.BitLength + 7) / 8;
        }

        public static BcTlsSecret CalculateDHAgreement(BcTlsCrypto crypto, DHPrivateKeyParameters privateKey,
            DHPublicKeyParameters publicKey, bool padded)
        {
            DHBasicAgreement basicAgreement = new DHBasicAgreement();
            basicAgreement.Init(privateKey);
            BigInteger agreementValue = basicAgreement.CalculateAgreement(publicKey);
            byte[] secret = EncodeValue(privateKey.Parameters, padded, agreementValue);
            return crypto.AdoptLocalSecret(secret);
        }

        public static DHParameters GetParameters(TlsDHConfig dhConfig)
        {
            DHGroup dhGroup = TlsDHUtilities.GetDHGroup(dhConfig);
            if (dhGroup == null)
                throw new ArgumentException("No DH configuration provided");

            return new DHParameters(dhGroup.P, dhGroup.G, dhGroup.Q, dhGroup.L);
        }

        protected readonly BcTlsCrypto crypto;
        protected readonly TlsDHConfig dhConfig;
        protected readonly DHParameters dhParameters;

        public BcTlsDHDomain(BcTlsCrypto crypto, TlsDHConfig dhConfig)
        {
            this.crypto = crypto;
            this.dhConfig = dhConfig;
            this.dhParameters = GetParameters(dhConfig);
        }

        public virtual BcTlsSecret CalculateDHAgreement(DHPrivateKeyParameters privateKey,
            DHPublicKeyParameters publicKey)
        {
            return CalculateDHAgreement(crypto, privateKey, publicKey, dhConfig.IsPadded);
        }

        public virtual TlsAgreement CreateDH()
        {
            return new BcTlsDH(this);
        }

        /// <exception cref="IOException"/>
        public virtual BigInteger DecodeParameter(byte[] encoding)
        {
            if (dhConfig.IsPadded && GetValueLength(dhParameters) != encoding.Length)
                throw new TlsFatalAlert(AlertDescription.illegal_parameter);

            return new BigInteger(1, encoding);
        }

        /// <exception cref="IOException"/>
        public virtual DHPublicKeyParameters DecodePublicKey(byte[] encoding)
        {
            /*
             * RFC 7919 3. [..] the client MUST verify that dh_Ys is in the range 1 < dh_Ys < dh_p - 1.
             * If dh_Ys is not in this range, the client MUST terminate the connection with a fatal
             * handshake_failure(40) alert.
             */
            try
            {
                BigInteger y = DecodeParameter(encoding);

                return new DHPublicKeyParameters(y, dhParameters);
            }
            catch (Exception e)
            {
                throw new TlsFatalAlert(AlertDescription.handshake_failure, e);
            }
        }

        /// <exception cref="IOException"/>
        public virtual byte[] EncodeParameter(BigInteger x)
        {
            return EncodeValue(dhParameters, dhConfig.IsPadded, x);
        }

        /// <exception cref="IOException"/>
        public virtual byte[] EncodePublicKey(DHPublicKeyParameters publicKey)
        {
            return EncodeValue(dhParameters, true, publicKey.Y);
        }

        public virtual AsymmetricCipherKeyPair GenerateKeyPair()
        {
            DHBasicKeyPairGenerator keyPairGenerator = new DHBasicKeyPairGenerator();
            keyPairGenerator.Init(new DHKeyGenerationParameters(crypto.SecureRandom, dhParameters));
            return keyPairGenerator.GenerateKeyPair();
        }
    }
}
#pragma warning restore
#endif
