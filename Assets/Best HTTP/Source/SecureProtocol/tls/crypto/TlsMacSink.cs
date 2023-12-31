﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities.IO;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Tls.Crypto
{
    public class TlsMacSink
        : BaseOutputStream
    {
        private readonly TlsMac m_mac;

        public TlsMacSink(TlsMac mac)
        {
            this.m_mac = mac;
        }

        public virtual TlsMac Mac
        {
            get { return m_mac; }
        }

        public override void WriteByte(byte b)
        {
            m_mac.Update(new byte[]{ b }, 0, 1);
        }

        public override void Write(byte[] buf, int off, int len)
        {
            if (len > 0)
            {
                m_mac.Update(buf, off, len);
            }
        }
    }
}
#pragma warning restore
#endif
