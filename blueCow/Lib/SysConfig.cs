using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    public static class SysConfig
    {
        /**
         * Fun facts:
         * Max kmdist = 19918 (Hop)
         * Min kmdist = 13 (hop) (some are 0 for some reason? Different codes fo the same country maybe)
         */
        public static readonly int chromeLength = 203;
        public static readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Dropbox\Glyndwr\optimisation\blueCow\blueCow\App_Data\Distances_vs2015.mdf;Integrated Security=True";
        //public static readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\S14003221\Desktop\blueCow\blueCow\App_Data\Distances.mdf;Integrated Security=True";
        public static readonly int minBid = 500000;
        public static readonly int maxBid = 5000000;
        public static readonly int minCities = 30;
        public static readonly int maxCities = 50;
        public static readonly int maxTotalDist = 400000;
        public static readonly int minTotalDist = 100000;
        public static readonly int maxHopDist = 10000;
        public static readonly int minHopDist = 1000;
        public static readonly Dictionary<string, string> illegalHops = new Dictionary<string, string>()
        {
            { "CON", "USA" },
            { "SPN", "GRG" },
            { "BOL", "CAO" },
            { "MOR", "NTH" },
            { "TOG", "CRO" }
        };
        public static readonly double crossOverRate = 80;
        public static readonly int mutationRate = 1;
        public static readonly int tourPopSize = 20;
    }
}
