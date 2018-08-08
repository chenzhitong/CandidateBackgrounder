using CandidateBackgrounder.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace CandidateBackgrounder
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Getvalidators();
                GetTxCount();
                Thread.Sleep(1000);
            }
        }

        static List<Block> blocklist = new List<Block>();
        static List<Block> blockgroup = new List<Block>();
        static readonly int blocklistCount = 3600;

        private static void GetTxCount()
        {
            var currentBlock = Helper.GetBlockCount();
            //Initializing data
            if (blocklist.Count < blocklistCount)
            {
                for (int i = currentBlock - blocklistCount; i < currentBlock; i++)
                {
                    blocklist.Add(Helper.GetBlock(i));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"[GetBlock:{i}], Blocklist Count:{blocklist.Count}");
                }
            }
            else
            {
                for (int i = blocklist[blocklist.Count - 1].Index + 1; i < currentBlock; i++)
                {
                    blocklist.Add(Helper.GetBlock(i));
                    Console.SetCursorPosition(0, Console.CursorTop);
                    Console.Write($"[GetBlock:{i}], Blocklist Count:{blocklist.Count}");
                }
                while (blocklist.Count > blocklistCount)
                    blocklist.RemoveAt(0);
            }
            Console.WriteLine();

            int step = 100;
            blockgroup.Clear();
            for (int i = 0; i < blocklist.Count; i += step)
            {
                var groupItem = blocklist.Skip(i).Take(step);
                blockgroup.Add(new Block()
                {
                    Index = groupItem.First().Index,
                    Timestamp = groupItem.First().Timestamp,
                    TxCount = (int)groupItem.Average(p => p.TxCount),
                    Size = (int)groupItem.Average(p => p.Size),
                });
            }
            var result = new BlockViewModels();
            foreach (var item in blockgroup)
            {
                result.IndexList.Add(item.Index);
                result.TimestampList.Add(item.Timestamp);
                result.TxCountList.Add(item.TxCount);
                result.SizeList.Add(item.Size);
            }
            SaveFile("txcount.json", JsonConvert.SerializeObject(result));
        }
        
        private static void Getvalidators()
        {
            using (var context = new Context())
            {
                JArray list = Helper.GetValidators();
                var result = new List<CandidateViewModels>();
                foreach (JObject item in list)
                {
                    var c = CandidateViewModels.FromJson(item);
                    c.Info = context.Candidates.FirstOrDefault(p => p.PublicKey == c.PublicKey);
                    result.Add(c);
                }
                var text = JsonConvert.SerializeObject(result.OrderByDescending(p => p.Votes).Select(p => new {
                    p.PublicKey,
                    p.Votes,
                    Info = p.Info == null ? null : new
                    {
                        p.Info.Email,
                        p.Info.Website,
                        p.Info.SocialAccount,
                        p.Info.Summary
                    },
                    p.Active
                }));
                SaveFile("validators.json", text);
            }
        }

        private static void SaveFile(string filename, string text)
        {
            try
            {
                File.WriteAllText(filename, text);
                Console.WriteLine($"Output: {filename}");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:" + e.Message);
            }
        }
    }
}
