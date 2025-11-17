using System;
using System.Security.Cryptography;
using System.Text;

namespace TestApp.Comman
{
    public static class Helper
    {
        public static bool bIsValidUser = false;
        public static bool IsAdmin = false;
        public static string sGuid = string.Empty;
        
        public static bool IsValidUser(string sUsername)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(sUsername) && Convert.ToBoolean(sUsername);
            }
            catch
            {
                return false;
            }
        }
        
        public static string HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
                return string.Empty;
                
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password + "MicroFinSalt"));
                return Convert.ToBase64String(bytes);
            }
        }
        
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(enteredPassword) || string.IsNullOrEmpty(storedHash))
                return false;
                
            string hashOfInput = HashPassword(enteredPassword);
            return hashOfInput == storedHash;
        }
    }
}