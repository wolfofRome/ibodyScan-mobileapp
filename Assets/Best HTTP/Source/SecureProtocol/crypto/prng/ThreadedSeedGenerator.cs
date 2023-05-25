﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;
using System.Threading;

#if NO_THREADS || NETFX_CORE
using System.Threading.Tasks;
#endif

using BestHTTP.SecureProtocol.Org.BouncyCastle.Utilities;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Crypto.Prng
{
    /**
     * A thread based seed generator - one source of randomness.
     * <p>
     * Based on an idea from Marcus Lippert.
     * </p>
     */
    public class ThreadedSeedGenerator
    {
        private class SeedGenerator
        {
#if NETCF_1_0
			// No volatile keyword, but all fields implicitly volatile anyway
			private int		counter = 0;
			private bool	stop = false;
#else
            private volatile int counter = 0;
            private volatile bool stop = false;
#endif

            private void Run(object ignored)
            {
                while (!this.stop)
                {
                    this.counter++;
                }
            }

            public byte[] GenerateSeed(
                int numBytes,
                bool fast)
            {
#if SILVERLIGHT || PORTABLE || NETFX_CORE
                return DoGenerateSeed(numBytes, fast);
#else
                ThreadPriority originalPriority = Thread.CurrentThread.Priority;
                try
                {
                    Thread.CurrentThread.Priority = ThreadPriority.Normal;
                    return DoGenerateSeed(numBytes, fast);
                }
                finally
                {
                    Thread.CurrentThread.Priority = originalPriority;
                }
#endif
            }

            private byte[] DoGenerateSeed(
                int numBytes,
                bool fast)
            {
                this.counter = 0;
                this.stop = false;

                byte[] result = new byte[numBytes];
                int last = 0;
                int end = fast ? numBytes : numBytes * 8;

#if NO_THREADS || NETFX_CORE
                Task.Factory.StartNew(() => Run(null), TaskCreationOptions.None);
#else
                ThreadPool.QueueUserWorkItem(new WaitCallback(Run));
#endif

#if PORTABLE || NETFX_CORE
                AutoResetEvent autoResetEvent = new AutoResetEvent(false);
#endif

                try
                {
                    for (int i = 0; i < end; i++)
                    {
                        while (this.counter == last)
                        {
                            try
                            {
#if PORTABLE || NETFX_CORE
                                autoResetEvent.WaitOne(1);
#else
                                Thread.Sleep(1);
#endif
                            }
                            catch (Exception)
                            {
                                // ignore
                            }
                        }

                        last = this.counter;

                        if (fast)
                        {
                            result[i] = (byte)last;
                        }
                        else
                        {
                            int bytepos = i / 8;
                            result[bytepos] = (byte)((result[bytepos] << 1) | (last & 1));
                        }
                    }
                }
                finally
                {
#if PORTABLE || NETFX_CORE
                    autoResetEvent.Dispose();
#endif
                }

                this.stop = true;

                return result;
            }
        }

        /**
         * Generate seed bytes. Set fast to false for best quality.
         * <p>
         * If fast is set to true, the code should be round about 8 times faster when
         * generating a long sequence of random bytes. 20 bytes of random values using
         * the fast mode take less than half a second on a Nokia e70. If fast is set to false,
         * it takes round about 2500 ms.
         * </p>
         * @param numBytes the number of bytes to generate
         * @param fast true if fast mode should be used
         */
        public byte[] GenerateSeed(
            int numBytes,
            bool fast)
        {
            return new SeedGenerator().GenerateSeed(numBytes, fast);
        }
    }
}
#pragma warning restore
#endif
