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

        // TODO: add hop checker to penalise distance if hop to short or long
        public long TourDistance(Individual ind)
        {
            long distance = 0;
            for(int i = 0; i < ind.TravelOrder.Count; i++)
            {
                if(i+1 >= ind.TravelOrder.Count)
                {
                    distance += _dbh.GetDistance(ind.TravelOrder[i], ind.TravelOrder[(i - ind.TravelOrder.Count) + 1]) *
                        _cons.IllegalHopPenalty(ind.TravelOrder[i], ind.TravelOrder[(i - ind.TravelOrder.Count) + 1]);
                }
                else
                {
                    distance += _dbh.GetDistance(ind.TravelOrder[i], ind.TravelOrder[i + 1]) *
                        _cons.IllegalHopPenalty(ind.TravelOrder[i], ind.TravelOrder[i + 1]);
                }
            }
            return distance;
        }
    }
}
