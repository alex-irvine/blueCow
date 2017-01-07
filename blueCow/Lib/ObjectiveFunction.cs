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
        private ConstraintHandler _cons;

        public ObjectiveFunction()
        {
            _cons = new ConstraintHandler();
        }

        public long Evaluate(Individual ind, DatabaseHelper dbh)
        {
            // get bids
            Dictionary<string, int> bids = dbh.GetBids();

            long objective = 0;
            for(int i = 0; i < ind.Cities.Length; i++)
            {
                if (ind.Cities[i])
                {
                    objective += bids.Values.ElementAt(i);
                }
            }
            // get violation of best tour
            objective -= ind.TourViolation;
            // get continent violations
            objective -= ind.ContinentViolation;
            return objective;
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
            violations.Add(_cons.TotalDistancePenalty(distance));
            return violations.Sum();
        }
    }
}
