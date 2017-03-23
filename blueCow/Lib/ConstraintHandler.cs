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
        private long _illegalHopePenalty = SysConfig.illegalHopePenalty;
        private long _hopDistPenalty = SysConfig.hopDistPenalty;
        private long _totalDistPenalty = SysConfig.totalDistPenalty;
        private long _continentPenalty = SysConfig.continentPenalty;

        public ConstraintHandler()
        {
        }

        ///<summary>
        /// Check the total distance constraint
        /// </summary> 
        /// <param name="dist">distance</param>
        /// <returns></returns> 
        public long TotalDistancePenalty(long dist)
        {
            return dist < SysConfig.minTotalDist ? (SysConfig.minTotalDist - dist) * _totalDistPenalty :
                (dist > SysConfig.maxTotalDist ? (dist - SysConfig.maxTotalDist) * _totalDistPenalty : 0);
        }

        ///<summary>
        /// Check the hop distance constraint
        /// </summary> 
        /// <param name="dist">distance</param>
        /// <returns></returns>
        public long HopDistPenalty(long dist)
        {
            return dist < SysConfig.minHopDist ? (SysConfig.minHopDist - dist) * _hopDistPenalty :
                (dist > SysConfig.maxHopDist ? (dist - SysConfig.maxHopDist) * _hopDistPenalty : 0);
        }

        /// <summary>
        /// Check the illegal travel constraint
        /// </summary>
        /// <param name="start">Name of country (USA)</param>
        /// <param name="end">Name of country (USA)</param>
        /// <returns></returns>
        public long IllegalHopPenalty(string start, string end)
        {
            bool illegal = false;
            foreach(var kvp in SysConfig.illegalHops)
            {
                illegal = ((start == kvp[0] && end == kvp[1]) || (start == kvp[1] && end == kvp[0])) ? true : illegal || false;
            }
            return illegal ? _illegalHopePenalty : 0;
        }

        ///<summary>
        /// Check the Continent violation constraint
        /// </summary> 
        /// <param name="ind">individual?</param>
        /// <param name="dbh">database helper</param>
        /// <returns></returns>
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

            foreach(KeyValuePair<string, int> kvp  in numVisits)
            {
                switch (kvp.Key)
                {
                    case "SA":
                        if (kvp.Value < SysConfig.samericanCityLimit)                        
                            violation += (SysConfig.samericanCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.samericanCityLimitMax)
                            violation += (kvp.Value - SysConfig.samericanCityLimitMax) * _continentPenalty;
                        break;
                    case "AS":
                        if (kvp.Value < SysConfig.asiaCityLimit)
                            violation += (SysConfig.asiaCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.asiaCityLimitMax)
                            violation += (kvp.Value - SysConfig.asiaCityLimitMax) * _continentPenalty;
                        break;
                    case "EU":
                        if (kvp.Value < SysConfig.europeanCityLimit)
                            violation += (SysConfig.europeanCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.europeanCityLimitMax)
                            violation += (kvp.Value - SysConfig.europeanCityLimitMax) * _continentPenalty;
                        break;
                    case "AF":
                        if (kvp.Value < SysConfig.africanCityLimit)
                            violation += (SysConfig.africanCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.africanCityLimitMax)
                            violation += (kvp.Value - SysConfig.africanCityLimitMax) * _continentPenalty;
                        break;
                    case "OC":
                        if (kvp.Value < SysConfig.oceaniaCityLimit)
                            violation += (SysConfig.samericanCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.oceaniaCityLimitMax)
                            violation += (kvp.Value - SysConfig.oceaniaCityLimitMax) * _continentPenalty;
                        break;
                    case "NA":
                        if (kvp.Value < SysConfig.namericaCityLimit)
                            violation += (SysConfig.samericanCityLimit - kvp.Value) * _continentPenalty;
                        if (kvp.Value > SysConfig.namericaCityLimitMax)
                            violation += (kvp.Value - SysConfig.namericaCityLimitMax) * _continentPenalty;
                        break;
                }
            }
            return violation;
        }
    }
}
