using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RatulCore.Utility
{
    public class RegexUtility
    {
        private bool _isEmailInvalid = false;

        public bool IsEmailValid(string email)
        {
            _isEmailInvalid = false;
            if (String.IsNullOrEmpty(email))
                return false;

            try
            {
                // Use IdnMapping class to convert Unicode domain names. 
                email = Regex.Replace(email, @"(@)(.+)$", this.DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));

                if (_isEmailInvalid)
                    return false;

                // Return true if email is in valid e-mail format. 
                return Regex.IsMatch(email,
                      @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                _isEmailInvalid = true;
            }
            return match.Groups[1].Value + domainName;
        }
    }
}
