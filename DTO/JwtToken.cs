using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordManagementSystemClientSide.DTO
{
    public class JwtToken
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
        public int ExpiresIn { get; set; }
    }
}