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
        private List<Individual> _selectedMembers;

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
                _population.Add(new Individual(_rand,dbh));
            }
            return _population;
        }

        public List<Individual> GetPopulation()
        {
            return _population;
        }

        public List<Individual> EvaluatePopulation(Func<Individual,DatabaseHelper,long> objectiveFunc,DatabaseHelper dbh)
        {
            foreach(var i in _population)
            {
                i.ObjectiveValue = objectiveFunc(i, dbh);
            }
            return _population;
        }

        public List<Individual> EvaluateTourViolations(Func<List<string>, long> objectiveFunc)
        {
            foreach(var i in _population)
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
            // get first item to swap
            int ind1 = _rand.Next(0, travelOrder.Count - 1);
            // get second item to swap
            int ind2 = _rand.Next(0, travelOrder.Count - 1);
            while(ind1 == ind2)
            {
                ind1 = _rand.Next(0, travelOrder.Count - 1);
                ind2 = _rand.Next(0, travelOrder.Count - 1);
            }
            // swap order and return
            string itemAtInd1 = travelOrder[ind1];
            string itemAtInd2 = travelOrder[ind2];
            travelOrder[ind1] = itemAtInd2;
            travelOrder[ind2] = itemAtInd1;
            return travelOrder;
        }

        public List<string> CrossoverTravelOrder(List<string> parent1orig, List<string> parent2orig)
        {
            // create clones so as not to ruin parents
            var parent2 = (List<string>)parent2orig.Clone();
            var parent1 = (List<string>)parent1orig.Clone();
            // get subset
            int startingPoint = _rand.Next(0,parent1.Count-1);
            int length = _rand.Next(0, Convert.ToInt32(Math.Round((double)(parent1.Count / 2))));
            List<string> subset = new List<string>();
            int index = startingPoint;
            for(int i = 0; i < length; i++)
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
            for(int i = 0; i < length; i++)
            {
                if (index + 1 > parent1.Count)
                {
                    index -= parent1.Count;
                }
                child[index] = subset[i];
                index++;
            }
            // fill rest from parent 2
            for(int i = 0; i < child.Count; i++)
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
        public Tour RouletteSelectTour(List<Tour> tours)
        {
            // get all fitnesses
            long[] fitnesses = new long[tours.Count];
            foreach(var i in tours)
            {
                fitnesses[tours.IndexOf(i)] = i.Violation;
            }
            // get max
            long maxFitness = fitnesses.Max();
            if(maxFitness == 0)
            {
                // nothing to optimise just return first ind
                return tours[0];
            }
            while (true)
            {
                // randomly select a member
                Tour ind = tours[_rand.Next(0, tours.Count-1)];
                // get probablity and generate random number between 0 and 1 if probablity greater than or equal to number return ind
                double probability = 1 - (ind.Violation / maxFitness);
                if (_rand.Next(0, 100) / 100 <= probability)
                {
                    return ind;
                }
            }
        }

        public Tour TournamentSelectionTour(List<Tour> tours, int tournSize)
        {
            List<Tour> tournament = new List<Tour>();
            for(int i = 0; i < tournSize; i++)
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
            for(int i=0;i< tours.Count;i++)
            {
                if(tours[i].Violation < violation)
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
            foreach(var t in newTours)
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
                if(ind.TourPopulation[index].Violation > t.Violation)
                {
                    ind.TourPopulation[index] = t;
                }
            }
            return ind;
        }

        public bool[] CrossoverBids(bool[] parent1, bool[] parent2)
        {
            // just single point for now
            int xOverPoint = _rand.Next(0, parent1.Length - 1);
            bool[] child = new bool[parent1.Length];
            for(int i = 0; i <= xOverPoint; i++)
            {
                child[i] = parent1[i];
            }
            for(int i = xOverPoint; i < parent2.Length; i++)
            {
                child[i] = parent2[i];
            }
            return child;
        }

        public bool[] MutateBids(bool[] bids)
        {
            int bitToFlip = _rand.Next(0, bids.Length - 1);
            // flip and return
            bids[bitToFlip] = !bids[bitToFlip];
            return bids;
        }

        // stochastic acceptance
        public Individual RouletteSelectBids(List<Individual> inds)
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
                double probability = (ind.ObjectiveValue / maxFitness);
                if (_rand.Next(0, 100) / 100 <= probability)
                {
                    return ind;
                }
            }
        }

        public List<Individual> ReplaceWorstBids(List<Individual> origPop, List<Individual> newPop)
        { 
            foreach(var n in newPop)
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
