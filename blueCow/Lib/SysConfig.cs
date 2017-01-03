using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    public static class SysConfig
    {
        public static readonly int chromeLength = 203;
        public static readonly string connString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\S14003221\Desktop\blueCow\blueCow\App_Data\Distances.mdf;Integrated Security=True";
        public static readonly int minBid = 500000;
        public static readonly int maxBid = 5000000;
        public static readonly int minCities = 30;
        public static readonly int maxCities = 50;
    }
}
