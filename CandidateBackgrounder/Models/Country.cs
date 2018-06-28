using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateBackgrounder.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ZhName { get; set; }
        public string Code { get; set; }
        public string Code2 { get; set; }
        public bool IsShow { get; set; }
    }
}
