using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace POATools
{
    public class ChainSpec
    {
        public string KeyStorageabi { get; }
        public string abi { get; }
        public string abiMetaData { get; }
        public string votingToManageEmissionFunds { get; }
        public string votingToChangeKeysABI { get; }
        public string VOTING_TO_CHANGE_KEYS_ADDRESS { get; }
        public string VOTING_TO_CHANGE_MIN_THRESHOLD_ADDRESS { get; }
        public string VOTING_TO_CHANGE_PROXY_ADDRESS { get; }
        public string VOTING_TO_MANAGE_EMISSION_FUNDS_ADDRESS { get; }
        public string BALLOTS_STORAGE_ADDRESS { get; }
        public string KEYS_MANAGER_ADDRESS { get; }
        public string METADATA_ADDRESS { get; }
        public string PROXY_ADDRESS { get; }
        public string POA_ADDRESS { get; }
        public string MOC { get; }

        public ChainSpec(string network)
        {
            KeyStorageabi = Helpers.GetAbi(network.ToLower(), "KeysManager");
            abi = Helpers.GetAbi(network.ToLower(), "PoaNetworkConsensus");
            abiMetaData = Helpers.GetAbi(network.ToLower(), "ValidatorMetadata");
            votingToChangeKeysABI = Helpers.GetAbi(network.ToLower(), "VotingToChangeKeys");
            votingToManageEmissionFunds = Helpers.GetAbi(network.ToLower(), "VotingToManageEmissionFunds");

            string contractsRaw = Helpers.GetContracts(network.ToLower());
            JObject o = JObject.Parse(contractsRaw);
            VOTING_TO_CHANGE_KEYS_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["VOTING_TO_CHANGE_KEYS_ADDRESS"]).Value.ToString();
            VOTING_TO_CHANGE_MIN_THRESHOLD_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["VOTING_TO_CHANGE_MIN_THRESHOLD_ADDRESS"]).Value.ToString();
            VOTING_TO_CHANGE_PROXY_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["VOTING_TO_CHANGE_PROXY_ADDRESS"]).Value.ToString();
            BALLOTS_STORAGE_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["BALLOTS_STORAGE_ADDRESS"]).Value.ToString();
            KEYS_MANAGER_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["KEYS_MANAGER_ADDRESS"]).Value.ToString();
            METADATA_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["METADATA_ADDRESS"]).Value.ToString();
            PROXY_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["PROXY_ADDRESS"]).Value.ToString();
            POA_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["POA_ADDRESS"]).Value.ToString();
            MOC = ((Newtonsoft.Json.Linq.JValue)o["MOC"]).Value.ToString();
            VOTING_TO_MANAGE_EMISSION_FUNDS_ADDRESS = ((Newtonsoft.Json.Linq.JValue)o["VOTING_TO_MANAGE_EMISSION_FUNDS_ADDRESS"]).Value.ToString();
        }
    }
}
