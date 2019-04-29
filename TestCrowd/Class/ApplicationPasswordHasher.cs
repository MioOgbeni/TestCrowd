using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.AspNet.Identity;

namespace TestCrowd.Class
{
    public class ApplicationPasswordHasher
    {
        private static PasswordHasher hasher = new PasswordHasher();

        public static string HashPasword(string password)
        {
            return hasher.HashPassword(password);
        }

        public static bool VerifyPassword(string hashedPassword, string originalPassword)
        {
            var result = hasher.VerifyHashedPassword(hashedPassword, originalPassword);
            if (result == PasswordVerificationResult.Success ||
                result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                return true;
            }

            return false;
        }

        public static String HashPasswordOld(String password, String salt)
        {
            var combinedPassword = String.Concat(password, salt);
            var sha256 = new SHA256Managed();
            var bytes = UTF8Encoding.UTF8.GetBytes(combinedPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        public static String GetRandomSaltOld(Int32 size = 12)
        {
            var random = new RNGCryptoServiceProvider();
            var salt = new Byte[size];
            random.GetBytes(salt);
            return Convert.ToBase64String(salt);
        }
    }
}