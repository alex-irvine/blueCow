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
        private DatabaseHelper _dbh;
        private int _illegalHopePenalty = 1000000;
        private int _hopDistPenalty = 1000000;
        private int _totalDistPenalty = 1000000;

        public ConstraintHandler()
        {
            _dbh = new DatabaseHelper();
        }

        public int TotalDistancePenalty(int dist)
        {
            return dist < SysConfig.minTotalDist ? (SysConfig.minTotalDist - dist) * _totalDistPenalty :
                (dist > SysConfig.maxTotalDist ? (dist - SysConfig.maxTotalDist) * _totalDistPenalty : 0);
        }

        public int HopDistPenalty(int dist)
        {
            return dist < SysConfig.minHopDist ? (SysConfig.minHopDist - dist) * _hopDistPenalty :
                (dist > SysConfig.maxHopDist ? (dist - SysConfig.maxHopDist) * _hopDistPenalty : 0);
        }

        public int IllegalHopPenalty(string start, string end)
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
            return illegal ? _illegalHopePenalty : 1;
        }
    }
}
