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
        public double Evaluate(Individual ind)
        {
            // get bids
            var dbh = new DatabaseHelper();
            Dictionary<string, int> bids = dbh.GetBids();

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
    }
}
