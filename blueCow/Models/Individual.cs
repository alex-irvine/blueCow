using blueCow.Lib;
using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Models
{
    class Individual
    {
        public bool[] Cities { get; set; }
        public double ObjectiveValue { get; set; }

        public Individual()
        {
            Random rand = new Random();
            this.Cities = new bool[SysConfig.chromeLength];
            int[] indexes = new int[rand.Next(SysConfig.minCities, SysConfig.maxCities)];
            foreach(var i in indexes)
            {

            }
            for(int i = 0; i < SysConfig.chromeLength; i++)
            {
                this.Cities[i] = rand.Next(0, 1000) < 500 ? false : true;
            }
        }
    }
}
