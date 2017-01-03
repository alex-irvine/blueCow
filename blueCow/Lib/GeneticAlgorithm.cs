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

        public List<Individual> GeneratePopulation(int popSize)
        {
            _population = new List<Individual>();
            for(int i = 0; i < popSize; i++)
            {
                _population.Add(new Individual(_rand));
            }
            return _population;
        }

        public List<Individual> GetPopulation()
        {
            return _population;
        }

        public List<Individual> EvaluatePopulation(Func<Individual, double> objectiveFunc)
        {
            foreach(var i in _population)
            {
                i.ObjectiveValue = objectiveFunc(i);
            }
            return _population;
        }

        public List<Individual> EvaluateTourDistances(Func<Individual, long> objectiveFunc)
        {
            foreach(var i in _population)
            {
                i.TourDistance = objectiveFunc(i);
            }
            return _population;
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

        public List<string> CrossoverTravelOrder(List<string> parent1, List<string> parent2)
        {
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
            }
            // fill rest from parent 2
            for(int i = 0; i < child.Count; i++)
            {
                if (child[i] == null)
                {
                    for(int j = 0; j < parent2.Count; j++)
                    {
                        bool inList = false;
                        for(int k = 0; k < child.Count; k++)
                        {
                            if(parent2[j] == child[k])
                            {
                                inList = true;
                            }
                        }
                        if (!inList)
                        {
                            child[i] = parent2[j];
                            break;
                        }
                    }
                }
            }
            return child;
        }
    }
}
