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
                _population.Add(new Individual);
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
    }
}
