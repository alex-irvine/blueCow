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
        public static string connString = Properties.Settings.Default.Distances_vs2015ConnectionString1;
        //public static readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\S14003221\Desktop\blueCow\blueCow\App_Data\Distances.mdf;Integrated Security=True";
        public static int minBid = 100000;
        public static int maxBid = 10000000;
        public static int minCities = 20;
        public static int maxCities = 60;
        public static int maxTotalDist = 300000;
        public static int minTotalDist = 100000;
        public static int maxHopDist = 8000;
        public static int minHopDist = 1000;
        //changed to a list because if dictionnary, only one exception for one city (unique key)
        public static List<string[]> illegalHops = new List<string[]>();

        public static long illegalHopePenalty = 100000000;
        public static long hopDistPenalty = 100000;
        public static long totalDistPenalty = 100000;
        public static long continentPenalty = 100000000;

        public static double crossOverRate = 80;
        public static int crossoverPoints = 3;
        public static int mutationRate = 5;
        public static int tourPopSize = 50;
        public static string selectionMethod = "roulette";
        public static int stepSize = 10;

        
        public static List<string> majorContinents = new List<string>()
        {
            "AS",
            "EU",
            "AF",
            "OC",
            "NA",
            "SA"
        };
        public static int minCountriesPerContinent = 3;
        public static int africanCityLimit = 3;
        public static int europeanCityLimit = 3;
        public static int namericaCityLimit = 3;
        public static int samericanCityLimit = 3;
        public static int asiaCityLimit = 3;
        public static int oceaniaCityLimit = 3;

        public static int africanCityLimitMax = 10;
        public static int europeanCityLimitMax = 10;
        public static int namericaCityLimitMax = 10;
        public static int samericanCityLimitMax = 10;
        public static int asiaCityLimitMax = 10;
        public static int oceaniaCityLimitMax = 10;

        public static int maxTourGenerations = 100;

        public static string replacementMethod = "child to parent";
        public static string pdfPath = @"G:\blueCowReports\";
    }
}
