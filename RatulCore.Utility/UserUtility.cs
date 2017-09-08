using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RatulCore.Utility
{
    public class UserUtility
    {
        public static string GetNewPassword(int size, bool hasLetter, bool hasDigit, bool hasSpecialCharacter)
        {
            if (!hasLetter && !hasDigit && !hasSpecialCharacter)
                throw new ArgumentException("All arguments are false.");

            string password = "";
            Random rand = new Random((int)DateTime.Now.Ticks);
            while (password.Length < size)
            {
                char ch = (char)rand.Next(33, 126);
                if (hasLetter && char.IsLetter(ch))
                    password += ch;
                else if (hasDigit && char.IsDigit(ch))
                    password += ch;
                else if (hasSpecialCharacter && !char.IsLetterOrDigit(ch))
                    password += ch;
            }
            return password;
        }
        public static string GetNewVerificationCode()
        {
            string verificationCode = GuidUtility.GetNewSequentialGuid().ToString().Replace("-", "");
            return verificationCode;
        }
    }
}
