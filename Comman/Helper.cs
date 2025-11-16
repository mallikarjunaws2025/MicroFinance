using System;

namespace TestApp.Comman
{
    public static class Helper
    {
        public static bool bIsValidUser = false;
        public static bool IsAdmin = false;
        public static string sGuid = string.Empty; 
        public static bool IsValidUser(string sUsername)
        {
            bool bResult = false;
            try
            {
                if (!string.IsNullOrEmpty(Convert.ToString(sUsername)))
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                bResult = false;
            }
            return bResult;
        }

        
    }
}