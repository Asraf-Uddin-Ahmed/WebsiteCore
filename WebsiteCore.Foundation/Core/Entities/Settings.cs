using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebsiteCore.Foundation.Core.Enums;

namespace WebsiteCore.Foundation.Core.Entities
{
    public class Settings : Entity
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Value { get; set; }

        public SettingsType Type { get; set; }

    }
}
