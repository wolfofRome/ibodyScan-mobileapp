﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System.Collections;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.EdEC;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X9;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Agreement;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Agreement.Kdf;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Digests;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Security
{
	/// <remarks>
	///  Utility class for creating IBasicAgreement objects from their names/Oids
	/// </remarks>
	public sealed class AgreementUtilities
	{
		private AgreementUtilities()
		{
		}

		private static readonly IDictionary algorithms = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateHashtable();
        //private static readonly IDictionary oids = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.CreateHashtable();

        static AgreementUtilities()
		{
            algorithms[X9ObjectIdentifiers.DHSinglePassCofactorDHSha1KdfScheme.Id] = "ECCDHWITHSHA1KDF";
			algorithms[X9ObjectIdentifiers.DHSinglePassStdDHSha1KdfScheme.Id] = "ECDHWITHSHA1KDF";
			algorithms[X9ObjectIdentifiers.MqvSinglePassSha1KdfScheme.Id] = "ECMQVWITHSHA1KDF";

            algorithms[EdECObjectIdentifiers.id_X25519.Id] = "X25519";
            algorithms[EdECObjectIdentifiers.id_X448.Id] = "X448";
        }

        public static IBasicAgreement GetBasicAgreement(
			DerObjectIdentifier oid)
		{
			return GetBasicAgreement(oid.Id);
		}

		public static IBasicAgreement GetBasicAgreement(
			string algorithm)
		{
            string mechanism = GetMechanism(algorithm);

            if (mechanism == "DH" || mechanism == "DIFFIEHELLMAN")
				return new DHBasicAgreement();

			if (mechanism == "ECDH")
				return new ECDHBasicAgreement();

            if (mechanism == "ECDHC" || mechanism == "ECCDH")
                    return new ECDHCBasicAgreement();

			if (mechanism == "ECMQV")
				return new ECMqvBasicAgreement();

			throw new SecurityUtilityException("Basic Agreement " + algorithm + " not recognised.");
		}

		public static IBasicAgreement GetBasicAgreementWithKdf(
			DerObjectIdentifier oid,
			string				wrapAlgorithm)
		{
			return GetBasicAgreementWithKdf(oid.Id, wrapAlgorithm);
		}

		public static IBasicAgreement GetBasicAgreementWithKdf(
			string agreeAlgorithm,
			string wrapAlgorithm)
		{
            string mechanism = GetMechanism(agreeAlgorithm);

            // 'DHWITHSHA1KDF' retained for backward compatibility
			if (mechanism == "DHWITHSHA1KDF" || mechanism == "ECDHWITHSHA1KDF")
				return new ECDHWithKdfBasicAgreement(
					wrapAlgorithm,
					new ECDHKekGenerator(
						new Sha1Digest()));

			if (mechanism == "ECMQVWITHSHA1KDF")
				return new ECMqvWithKdfBasicAgreement(
					wrapAlgorithm,
					new ECDHKekGenerator(
						new Sha1Digest()));

			throw new SecurityUtilityException("Basic Agreement (with KDF) " + agreeAlgorithm + " not recognised.");
		}

        public static IRawAgreement GetRawAgreement(
            DerObjectIdentifier oid)
        {
            return GetRawAgreement(oid.Id);
        }

        public static IRawAgreement GetRawAgreement(
            string algorithm)
        {
            string mechanism = GetMechanism(algorithm);

            if (mechanism == "X25519")
                return new X25519Agreement();

            if (mechanism == "X448")
                return new X448Agreement();

            throw new SecurityUtilityException("Raw Agreement " + algorithm + " not recognised.");
        }

		public static string GetAlgorithmName(
			DerObjectIdentifier oid)
		{
			return (string)algorithms[oid.Id];
		}

        private static string GetMechanism(string algorithm)
        {
            string upper = BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.Platform.ToUpperInvariant(algorithm);
            string mechanism = (string)algorithms[upper];
            return mechanism == null ? upper : mechanism;
        }
	}
}
#pragma warning restore
#endif
