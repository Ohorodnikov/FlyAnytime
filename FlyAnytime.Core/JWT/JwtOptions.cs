using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlyAnytime.Core.JWT
{
    public class JwtOptions
    {
        public const string ISSUER = "MyAuthServer"; // издатель токена
        public const string AUDIENCE = "MyAuthClient"; // потребитель токена
        private const string KEY = "r626/41vXHDvIDA50uZAd2j19VJWW2fg2AmGubovVbxRwY5GGnNFvh/EwAYmMOabs8TjnNhZfC5y/kUlu0PElg==";
        public const int LIFETIME = 1; // время жизни токена - 1 минута
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
