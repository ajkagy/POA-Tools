using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace POATools
{
    class Program
    {
        public static IConfiguration Configuration { get; set; }
        static void Main(string[] args)
        {
            try
            {
            var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("settings.json");

            Configuration = builder.Build();

            //Validation on Config Items
                if(Configuration["RPC"] == "")
                {
                        throw new Exception("Error: No RPC Server Set");
                }
                if (Configuration["BallotKey"] == "")
                {
                    throw new Exception("Error: Invalid Ballot Key");
                }
                if (Configuration["Network"].ToUpper() != "SOKOL" && Configuration["Network"].ToUpper() != "CORE")
                {
                    throw new Exception("Error: Invalid Network");
                }
                if (Int32.Parse(Configuration["Starting Block"].Trim()) < 0)
                {
                    throw new Exception("Error: Invalid Starting Block");
                }
                if (Int32.Parse(Configuration["Ballot"].Trim()) < 0)
                {
                    throw new Exception("Error: Invalid Ballot");
                }

                ChainSpec spec = new ChainSpec(Configuration["Network"]);

                IndividualBallot ballot = new IndividualBallot(Configuration["RPC"], Configuration["Network"], Int32.Parse(Configuration["Starting Block"]), Int32.Parse(Configuration["Ballot"]), Configuration["BallotKey"], spec);
                ballot.getValdatorVotingKeys().Wait();
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }
    }
}
