using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace The_Betting_App
{
    public class Result
    {
        #region Variables
        String opposition;
        String outcome;
        String score;
        #endregion

        #region Constructor
        public Result(String opp, String outcome, String score) 
        {
            this.opposition = opp;
            this.outcome = outcome;
            this.score = score;
        }
        #endregion 

        #region Accessors
        public String getOpposition() { return this.opposition; }
        public String getOutcome() { return this.outcome; }
        public String getScore() { return this.score; }
        #endregion

    }
}
