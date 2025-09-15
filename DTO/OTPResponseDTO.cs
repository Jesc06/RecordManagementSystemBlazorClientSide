using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RecordManagementSystemClientSide.DTO
{
    public class OTPResponseDTO
    {
           public string SessionId { get; set; }
           public DateTime ExpiryTime { get; set; }
    }
}