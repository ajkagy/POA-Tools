using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace POATools
{
    public class Helpers
    {
        public static DateTime UnixTimeStampToDateTime(int unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds((int)unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string GetAbi(string network, string abi)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/poanetwork/poa-chain-spec/" + network.ToLower() + "/abis/" + abi + ".abi.json");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            return content;
        }

        public static string GetSpec(string network)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/poanetwork/poa-chain-spec/" + network.ToLower() + "/spec.json");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            return content;
        }

        public static string GetContracts(string network)
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead("https://raw.githubusercontent.com/poanetwork/poa-chain-spec/" + network.ToLower() + "/contracts.json");
            StreamReader reader = new StreamReader(stream);
            String content = reader.ReadToEnd();
            return content;
        }
    }
}
