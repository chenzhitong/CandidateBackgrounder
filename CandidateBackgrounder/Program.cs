using CandidateBackgrounder.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CandidateBackgrounder
{
    class Program
    {
        static readonly string RequestURL = (string)JObject.Parse(File.ReadAllText("Config.json"))["RequestURL"];

        static void Main(string[] args)
        {
            while (true)
            {
                Getvalidators();
                GetTxCount();
            }
        }

        static List<Block> blocklist = new List<Block>();
        static List<Block> blockgroup = new List<Block>();
        static readonly int blocklistCount = 3600;
        static readonly int blocklistMaxCount = blocklistCount * 3;

        private static void GetTxCount()
        {
            Console.WriteLine("GetTxCount via Json RPC");
            var currentBlock = GetBlockCount();
            if (blocklist.Count < blocklistCount)
            {
                for (int i = currentBlock - blocklistCount; i < currentBlock; i++)
                {
                    blocklist.Add(GetBlock(i));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"[GetBlock:{i}], Blocklist Count:{blocklist.Count}");
                }
            }
            currentBlock = GetBlockCount();
            for (int i = blocklist[blocklist.Count - 1].Index + 1; i < currentBlock; i++)
            {
                blocklist.Add(GetBlock(i));
                Console.SetCursorPosition(0, Console.CursorTop);
                Console.Write($"[GetBlock:{i}], Blocklist Count:{blocklist.Count}");
            }
            while (blocklist.Count > blocklistMaxCount)
                blocklist.RemoveAt(0);
            Console.WriteLine($"Grouping results in progress");

            int step = 100;
            blockgroup.Clear();
            for (int i = 0; i < blocklist.Count; i += step)
            {
                var groupItem = blocklist.Skip(i).Take(step);
                blockgroup.Add(new Block()
                {
                    Index = groupItem.First().Index,
                    Timestamp = groupItem.First().Timestamp,
                    TxCount = (int)groupItem.Average(p => p.TxCount)
                });
            }

            File.WriteAllText("txcount.json", JsonConvert.SerializeObject(blockgroup));
            Console.WriteLine("Output: txcount.json");
        }

        private static Block GetBlock(int index)
        {
            var response = Helper.PostWebRequest(RequestURL, $"{{'jsonrpc': '2.0', 'method': 'getblock', 'params': [{index},1],  'id': 1}}");
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Please run neo-cli.");
                return null;
            }
            return Block.FromJson(JObject.Parse(response)["result"]);
        }

        private static int GetBlockCount()
        {
            var response = Helper.PostWebRequest(RequestURL, "{'jsonrpc': '2.0', 'method': 'getblockcount', 'params': [],  'id': 1}");
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Please run neo-cli.");
                return 0;
            }
            return (int)JObject.Parse(response)["result"];
        }

        private static void Getvalidators()
        {
            using (var context = new Context())
            {
                Console.WriteLine("GetValidators via Json RPC");
                var response = Helper.PostWebRequest(RequestURL, "{'jsonrpc': '2.0', 'method': 'getvalidators', 'params': [],  'id': 1}");
                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Please run neo-cli.");
                    return;
                }
                var json = JObject.Parse(response)["result"];
                JArray list = (JArray)json;
                var result = new List<CandidateViewModels>();
                foreach (JObject item in list)
                {
                    var c = CandidateViewModels.FromJson(item);
                    c.Info = context.Candidates.FirstOrDefault(p => p.PublicKey == c.PublicKey);
                    if (c.Info != null && c.Info.IP != null)
                    {
                        Console.WriteLine($"Test connect state, IP: {c.Info.IP}");
                        c.State = Helper.IsConnect(c.Info.IP) ? NodeState.Online : NodeState.Offline;
                        c.Info.IP = null;
                    }
                    result.Add(c);
                }
                Console.WriteLine("Output: validators.json");
                File.WriteAllText("validators.json", JsonConvert.SerializeObject(result));
            }
        }
    }
}
