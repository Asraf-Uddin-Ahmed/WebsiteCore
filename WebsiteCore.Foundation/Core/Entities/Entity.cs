using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebsiteCore.Foundation.Core.Entities
{
    public abstract class Entity
    {
        public Guid Id { get; set; }
    }
}
