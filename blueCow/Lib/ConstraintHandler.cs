using blueCow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class ConstraintHandler
    {
        private int _illegalHopePenalty = 100000000;
        private int _hopDistPenalty = 10000;
        private int _totalDistPenalty = 10000;
        private int _continentPenalty = 100000000;

        public ConstraintHandler()
        {
        }

        public long TotalDistancePenalty(long dist)
        {
            return dist < SysConfig.minTotalDist ? (SysConfig.minTotalDist - dist) * _totalDistPenalty :
                (dist > SysConfig.maxTotalDist ? (dist - SysConfig.maxTotalDist) * _totalDistPenalty : 0);
        }

        public long HopDistPenalty(long dist)
        {
            return dist < SysConfig.minHopDist ? (SysConfig.minHopDist - dist) * _hopDistPenalty :
                (dist > SysConfig.maxHopDist ? (dist - SysConfig.maxHopDist) * _hopDistPenalty : 0);
        }

        public long IllegalHopPenalty(string start, string end)
        {
            bool illegal = false;
            foreach(var kvp in SysConfig.illegalHops)
            {
                if((start == kvp.Key && end == kvp.Value)||
                    start == kvp.Value && end == kvp.Key)
                {
                    illegal = true;
                }
            }
            return illegal ? _illegalHopePenalty : 0;
        }

        public long ContinentViolation(Individual ind, DatabaseHelper dbh)
        {
            Dictionary<string, int> numVisits = new Dictionary<string, int>();
            Dictionary<string, string> contCodes = dbh.GetContinentCodes();
            foreach(var c in SysConfig.majorContinents)
            {
                numVisits.Add(c, 0);
            }
            foreach(var c in ind.TravelOrder)
            {
                numVisits[dbh.GetContinentCode(c)]++;
                if (contCodes.ContainsKey(c + "_D"))
                {
                    numVisits[dbh.GetContinentCode(c+"_D")]++;
                }
            }
            long violation = 0;
            foreach(var kvp in numVisits)
            {
                if(kvp.Value < SysConfig.minCountriesPerContinent)
                {
                    violation += (SysConfig.minCountriesPerContinent - kvp.Value) * _continentPenalty;
                }
            }
            return violation;
        }
    }
}
