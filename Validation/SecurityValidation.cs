using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TestApp.Validation
{
    public class NoScriptAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            
            string input = value.ToString();
            
            // Check for script tags and common XSS patterns
            string[] dangerousPatterns = {
                @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
                @"javascript:",
                @"vbscript:",
                @"onload=",
                @"onerror=",
                @"onclick=",
                @"onmouseover=",
                @"<iframe",
                @"<object",
                @"<embed"
            };

            foreach (string pattern in dangerousPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field contains potentially unsafe content.";
        }
    }

    public class SafeSqlAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null) return true;
            
            string input = value.ToString();
            
            // Check for SQL injection patterns
            string[] sqlInjectionPatterns = {
                @"(\s|^)(union|select|insert|delete|update|drop|create|alter|exec|execute)(\s|$)",
                @"--",
                @"/\*.*\*/",
                @"'.*'",
                @";.*",
                @"\bor\b.*=.*",
                @"\band\b.*=.*"
            };

            foreach (string pattern in sqlInjectionPatterns)
            {
                if (Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase))
                {
                    return false;
                }
            }

            return true;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} field contains potentially unsafe SQL content.";
        }
    }
}