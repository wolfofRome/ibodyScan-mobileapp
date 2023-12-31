﻿#if !BESTHTTP_DISABLE_ALTERNATE_SSL && (!UNITY_WEBGL || UNITY_EDITOR)
#pragma warning disable
using System;

namespace BestHTTP.SecureProtocol.Org.BouncyCastle.Math.Raw
{
    internal abstract class Bits
    {
        internal static uint BitPermuteStep(uint x, uint m, int s)
        {
            uint t = (x ^ (x >> s)) & m;
            return (t ^ (t << s)) ^ x;
        }

        internal static ulong BitPermuteStep(ulong x, ulong m, int s)
        {
            ulong t = (x ^ (x >> s)) & m;
            return (t ^ (t << s)) ^ x;
        }

        internal static uint BitPermuteStepSimple(uint x, uint m, int s)
        {
            return ((x & m) << s) | ((x >> s) & m);
        }

        internal static ulong BitPermuteStepSimple(ulong x, ulong m, int s)
        {
            return ((x & m) << s) | ((x >> s) & m);
        }
    }
}
#pragma warning restore
#endif
