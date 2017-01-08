using blueCow.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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

        public List<Individual> InitialisePopulation(int popSize, DatabaseHelper dbh, ProgressBar progBar = null)
        {
            var population = _ga.GeneratePopulation(popSize, dbh);
            // optimise tours and set tour constraint and get best tour
            for(int i=0;i<population.Count;i++)
            {
                for(int j = 0; j < SysConfig.maxTourGenerations; j++)
                {
                    population[i] = OptimiseTour(population[i], SysConfig.selectionMethod, dbh);
                    if(population[i].TourViolation == 0)
                    {
                        break;
                    }
                }
                // set continent constraint
                population[i].ContinentViolation = new ConstraintHandler().ContinentViolation(population[i], dbh);
                if(progBar != null)
                {
                    progBar.Value = i + 1;
                }
            }
            _ga.SetPopulation(population);
            return _ga.EvaluatePopulation(new ObjectiveFunction().Evaluate, dbh);
        }

        public List<Individual> InitialiseDiversePopulation(int popSize, DatabaseHelper dbh, ProgressBar progBar = null)
        {
            var population = _ga.GenerateDiversePopulation(popSize, dbh);
            // optimise tours and set tour constraint and get best tour
            for (int i = 0; i < population.Count; i++)
            {
                for (int j = 0; j < SysConfig.maxTourGenerations; j++)
                {
                    population[i] = OptimiseTour(population[i], SysConfig.selectionMethod, dbh);
                    if (population[i].TourViolation == 0)
                    {
                        break;
                    }
                }
                // set continent constraint
                population[i].ContinentViolation = new ConstraintHandler().ContinentViolation(population[i], dbh);
                if (progBar != null)
                {
                    progBar.Value = i + 1;
                }
            }
            _ga.SetPopulation(population);
            return _ga.EvaluatePopulation(new ObjectiveFunction().Evaluate, dbh);
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
                //Tour child2 = new Tour()
                //{
                //    TravelOrder = _ga.CrossoverTravelOrder(parent2.TravelOrder, parent1.TravelOrder)
                //};
                // should we mutate?
                if (_rand.Next(1, 100) < SysConfig.mutationRate)
                {
                    child1.TravelOrder = _ga.MutateTravelOrder(child1.TravelOrder);
                    //child2.TravelOrder = _ga.MutateTravelOrder(child2.TravelOrder);
                }
                // if child better than either parent add to new pop
                child1.Violation = _ga.EvaluateTourViolation(_obj.TourViolation, child1.TravelOrder, dbh);
                if (child1.Violation < parent1.Violation || child1.Violation < parent2.Violation)
                {
                    newTours.Add(child1);
                }
                //child2.Violation = _ga.EvaluateTourViolation(_obj.TourViolation, child2.TravelOrder, dbh);
                //if (child2.Violation < parent1.Violation || child2.Violation < parent2.Violation)
                //{
                //    newTours.Add(child2);
                //}
            }
            // replace tours
            ind = _ga.ReplaceWorstTours(ind, newTours);
            // set best tour in individual
            Tour bestTour = _ga.GetFittestTour(ind.TourPopulation);
            ind.TravelOrder = bestTour.TravelOrder;
            ind.TourViolation = bestTour.Violation;
            return ind;
        }

        public List<Individual> OptimiseBidsWithConstraints(List<Individual> inds, DatabaseHelper dbh, string selectionMethod, int tournamentSize = 5)
        {
            // pop should be created and initially evaluated (List<Individual>)
            // just one run (put in loop)
            int numCrossovers = Convert.ToInt32(Math.Round((SysConfig.crossOverRate / 100) * inds.Count));
            List<Individual> newPop = new List<Individual>();
            for (int i = 0; i < numCrossovers; i++)
            {
                // get two parents using roulette or tournament
                Individual parent1 = selectionMethod == "roulette" ? _ga.RouletteSelectBids(inds) : _ga.TournamentSelectBids(inds, tournamentSize);
                Individual parent2 = selectionMethod == "roulette" ? _ga.RouletteSelectBids(inds) : _ga.TournamentSelectBids(inds, tournamentSize);
                Individual child1 = new Individual()
                {
                    Cities = _ga.CrossoverBids(parent1.Cities, parent2.Cities)
                };
                // should we mutate?
                if (_rand.Next(1, 100) < SysConfig.mutationRate)
                {
                    child1.Cities = _ga.MutateBids(child1.Cities);
                }
                // check not over or under cities limit
                int numCities = 0;
                foreach(var c in child1.Cities)
                {
                    if (c)
                    {
                        numCities++;
                    }
                }
                if(numCities > SysConfig.maxCities || numCities < SysConfig.minCities)
                {
                    // no good onto next one
                    continue;
                }
                child1.CountriesVisited = numCities;
                child1.GenerateTours(dbh);
                // evaluate child
                // optimise tour first
                for(int j = 0; j < SysConfig.maxTourGenerations; j++)
                {
                    child1 = OptimiseTour(child1, SysConfig.selectionMethod, dbh);
                    if(child1.TourViolation == 0)
                    {
                        break;
                    }
                }
                // set continent constraint
                child1.ContinentViolation = new ConstraintHandler().ContinentViolation(child1, dbh);
                // evaluate constrained objective
                child1.ObjectiveValue = _obj.Evaluate(child1, dbh);
                // if child better than either parent add to new pop
                if (child1.ObjectiveValue > parent1.ObjectiveValue || child1.ObjectiveValue > parent2.ObjectiveValue)
                {
                    //replace worst parent
                    newPop = parent1.ObjectiveValue < parent2.ObjectiveValue ?
                        _ga.ReplaceParent(parent1, child1) : _ga.ReplaceParent(parent2, child1);
                }
            }
            // return new population
            return newPop;
        }
    }
}
