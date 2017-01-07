using blueCow.Lib;
using GeneticSharp.Domain.Chromosomes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace blueCow.Models
{
    class Individual
    {
        public bool[] Cities { get; set; }
        public long ObjectiveValue { get; set; }
        public List<string> TravelOrder { get; set; }
        public long TourViolation { get; set; }
        public long ContinentViolation { get; set; }
        public List<Tour> TourPopulation { get; set; }
        public int CountriesVisited { get; set; }

        // full constructor
        public Individual(Random rand, DatabaseHelper dbh)
        {
            this.Cities = new bool[SysConfig.chromeLength];
            this.TravelOrder = new List<string>();
            // randomly choose a number of cities to visit between min and max
            int numCities = rand.Next(SysConfig.minCities, SysConfig.maxCities);
            this.CountriesVisited = numCities;
            // for each city 
            for (int i = 0; i < numCities; i++)
            {
                // generate a random index and if already selected choose a new one
                int index = rand.Next(0, SysConfig.chromeLength);
                while (this.Cities[index])
                {
                    index = rand.Next(0, SysConfig.chromeLength);
                }
                // set the random index to true (will visit)
                this.Cities[index] = true;
            }
            // get list of city codes for travel order
            Dictionary<string, int> bids = dbh.GetCountryCodeIndexes();
            for (var i = 0; i < this.Cities.Length; i++)
            {
                if (this.Cities[i])
                {
                    this.TravelOrder.Add(bids.Keys.ElementAt(i));
                }
            }
            // shuffle the initial order to expose new patterns
            this.TravelOrder.Shuffle();
            // create tour population
            this.TourPopulation = new List<Tour>();
            for (int i = 0; i < SysConfig.tourPopSize; i++)
            {
                List<string> randomTour = (List<string>)this.TravelOrder.Clone();
                randomTour.Shuffle();
                long violation = new ObjectiveFunction().TourViolation(randomTour, dbh);
                this.TourPopulation.Add(new Tour() { TravelOrder = randomTour, Violation = violation });
            }
        }

        // shell constructor
        public Individual()
        {
            this.Cities = new bool[SysConfig.chromeLength];
            this.TravelOrder = new List<string>();
            this.TourPopulation = new List<Tour>();
        }

        public void GenerateTours(DatabaseHelper dbh)
        {
            // get list of city codes for travel order
            Dictionary<string, int> bids = dbh.GetCountryCodeIndexes();
            for (var i = 0; i < this.Cities.Length; i++)
            {
                if (this.Cities[i])
                {
                    this.TravelOrder.Add(bids.Keys.ElementAt(i));
                }
            }
            // create tour population
            this.TourPopulation = new List<Tour>();
            for (int i = 0; i < SysConfig.tourPopSize; i++)
            {
                List<string> randomTour = (List<string>)this.TravelOrder.Clone();
                randomTour.Shuffle();
                long violation = new ObjectiveFunction().TourViolation(randomTour, dbh);
                this.TourPopulation.Add(new Tour() { TravelOrder = randomTour, Violation = violation });
            }
        }
    }
}
