using System;
using System.Collections.Generic;
using System.Text;

namespace POATools.Modules
{
    public class Validator
    {
        public Validator()
        {
            voted = false;
            voteRecord = "";
            timeVoted = "";
        }

        public string mining { get; set; }
        public string voting { get; set; }
        public string timeVoted { get; set; }
        public bool voted { get; set; }
        public string voteRecord { get; set; }
    }
}
