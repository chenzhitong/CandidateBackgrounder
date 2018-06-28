using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace CandidateBackgrounder.Models
{
    public class CandidateViewModels
    {
        public string PublicKey { get; set; }

        public int Votes { get; set; }

        public Candidate Info { get; set; }

        public bool Active { get; set; }

        static public CandidateViewModels FromJson(JObject obj)
        {
            return new CandidateViewModels()
            {
                PublicKey = (string)obj["publickey"],
                Votes = Convert.ToInt32(obj["votes"].ToString().Trim('"')),
                Active = (bool)obj["active"]
            };
        }

        public NodeState State { get; set; }
    }

    public enum NodeState
    {
        Online,
        Offline,
        Unknown
    }
}
