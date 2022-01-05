using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ewallet.Models
{
    public class Photo
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public bool IsMain { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
