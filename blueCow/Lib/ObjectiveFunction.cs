using blueCow.Models;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Fitnesses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class ObjectiveFunction
    {
        private DatabaseHelper _dbh;
        private ConstraintHandler _cons;

        public ObjectiveFunction()
        {
            _dbh = new DatabaseHelper();
            _cons = new ConstraintHandler();
        }

        public double Evaluate(Individual ind)
        {
            // get bids
            Dictionary<string, int> bids = _dbh.GetBids();

            double objective = 0;
            for(int i = 0; i < ind.Cities.Length; i++)
            {
                if (ind.Cities[i])
                {
                    objective += bids.Values.ElementAt(i);
                }
            }
            return objective;
        }
        
        public long TourViolation(List<string> tour)
        {
            long distance = 0;
            // add up each hop and store hop violations (illegal or too long/short)
            List<long> violations = new List<long>();
            for(int i = 0; i < tour.Count; i++)
            {
                if(i+1 >= tour.Count)
                {
                    distance += _dbh.GetDistance(tour[i], tour[(i - tour.Count) + 1]);
                    violations.Add(_cons.IllegalHopPenalty(tour[i], tour[(i - tour.Count) + 1]));
                    violations.Add(_cons.HopDistPenalty(_dbh.GetDistance(tour[i], tour[(i - tour.Count) + 1])));
                }
                else
                {
                    distance += _dbh.GetDistance(tour[i], tour[i + 1]);
                    violations.Add(_cons.IllegalHopPenalty(tour[i], tour[i + 1]));
                    violations.Add(_cons.HopDistPenalty(_dbh.GetDistance(tour[i], tour[i + 1])));
                }
            }
            // return sum of violations (constraint satisfaction) with any total distance violation
            violations.Add(Convert.ToInt32(_cons.TotalDistancePenalty(distance)));
            return violations.Sum();
        }

        public long TourViolation(List<string> tour, DatabaseHelper dbh)
        {
            long distance = 0;
            // add up each hop and store hop violations (illegal or too long/short)
            List<long> violations = new List<long>();
            for (int i = 0; i < tour.Count; i++)
            {
                if (i + 1 >= tour.Count)
                {
                    distance += dbh.GetDistanceFromMatrix(tour[i], tour[(i - tour.Count) + 1]);
                    violations.Add(_cons.IllegalHopPenalty(tour[i], tour[(i - tour.Count) + 1]));
                    violations.Add(_cons.HopDistPenalty(dbh.GetDistanceFromMatrix(tour[i], tour[(i - tour.Count) + 1])));
                }
                else
                {
                    distance += dbh.GetDistanceFromMatrix(tour[i], tour[i + 1]);
                    violations.Add(_cons.IllegalHopPenalty(tour[i], tour[i + 1]));
                    violations.Add(_cons.HopDistPenalty(dbh.GetDistanceFromMatrix(tour[i], tour[i + 1])));
                }
            }
            // return sum of violations (constraint satisfaction) with any total distance violation
            violations.Add(Convert.ToInt32(_cons.TotalDistancePenalty(distance)));
            return violations.Sum();
        }
    }
}
