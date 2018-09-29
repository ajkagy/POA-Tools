using System;
using System.Collections.Generic;
using System.Text;
using Nethereum.Contracts.CQS;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.Blocks;
using Nethereum.RPC.Eth.DTOs;
using System.Numerics;
using Nethereum.ABI;
using Nethereum.ABI.FunctionEncoding;
using System.Threading.Tasks;
using POATools.Modules;

namespace POATools
{
    public class IndividualBallot
    {
        private string _rpc;
        private string _network;
        private string _mode;
        private int _startingBlock;
        private int _ballot;
        private string _ballotKey;
        private ChainSpec _specData;
        private bool _ballotInfoSet;
        private string _ballotInfo;

        public IndividualBallot(string rpc, string network, string mode, int startingBlock, int ballot, string ballotKey, ChainSpec spec)
        {
            _rpc = rpc;
            _network = network;
            _startingBlock = startingBlock;
            _ballot = ballot;
            _specData = spec;
            _ballotKey = ballotKey;
            _ballotInfoSet = false;
            _mode = mode;
            _ballotInfo = "";
        }

        public async Task<Int32> findEmissionBallotStats()
        {
            var web3 = new Nethereum.Web3.Web3(_rpc);
            Nethereum.ABI.Decoders.StringTypeDecoder decoder = new Nethereum.ABI.Decoders.StringTypeDecoder();

            var votingToManageEmissionFundsContract = web3.Eth.GetContract(_specData.votingToManageEmissionFunds, _specData.VOTING_TO_MANAGE_EMISSION_FUNDS_ADDRESS);

            var contract = web3.Eth.GetContract(_specData.abi, _specData.POA_ADDRESS);
            var getValidators = contract.GetFunction("getValidators");
            List<string> resultValidators = await getValidators.CallAsync<List<string>>();

            var keysStorageContract = web3.Eth.GetContract(_specData.KeyStorageabi, _specData.KEYS_MANAGER_ADDRESS);
            var getVotingByMining = keysStorageContract.GetFunction("getVotingByMining");
            List<string> votingKeys = new List<string>();
            List<Validator> validators = new List<Validator>();
            for (int i = 0; i < resultValidators.Count; i++)
            {
                Validator val = new Validator();
                var votingKey = await getVotingByMining.CallAsync<string>(resultValidators[i]);
                val.voting = votingKey;
                val.mining = resultValidators[i];
                validators.Add(val);
            }

            //Example Starting Blocks on Sokol
            //2721500 James
            //2824000 Theresa
            //2922300 Artiom
            //2959323 Kristina
            HexBigInteger maxBlockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            for (var i = _startingBlock; i <= maxBlockNumber.Value; i++)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine("Searching Block  " + i);
                }

                var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(i));
                if (block != null && block.Transactions != null)
                {

                    for (int y = 0; y < block.Transactions.Length; y++)
                    {
                        if (block.Transactions[y].To != null)
                        {
                            if (block.Transactions[y].To.ToLower() == _specData.VOTING_TO_MANAGE_EMISSION_FUNDS_ADDRESS.ToLower() && validators.Find(x => x.voting.Contains(block.Transactions[y].From)) != null)
                            {
                                Console.WriteLine("Transaction Found, block: " + block.Number.Value.ToString());
                                Validator v = validators.Find(x => x.voting.Contains(block.Transactions[y].From));
                                DateTime dt = Helpers.UnixTimeStampToDateTime((int)block.Timestamp.Value);
                                try
                                {
                                    List<ParameterOutput> output = votingToManageEmissionFundsContract.GetFunction("vote").DecodeInput(block.Transactions[y].Input);
                                    if(output.Count == 2)
                                    {
                                        if ((BigInteger)output[0].Result == _ballot)
                                        {
                                            if((BigInteger)output[1].Result == 1)
                                            {
                                                v.voted = true;
                                                v.voteRecord = "Send";
                                                v.timeVoted = dt.ToString("MM/dd/yyyy hh:mm tt");
                                            }
                                            else if ((BigInteger)output[1].Result == 2)
                                            {
                                                v.voted = true;
                                                v.voteRecord = "Burn";
                                                v.timeVoted = dt.ToString("MM/dd/yyyy hh:mm tt");
                                            }
                                            else if ((BigInteger)output[1].Result == 3)
                                            {
                                                v.voted = true;
                                                v.voteRecord = "Freeze";
                                                v.timeVoted = dt.ToString("MM/dd/yyyy hh:mm tt");
                                            }
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {

                                }

                                if (!_ballotInfoSet)
                                {
                                    try
                                    {
                                        List<ParameterOutput> output = votingToManageEmissionFundsContract.GetFunction("createBallot").DecodeInput(block.Transactions[y].Input);
                                        if (output.Count == 4)
                                        {
                                            if (output[2].Result.ToString().ToLower() == _ballotKey.ToLower())
                                            {
                                                //Ballot Info
                                                try
                                                {
                                                    if (!_ballotInfoSet)
                                                    {
                                                        _ballotInfo = output[3].Result.ToString();
                                                        _ballotInfoSet = true;
                                                    }
                                                }
                                                catch (Exception ex)
                                                {

                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                               
                            }
                        }
                    }

                }
            }

            if (_ballotInfoSet)
            {
                Console.WriteLine(_ballotInfo + Environment.NewLine);
            }
            for (int i = 0; i < validators.Count; i++)
            {
                var c = web3.Eth.GetContract(_specData.abiMetaData, _specData.METADATA_ADDRESS);
                var f = c.GetFunction("validators");
                var valName = await f.CallAsync<string>(validators[i].mining);
                string vote = validators[i].voteRecord;
                if (validators[i].voteRecord == "")
                {
                    vote = "N/A";
                }
                Console.WriteLine("Validator: " + validators[i].mining + " - " + valName + "  VOTED: " + vote + " - VOTE TIME: " + validators[i].timeVoted);
            }

            return 1;
        }

        public async Task<Int32> getValdatorVotingKeys()
        {
            var web3 = new Nethereum.Web3.Web3(_rpc);
            Nethereum.ABI.Decoders.StringTypeDecoder decoder = new Nethereum.ABI.Decoders.StringTypeDecoder();

            var votingToChangeKeysContract = web3.Eth.GetContract(_specData.votingToChangeKeysABI, _specData.VOTING_TO_CHANGE_KEYS_ADDRESS);
            var contract = web3.Eth.GetContract(_specData.abi, _specData.POA_ADDRESS);
            var getValidators = contract.GetFunction("getValidators");
            List<string> resultValidators = await getValidators.CallAsync<List<string>>();

            var keysStorageContract = web3.Eth.GetContract(_specData.KeyStorageabi, _specData.KEYS_MANAGER_ADDRESS);
            var getVotingByMining = keysStorageContract.GetFunction("getVotingByMining");
            List<string> votingKeys = new List<string>();
            List<Validator> validators = new List<Validator>();
            for (int i = 0; i < resultValidators.Count; i++)
            {
                Validator val = new Validator();
                var votingKey = await getVotingByMining.CallAsync<string>(resultValidators[i]);
                val.voting = votingKey;
                val.mining = resultValidators[i];
                validators.Add(val);
            }

            //Example Starting Blocks on Sokol
            //2721500 James
            //2824000 Theresa
            //2922300 Artiom
            //2959323 Kristina
            HexBigInteger maxBlockNumber = await web3.Eth.Blocks.GetBlockNumber.SendRequestAsync();
            for (var i = _startingBlock; i <= maxBlockNumber.Value; i++)
            {
                if (i % 1000 == 0)
                {
                    Console.WriteLine("Searching Block  " + i);
                }

                var block = await web3.Eth.Blocks.GetBlockWithTransactionsByNumber.SendRequestAsync(new HexBigInteger(i));
                if (block != null && block.Transactions != null)
                {

                    for (int y = 0; y < block.Transactions.Length; y++)
                    {
                        if (block.Transactions[y].To != null)
                        {
                            if (block.Transactions[y].To.ToLower() == _specData.VOTING_TO_CHANGE_KEYS_ADDRESS.ToLower() && validators.Find(x => x.voting.Contains(block.Transactions[y].From)) != null)
                            {
                                Console.WriteLine("Transaction Found, block: " + block.Number.Value.ToString());
                                Validator v = validators.Find(x => x.voting.Contains(block.Transactions[y].From));
                                DateTime dt = Helpers.UnixTimeStampToDateTime((int)block.Timestamp.Value);
                                if (block.Transactions[y].Input.Substring(block.Transactions[y].Input.Length - 1) == "1")
                                {
                                    List<ParameterOutput> output = votingToChangeKeysContract.GetFunction("vote").DecodeInput(block.Transactions[y].Input);
                                    //Yes Vote
                                    if ((BigInteger)output[0].Result == _ballot)
                                    {
                                        v.voted = true;
                                        v.voteRecord = "Yes";
                                        v.timeVoted = dt.ToString("MM/dd/yyyy hh:mm tt");
                                    }
                                }
                                else if (block.Transactions[y].Input.Substring(block.Transactions[y].Input.Length - 1) == "2")
                                {
                                    List<ParameterOutput> output = votingToChangeKeysContract.GetFunction("vote").DecodeInput(block.Transactions[y].Input);
                                    //No Vote
                                    if ((BigInteger)output[0].Result == _ballot)
                                    {
                                        v.voted = true;
                                        v.timeVoted = dt.ToString("MM/dd/yyyy hh:mm tt");
                                        v.voteRecord = "No";
                                    }
                                }
                                else
                                {
                                    //Ballot Info
                                    try
                                    {
                                        if (!_ballotInfoSet)
                                        {
                                            List<ParameterOutput> outputBallotCreate = votingToChangeKeysContract.GetFunction("createVotingForKeys").DecodeInput(block.Transactions[y].Input);
                                            if (outputBallotCreate[2].Result.ToString().ToLower() == _ballotKey.ToLower())
                                            {
                                                _ballotInfo = outputBallotCreate[6].Result.ToString();
                                                _ballotInfoSet = true;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                        }
                    }

                }
            }

            if (_ballotInfoSet)
            {
                Console.WriteLine(_ballotInfo + Environment.NewLine);
            }
            for (int i = 0; i < validators.Count; i++)
            {
                var c = web3.Eth.GetContract(_specData.abiMetaData, _specData.METADATA_ADDRESS);
                var f = c.GetFunction("validators");
                var valName = await f.CallAsync<string>(validators[i].mining);
                string vote = validators[i].voteRecord;
                if (validators[i].voteRecord == "")
                {
                    vote = "N/A";
                }
                Console.WriteLine("Validator: " + validators[i].mining + " - " + valName + "  VOTED: " + vote + " - VOTE TIME: " + validators[i].timeVoted);
            }

            return 1;
        }
    }
}
