using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class BidGenerator
    {
        public void GenerateBids()
        {
            Random rand = new Random();
            DatabaseHelper dbh = new DatabaseHelper();
            List<string> codes = dbh.GetCountryCodes();
            List<int> bids = new List<int>();
            foreach(var c in codes)
            {
                bids.Add(rand.Next(SysConfig.minBid, SysConfig.maxBid));
            }
            // load db
            dbh.LoadBids(codes, bids);
        }
    }
}
