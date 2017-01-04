using blueCow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Lib
{
    class OptimisationController
    {
        private GeneticAlgorithm _ga;
        private ObjectiveFunction _obj;
        private Random _rand;

        public OptimisationController(GeneticAlgorithm ga, ObjectiveFunction obj)
        {
            _ga = ga;
            _obj = obj;
            _rand = new Random();
        }

        // just one run (put in a loop)
        public Individual OptimiseTour(Individual ind, string selectionMethod, int tournamentSize = 5)
        {
            // Ind already set with a tour population and violations
            // select n number of parents for mating pool based on crossover rate
            int numCrossovers = Convert.ToInt32(Math.Round((SysConfig.crossOverRate / 100) * ind.TourPopulation.Count));
            List<Tour> newTours = new List<Tour>();
            for(int i = 0; i < numCrossovers; i++)
            {
                // get two parents using roulette or tournament
                Tour parent1 = selectionMethod == "roulette" ? _ga.RouletteSelectTour(ind.TourPopulation) :
                    _ga.TournamentSelectionTour(ind.TourPopulation, tournamentSize);
                Tour parent2 = selectionMethod == "roulette" ? _ga.RouletteSelectTour(ind.TourPopulation) :
                    _ga.TournamentSelectionTour(ind.TourPopulation, tournamentSize);
                // create two children
                Tour child1 = new Tour()
                {
                    TravelOrder = _ga.CrossoverTravelOrder(parent1.TravelOrder, parent2.TravelOrder)
                };
                Tour child2 = new Tour()
                {
                    TravelOrder = _ga.CrossoverTravelOrder(parent1.TravelOrder, parent2.TravelOrder)
                };
                // should we mutate?
                if (_rand.Next(1,100) < SysConfig.mutationRate)
                {
                    child1.TravelOrder = _ga.MutateTravelOrder(child1.TravelOrder);
                    child2.TravelOrder = _ga.MutateTravelOrder(child2.TravelOrder);
                }
                // if child better than either parent add to new pop
                child1.Violation = _ga.EvaluateTourViolation(_obj.TourViolation, child1.TravelOrder);
                child2.Violation = _ga.EvaluateTourViolation(_obj.TourViolation, child2.TravelOrder);
                if (child1.Violation < parent1.Violation || child1.Violation < parent2.Violation)
                {
                    newTours.Add(child1);
                }
                if (child2.Violation < parent1.Violation || child2.Violation < parent2.Violation)
                {
                    newTours.Add(child2);
                }
            }
            // replace tours
            ind = _ga.ReplaceWorstTours(ind, newTours);
            return ind;
        }

        // just one run (put in a loop)
        public Individual OptimiseTour(Individual ind, string selectionMethod, DatabaseHelper dbh, int tournamentSize = 5)
        {
            // Ind already set with a tour population and violations
            // select n number of parents for mating pool based on crossover rate
            int numCrossovers = Convert.ToInt32(Math.Round((SysConfig.crossOverRate / 100) * ind.TourPopulation.Count));
            List<Tour> newTours = new List<Tour>();
            for (int i = 0; i < numCrossovers; i++)
            {
                // get two parents using roulette or tournament
                Tour parent1 = selectionMethod == "roulette" ? _ga.RouletteSelectTour(ind.TourPopulation) :
                    _ga.TournamentSelectionTour(ind.TourPopulation, tournamentSize);
                Tour parent2 = selectionMethod == "roulette" ? _ga.RouletteSelectTour(ind.TourPopulation) :
                    _ga.TournamentSelectionTour(ind.TourPopulation, tournamentSize);
                Tour child1 = new Tour()
                {
                    TravelOrder = _ga.CrossoverTravelOrder(parent1.TravelOrder, parent2.TravelOrder)
                };
                // should we mutate?
                if (_rand.Next(1, 100) < SysConfig.mutationRate)
                {
                    child1.TravelOrder = _ga.MutateTravelOrder(child1.TravelOrder);
                }
                // if child better than either parent add to new pop
                child1.Violation = _ga.EvaluateTourViolation(_obj.TourViolation, child1.TravelOrder, dbh);
                if (child1.Violation < parent1.Violation || child1.Violation < parent2.Violation)
                {
                    newTours.Add(child1);
                }
            }
            // replace tours
            ind = _ga.ReplaceWorstTours(ind, newTours);
            return ind;
        }
    }
}
