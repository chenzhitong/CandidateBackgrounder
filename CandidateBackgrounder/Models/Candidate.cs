using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateBackgrounder.Models
{
    public class Candidate
    {
        [Key]
        public string PublicKey { get; set; }
        
        public string Email { get; set; }

        public string IP { get; set; }

        public string Website { get; set; }

        public string Details { get; set; }
        
        public virtual Country Country { get; set; }

        public string SocialAccount { get; set; }

        public string Telegram { get; set; }

        public string Summary { get; set; }
    }
}
