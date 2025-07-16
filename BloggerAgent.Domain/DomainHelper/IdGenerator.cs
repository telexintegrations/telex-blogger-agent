using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BloggerAgent.Domain.DomainHelper
{
    public class IdGenerator
    {
      
        public static string GenerateObjectId()
        {
            byte[] bytes = new byte[12];

            // Add timestamp (first 4 bytes)
            var timestamp = BitConverter.GetBytes((int)DateTimeOffset.UtcNow.ToUnixTimeSeconds());
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timestamp); 

            Array.Copy(timestamp, 0, bytes, 0, 4);

            // Add 5 random bytes (next 5 bytes for machine and process)
            RandomNumberGenerator.Fill(bytes.AsSpan(4, 5));

            // Add counter/random (last 3 bytes)
            RandomNumberGenerator.Fill(bytes.AsSpan(9, 3));

            // Convert to 24-char hex string
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

}

