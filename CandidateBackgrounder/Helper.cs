using CandidateBackgrounder.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CandidateBackgrounder
{
    static class Helper
    {
        public static string PostWebRequest(string postUrl, string paramData)
        {
            try
            {
                byte[] byteArray = Encoding.UTF8.GetBytes(paramData);
                WebRequest webReq = WebRequest.Create(postUrl);
                webReq.Method = "POST";
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);
                }
                using (WebResponse response = webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "";
        }

        public static bool IsConnect(string ip)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                SendTimeout = 1000,
                ReceiveTimeout = 1000
            };
            try
            {
                socket.Connect(ip, 10333);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        static readonly string RequestURL = (string)JObject.Parse(File.ReadAllText("Config.json"))["RequestURL"];

        public static Block GetBlock(int index)
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var response = Helper.PostWebRequest(RequestURL, $"{{'jsonrpc': '2.0', 'method': 'getblock', 'params': [{index},1],  'id': 1}}");
                    if (string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine("Please run neo-cli.");
                        return null;
                    }
                    return Block.FromJson(JObject.Parse(response)["result"]);
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        public static int GetBlockCount()
        {
            for (int i = 0; i < 5; i++)
            {
                try
                {
                    var response = Helper.PostWebRequest(RequestURL, "{'jsonrpc': '2.0', 'method': 'getblockcount', 'params': [],  'id': 1}");
                    if (string.IsNullOrEmpty(response))
                    {
                        Console.WriteLine("Please run neo-cli.");
                        return 0;
                    }
                    return (int)JObject.Parse(response)["result"];
                }
                catch (Exception)
                {
                }
            }
            return 0;
        }
        
        public static JArray GetValidators()
        {
            var response = Helper.PostWebRequest(Helper.RequestURL, "{'jsonrpc': '2.0', 'method': 'getvalidators', 'params': [],  'id': 1}");
            if (string.IsNullOrEmpty(response))
            {
                Console.WriteLine("Please run neo-cli.");
                return null;
            }
            return (JArray)JObject.Parse(response)["result"];
        }
    }
}
