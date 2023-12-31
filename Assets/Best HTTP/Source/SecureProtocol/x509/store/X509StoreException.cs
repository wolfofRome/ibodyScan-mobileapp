﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.X509.Store
{
#if !(NETCF_1_0 || NETCF_2_0 || SILVERLIGHT || PORTABLE || NETFX_CORE)
    [Serializable]
#endif
    public class X509StoreException
		: Exception
	{
		public X509StoreException()
		{
		}

		public X509StoreException(
			string message)
			: base(message)
		{
		}

		public X509StoreException(
			string		message,
			Exception	e)
			: base(message, e)
		{
		}
	}
}
#pragma warning restore
#endif
