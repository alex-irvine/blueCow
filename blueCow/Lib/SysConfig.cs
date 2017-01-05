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
         * Min kmdist = 13 (hop) (some are 0 for some reason? Different codes for the same country maybe)
         */
        public static int chromeLength = 203; // the number of cities
        public static string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=E:\Dropbox\Glyndwr\optimisation\blueCow\blueCow\App_Data\Distances_vs2015.mdf;Integrated Security=True";
        //public static readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\S14003221\Desktop\blueCow\blueCow\App_Data\Distances.mdf;Integrated Security=True";
        public static int minBid = 500000;
        public static int maxBid = 5000000;
        public static int minCities = 30;
        public static int maxCities = 50;
        public static int maxTotalDist = 400000;
        public static int minTotalDist = 100000;
        public static int maxHopDist = 10000;
        public static int minHopDist = 1000;
        public static Dictionary<string, string> illegalHops = new Dictionary<string, string>()
        {
            { "CON", "USA" },
            { "SPN", "GRG" },
            { "BOL", "CAO" },
            { "MOR", "NTH" },
            { "TOG", "CRO" }
        };
        public static double crossOverRate = 80;
        public static int mutationRate = 5;
        public static int tourPopSize = 50;
        public static string selectionMethod = "roulette";
    }
}
