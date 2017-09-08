using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace RatulCore.Utility.Email
{
    public class MessageSettings
    {
        public ICollection<NameWithEmail> ToList { get; set; }
        public ICollection<NameWithEmail> ReplyToList { get; set; }
        public NameWithEmail From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }

        private void SetMailAddressCollection(MailAddressCollection collection, ICollection<NameWithEmail> listNameWithEmail)
        {
            foreach (NameWithEmail nameWithEmail in listNameWithEmail)
            {
                collection.Add(new MailAddress(nameWithEmail.EmailAddress, nameWithEmail.Name));
            }
        }

        public void SetMailAddressCollectionForToList(MailAddressCollection collection)
        {
            SetMailAddressCollection(collection, ToList);
        }

        public void SetMailAddressCollectionForReplyToList(MailAddressCollection collection)
        {
            SetMailAddressCollection(collection, ReplyToList);
        }
    }
}
