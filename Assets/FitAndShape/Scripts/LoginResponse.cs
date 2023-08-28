using System;
using UnityEngine;

namespace FitAndShape
{
    [Serializable]
    public sealed class LoginResponse
    {
        [SerializeField] LoginData data;

        public LoginData Data => data;

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}