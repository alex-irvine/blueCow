using blueCow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class GeneticAlgorithm
    {
        private Random _rand;
        private List<Individual> _population;

        public GeneticAlgorithm()
        {
            _rand = new Random();
        }

        public void SetPopulation(List<Individual> pop)
        {
            _population = pop;
        }

        public List<Individual> GeneratePopulation(int popSize, DatabaseHelper dbh)
        {
            _population = new List<Individual>();
            for (int i = 0; i < popSize; i++)
            {
                _population.Add(new Individual(_rand, dbh));
            }
            return _population;
        }

        public List<Individual> GenerateDiversePopulation(int popSize, DatabaseHelper dbh)
        {
            List<Individual> _population = new List<Individual>();
            bool[] alreadyBeen = new bool[SysConfig.chromeLength];
            Dictionary<string, int> bids = dbh.GetBids();
            var sortedDict = (from entry in bids orderby entry.Value descending select entry)
                .ToDictionary(pair => pair.Key, pair => pair.Value).Take(SysConfig.maxCities);
            for (int i = 0; i < popSize; i++)
            {
                // put highest possible val as first member
                if (i == 0)
                {
                    var indHighest = new Individual();
                    foreach(var kvp in sortedDict)
                    {
                        var idx = bids.IndexOf(kvp.Key);
                        indHighest.Cities[idx] = true;
                    }
                    indHighest.GenerateTours(dbh, _rand);
                    indHighest.CountriesVisited = SysConfig.maxCities;
                    _population.Add(indHighest);
                    continue;
                }
                var ind = new Individual();
                var numCities = _rand.Next(SysConfig.minCities, SysConfig.maxCities);
                for (int j = 0; j < numCities; j++)
                {
                    // generate a random index and if already selected choose a new one
                    int index = _rand.Next(0, SysConfig.chromeLength);
                    while (ind.Cities[index] || alreadyBeen[index])
                    {
                        index = _rand.Next(0, SysConfig.chromeLength);
                        if (alreadyBeen.Where(x => x == false).Count() < 1)
                        {
                            break;
                        }
                    }
                    // set the random index to true (will visit)
                    ind.Cities[index] = true;
                }
                ind.GenerateTours(dbh,_rand);
                ind.CountriesVisited = numCities;
                _population.Add(ind);
            }
            return _population;
        }

        public List<Individual> GetPopulation()
        {
            return _population;
        }

        public List<Individual> EvaluatePopulation(Func<Individual, DatabaseHelper, long> objectiveFunc, DatabaseHelper dbh)
        {
            foreach (var i in _population)
            {
                i.ObjectiveValue = objectiveFunc(i, dbh);
            }
            return _population;
        }

        public List<Individual> EvaluateTourViolations(Func<List<string>, long> objectiveFunc)
        {
            foreach (var i in _population)
            {
                i.TourViolation = objectiveFunc(i.TravelOrder);
            }
            return _population;
        }

        public long EvaluateTourViolation(Func<List<string>, long> objectiveFunc, List<string> tour)
        {
            return objectiveFunc(tour);
        }

        public long EvaluateTourViolation(Func<List<string>, DatabaseHelper, long> objectiveFunc, List<string> tour, DatabaseHelper dbh)
        {
            return objectiveFunc(tour, dbh);
        }

        public List<string> MutateTravelOrder(List<string> travelOrder)
        {
            int steps = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(travelOrder.Count * (SysConfig.stepSize / 100))));
            for (int i = 0; i < steps; i++)
            {
                // get first item to swap
                int ind1 = _rand.Next(0, travelOrder.Count - 1);
                // get second item to swap
                int ind2 = _rand.Next(0, travelOrder.Count - 1);
                while (ind1 == ind2)
                {
                    ind1 = _rand.Next(0, travelOrder.Count - 1);
                    ind2 = _rand.Next(0, travelOrder.Count - 1);
                }
                // swap order and return
                string itemAtInd1 = travelOrder[ind1];
                string itemAtInd2 = travelOrder[ind2];
                travelOrder[ind1] = itemAtInd2;
                travelOrder[ind2] = itemAtInd1;
            }
            return travelOrder;
        }

        public List<string> CrossoverTravelOrder(List<string> parent1orig, List<string> parent2orig)
        {
            // create clones so as not to ruin parents
            var parent2 = (List<string>)parent2orig.Clone();
            var parent1 = (List<string>)parent1orig.Clone();
            // get subset
            int startingPoint = _rand.Next(0, parent1.Count - 1);
            int length = _rand.Next(0, Convert.ToInt32(Math.Round((double)(parent1.Count / 2))));
            List<string> subset = new List<string>();
            int index = startingPoint;
            for (int i = 0; i < length; i++)
            {
                if (index + 1 > parent1.Count)
                {
                    index -= parent1.Count;
                }
                subset.Add(parent1[index]);
                // remove the added element from parent 2 for later
                for (int j = 0; j < parent2.Count; j++)
                {
                    if (parent1[index] == parent2[j])
                    {
                        parent2.RemoveAt(j);
                        break;
                    }
                }
                index++;
            }
            // initialise child
            List<string> child = new List<string>(new string[parent1.Count]);
            // bung in the subset
            index = startingPoint;
            for (int i = 0; i < length; i++)
            {
                if (index + 1 > parent1.Count)
                {
                    index -= parent1.Count;
                }
                child[index] = subset[i];
                index++;
            }
            // fill rest from parent 2
            for (int i = 0; i < child.Count; i++)
            {
                if (child[i] == null)
                {
                    child[i] = parent2[0];
                    parent2.RemoveAt(0);
                }
            }
            return child;
        }

        // stochastic acceptance
        public Tour RouletteSelectTour(List<Tour> tours, Tour dontSelect = null)
        {
            // get all fitnesses
            long[] fitnesses = new long[tours.Count];
            foreach (var i in tours)
            {
                fitnesses[tours.IndexOf(i)] = i.Violation;
            }
            // get max
            long maxFitness = fitnesses.Max();
            if (maxFitness == 0)
            {
                // nothing to optimise just return random ind
                return tours[_rand.Next(0,tours.Count - 1)];
            }
            while (true)
            {
                // randomly select a member
                Tour ind = tours[_rand.Next(0, tours.Count - 1)];
                // causes infini loop
                //if(dontSelect != null)
                //{
                //    if (ind.TravelOrder.OrderAndStringEquals(dontSelect.TravelOrder))
                //    {
                //        continue;
                //    }
                //}
                // get probablity and generate random number between 0 and 1 if probablity greater than or equal to number return ind
                double probability = 1 - Convert.ToDouble(ind.Violation) / Convert.ToDouble(maxFitness);
                if (Convert.ToDouble(_rand.Next(0, 100)) / 100 <= probability)
                {
                    return ind;
                }
            }
        }

        public Tour TournamentSelectionTour(List<Tour> tours, int tournSize)
        {
            List<Tour> tournament = new List<Tour>();
            for (int i = 0; i < tournSize; i++)
            {
                // get random index
                int index = _rand.Next(0, tours.Count - 1);
                tournament.Add(tours[index]);
            }
            // return lowest violation
            return GetFittestTour(tournament);
        }

        public Tour GetFittestTour(List<Tour> tours)
        {
            long violation = long.MaxValue;
            int index = 0;
            for (int i = 0; i < tours.Count; i++)
            {
                if (tours[i].Violation < violation)
                {
                    violation = tours[i].Violation;
                    index = i;
                }
            }
            return tours[index];
        }

        public Individual ReplaceWorstTours(Individual ind, List<Tour> newTours)
        {
            int index;
            long worstViolation;
            foreach (var t in newTours)
            {
                index = 0;
                worstViolation = ind.TourPopulation[0].Violation;
                foreach (var i in ind.TourPopulation)
                {
                    if (i.Violation >= worstViolation)
                    {
                        index = ind.TourPopulation.IndexOf(i);
                        worstViolation = i.Violation;
                    }
                }
                if (ind.TourPopulation[index].Violation > t.Violation)
                {
                    ind.TourPopulation[index] = t;
                }
            }
            return ind;
        }

        public List<Tour> ReplaceParentTour(List<Tour> tours, Tour parent, Tour child)
        {
            tours[tours.IndexOf(parent)] = child;
            return tours;
        }

        public bool[] CrossoverBids(bool[] parent1, bool[] parent2)
        {
            int xOverPoint = _rand.Next(0, Convert.ToInt32(Math.Round(Convert.ToDouble(parent1.Length/2))));
            int secondStartingPoint = 0;
            bool[] child = new bool[parent1.Length];
            for (int i = 0; i <= xOverPoint; i++)
            {
                child[i] = parent1[i];
                secondStartingPoint = xOverPoint + 1;
            }
            xOverPoint = _rand.Next(secondStartingPoint, parent2.Length - 1);
            for (int i = secondStartingPoint; i <= xOverPoint; i++)
            {
                child[i] = parent2[i];
            }
            for(int i = xOverPoint+1; i < parent1.Length; i++)
            {
                child[i] = parent1[i];
            }
            return child;
        }

        public bool[] MutateBids(bool[] bids)
        {
            int bitToFlip;
            int bitsToFlip = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(SysConfig.chromeLength * (SysConfig.stepSize / 100))));
            // flip and return
            for(int i = 0; i < bitsToFlip; i++)
            {
                bitToFlip = _rand.Next(0, bids.Length - 1);
                bids[bitToFlip] = !bids[bitToFlip];
            }
            return bids;
        }

        // stochastic acceptance
        public Individual RouletteSelectBids(List<Individual> inds, Individual dontSelect = null)
        {
            // get all fitnesses
            long[] fitnesses = new long[inds.Count];
            foreach (var i in inds)
            {
                fitnesses[inds.IndexOf(i)] = i.ObjectiveValue;
            }
            // get max
            long maxFitness = fitnesses.Max();
            while (true)
            {
                // randomly select a member
                Individual ind = inds[_rand.Next(0, inds.Count - 1)];
                // could cause infini loop
                //if (dontSelect != null)
                //{
                //    if (ind.Cities.OrderAndBoolEquals(dontSelect.Cities))
                //    {
                //        continue;
                //    }
                //}
                double probability = Convert.ToDouble(ind.ObjectiveValue) / Convert.ToDouble(maxFitness);
                if (Convert.ToDouble(_rand.Next(0, 100)) / 100 <= probability)
                {
                    return ind;
                }
            }
        }

        public Individual TournamentSelectBids(List<Individual> inds, int tournSize)
        {
            List<Individual> tournament = new List<Individual>();
            for (int i = 0; i < tournSize; i++)
            {
                // get random index
                int index = _rand.Next(0, inds.Count - 1);
                tournament.Add(inds[index]);
            }
            // return lowest violation
            return GetFittestIndividual(tournament);
        }

        public List<Individual> ReplaceWorstBids(List<Individual> origPop, List<Individual> newPop)
        {
            foreach (var n in newPop)
            {
                int index = 0;
                long worstObjective = origPop[0].ObjectiveValue;
                foreach (var i in origPop)
                {
                    if (i.ObjectiveValue <= worstObjective)
                    {
                        worstObjective = i.ObjectiveValue;
                        index = origPop.IndexOf(i);
                    }
                }
                if (origPop[index].ObjectiveValue < n.ObjectiveValue)
                {
                    origPop[index] = n;
                }
            }
            return origPop;
        }

        public List<Individual> ReplaceParent(Individual parent, Individual child)
        {
            _population[_population.IndexOf(parent)] = child;
            return _population;
        }

        public Individual GetFittestIndividual(List<Individual> inds)
        {
            long obj = 0;
            int index = 0;
            foreach(var i in inds)
            {
                if(i.ObjectiveValue > obj)
                {
                    obj = i.ObjectiveValue;
                    index = inds.IndexOf(i);
                }
            }
            return inds[index];
        }

        public Individual GetFittestIndividual()
        {
            long obj = 0;
            int index = 0;
            foreach (var i in _population)
            {
                if (i.ObjectiveValue > obj)
                {
                    obj = i.ObjectiveValue;
                    index = _population.IndexOf(i);
                }
            }
            return _population[index];
        }
    }
}
