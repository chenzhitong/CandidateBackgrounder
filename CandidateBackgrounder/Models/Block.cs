using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateBackgrounder.Models
{
    public class Block
    {
        public int Index;

        public int Timestamp;

        public int Size;

        public int TxCount;

        public static Block FromJson(JToken json)
        {
            var block = new Block()
            {
                Timestamp = (int)json["time"],
                Index = (int)json["index"],
                TxCount = json["tx"].Count(),
                Size = (int)json["size"],
            };
            return block;
        }
    }

}
