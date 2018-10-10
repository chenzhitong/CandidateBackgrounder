using System.ComponentModel.DataAnnotations;

namespace CandidateBackgrounder.Models
{
    public class Candidate
    {
        [Key]
        public string PublicKey { get; set; }

        public string Organization { get; set; }

        public string Logo { get; set; }

        public string Email { get; set; }

        public string Website { get; set; }

        public string SocialAccount { get; set; }

        public string Summary { get; set; }

    }
}
