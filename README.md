# POA-Tools

POA-Tools is a .NET CORE application that is built for POA Network based blockchains. It allows a user to see individual yes/no voting stats on individual ballots

# Prerequisites

1. Fully Archived Parity instance pointed at either Core or Sokol POA Network chains
(See [POA Installation](https://github.com/poanetwork/wiki/wiki/POA-Installation))
Extra command line params for Parity: --pruning=archive --no-warp

2. Windows/Mac/Linux OS

#Instructions
1. Open settings.json file and fill in the following infomation
    - RPC is defaulted to localhost. Leave this alone unless your rpc port is different for your local parity instance
    - Network: Use either "Sokol" or "Core"
    - Starting Block: The approximate starting block of where the ballot was created. !!This is important. If this is off then the program will not correctly read all of the votes recorded.!!
    - Ballot: The ballot number to search for in https://voting.poa.network
    - BallotKey: The affected key to look for in https://voting.poa.network

2. Start your local parity instance and let it fully sync
3. Build the program either in Visual Studio or run the provided release executable.