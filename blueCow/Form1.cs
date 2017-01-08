using blueCow.Lib;
using blueCow.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace blueCow
{
    public partial class Form1 : Form
    {
        private DatabaseHelper _dbh;
        private GeneticAlgorithm _ga;
        private OptimisationController _opt;

        public Form1()
        {
            InitializeComponent();
            _dbh = new DatabaseHelper();
            _ga = new GeneticAlgorithm();
            _opt = new OptimisationController(_ga,new ObjectiveFunction());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BidGenerator bg = new BidGenerator();
            bg.GenerateBids();
            Dictionary<string, int> bids = _dbh.GetBids();
            listBox1.Items.Clear();
            foreach(var kvp in bids)
            {
                listBox1.Items.Add(kvp);
            }
        }

        private void ShowEvaluations()
        {
            ObjectiveFunction obj = new ObjectiveFunction();
            List<Individual> inds = _ga.EvaluatePopulation(obj.Evaluate,_dbh);
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.ColumnCount = SysConfig.chromeLength;
            for(var i=0; i < inds.Count; i++)
            {
                listBox3.Items.Add(inds[i].ObjectiveValue.ToString());
                listBox4.Items.Add("Tour: " + inds[i].TourViolation.ToString() + " Continent: " + inds[i].ContinentViolation.ToString());
                var row = new DataGridViewRow();
                for(int j = 0; j < inds[i].Cities.Length; j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = Convert.ToInt16(inds[i].Cities[j])
                    });
                }
                dataGridView2.Rows.Add(row);
            }
            var best = _ga.GetFittestIndividual();
            listBox5.Items.Clear();
            string cities = "";
            foreach(var c in best.TravelOrder)
            {
                cities += c + ", ";
            }
            listBox5.Items.Add(cities);
            listBox5.Items.Add("Fitness: " + best.ObjectiveValue);
            listBox5.Items.Add("Tour Violation: " + best.TourViolation);
            listBox5.Items.Add("Continent Violation: " + best.ContinentViolation);
            listBox5.Items.Add("Num cities: " + best.CountriesVisited);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            int numCities = _dbh.GetCountryCodeIndexes().Count;
            progressBar3.Maximum = 41818;
            progressBar3.Step = 1;
            dataGridView1.Rows.Clear();
            int[,] distMatrix = _dbh.GetDistanceMatrix(progressBar3);
            dataGridView1.ColumnCount = distMatrix.GetLength(1);
            for (int i = 0; i < distMatrix.GetLength(0); i++)
            {
                var row = new DataGridViewRow();
                for(int j = 0; j < distMatrix.GetLength(1); j++)
                {
                    row.Cells.Add(new DataGridViewTextBoxCell()
                    {
                        Value = distMatrix[i, j]
                    });
                }
                dataGridView1.Rows.Add(row);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SysConfig.selectionMethod = comboBox1.Text;
            progressBar2.Maximum = Convert.ToInt32(numericUpDown3.Value);
            backgroundWorker2.RunWorkerAsync(chart1);
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var chart = (Chart)e.Argument;
            // do best ind
            Individual ind = _ga.GetFittestIndividual();
            for (int j = 0; j < numericUpDown3.Value; j++)
            {
                ind = _opt.OptimiseTour(ind, SysConfig.selectionMethod, _dbh);
                backgroundWorker2.ReportProgress((j + 1 ));
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            chart1.Series[0].Points.AddXY(chart1.Series[0].Points.Count, 
                (_ga.GetFittestTour(_ga.GetPopulation()[0].TourPopulation)).Violation);
            listBox7.Items.Clear();
            listBox6.Items.Clear();
            List<Tour> tours = _ga.GetPopulation()[0].TourPopulation;
            for(var i=0;i<tours.Count;i++)
            {
                string cities = "";
                foreach (var s in tours[i].TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox6.Items.Add(cities);
            }
            for(var t=0;t<tours.Count;t++)
            {
                listBox7.Items.Add(tours[t].Violation);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SysConfig.tourPopSize = Convert.ToInt32(numericUpDown1.Value);
            SysConfig.crossOverRate = Convert.ToInt32(numericUpDown2.Value);
            SysConfig.mutationRate = Convert.ToInt32(numericUpDown5.Value);
            SysConfig.maxTourGenerations = Convert.ToInt32(numericUpDown7);
            progressBar1.Maximum = Convert.ToInt32(numericUpDown4.Value);
            List<Individual> inds = _opt.InitialisePopulation(Convert.ToInt32(numericUpDown4.Value), _dbh,progressBar1);
            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart2.ChartAreas[0].AxisY.Minimum = _ga.GetFittestIndividual().ObjectiveValue;
            chart1.Series[0].Points.AddXY(0, (_ga.GetFittestTour(_ga.GetPopulation()[0].TourPopulation)).Violation);
            listBox2.Items.Clear();
            listBox9.Items.Clear();
            Dictionary<string, int> bids = _dbh.GetBids();
            foreach (var i in inds)
            {
                string cities = "";
                foreach (var c in i.TravelOrder)
                {
                    cities += c + ", ";
                }
                listBox2.Items.Add(cities);
            }
            ShowEvaluations();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var bids = _dbh.GetBids();
            foreach(var kvp in bids)
            {
                listBox1.Items.Add(kvp.Key + ": " + kvp.Value);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SysConfig.selectionMethod = comboBox1.Text;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            progressBar4.Maximum = Convert.ToInt32(numericUpDown6.Value);
            backgroundWorker3.RunWorkerAsync(chart2);
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var chart = (Chart)e.Argument;
            for (int j = 0; j < numericUpDown6.Value; j++)
            {
                _opt.OptimiseBidsWithConstraints(_ga.GetPopulation(), _dbh,SysConfig.selectionMethod);
                backgroundWorker3.ReportProgress(j + 1);
            }
        }

        private void backgroundWorker3_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar4.Value = e.ProgressPercentage;
            chart2.Series[0].Points.AddXY(chart2.Series[0].Points.Count,
                _ga.GetFittestIndividual().ObjectiveValue);
            listBox2.Items.Clear();
            List < Individual> pop = _ga.GetPopulation();
            for(var i = 0; i < pop.Count; i++)
            {
                string cities = "";
                foreach (var s in pop[i].TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox2.Items.Add(cities);
            }
            Individual best = _ga.GetFittestIndividual();
            listBox9.Items.Add("Generation " + e.ProgressPercentage + ": " + best.ObjectiveValue.ToString());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            SysConfig.tourPopSize = Convert.ToInt32(numericUpDown1.Value);
            SysConfig.crossOverRate = Convert.ToInt32(numericUpDown2.Value);
            SysConfig.mutationRate = Convert.ToInt32(numericUpDown5.Value);
            SysConfig.maxTourGenerations = Convert.ToInt32(numericUpDown7.Value);
            progressBar1.Maximum = Convert.ToInt32(numericUpDown4.Value);
            List<Individual> inds = _opt.InitialiseDiversePopulation(Convert.ToInt32(numericUpDown4.Value), _dbh, progressBar1);
            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart2.ChartAreas[0].AxisY.Minimum = _ga.GetFittestIndividual().ObjectiveValue;
            chart1.Series[0].Points.AddXY(0, ((Tour)_ga.GetFittestTour(_ga.GetPopulation()[0].TourPopulation)).Violation);
            listBox2.Items.Clear();
            listBox9.Items.Clear();
            Dictionary<string, int> bids = _dbh.GetBids();
            foreach (var i in inds)
            {
                string cities = "";
                foreach (var c in i.TravelOrder)
                {
                    cities += c + ", ";
                }
                listBox2.Items.Add(cities);
            }
            ShowEvaluations();
            bool[] visited = new bool[SysConfig.chromeLength];
            foreach (var i in inds)
            {
                for (var c = 0; c < i.Cities.Length; c++)
                {
                    if (i.Cities[c])
                    {
                        visited[c] = true;
                    }
                }
            }
            int citiesMapped = visited.Where(x => x == true).Count();
            listBox8.Items.Clear();
            listBox8.Items.Add(citiesMapped);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            ShowEvaluations();
        }
    }
}
