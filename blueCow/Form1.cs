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

        private void button2_Click(object sender, EventArgs e)
        {
            List<Individual> inds = _ga.GeneratePopulation(Convert.ToInt32(numericUpDown1.Value));
            listBox2.Items.Clear();
            Dictionary<string, int> bids = _dbh.GetBids();
            foreach(var i in inds)
            {
                string cities = "";
                foreach(var c in i.TravelOrder)
                {
                    cities += c +", ";
                }
                listBox2.Items.Add(cities);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ObjectiveFunction obj = new ObjectiveFunction();
            List<Individual> inds = _ga.EvaluatePopulation(obj.Evaluate);
            listBox3.Items.Clear();
            foreach(var i in inds)
            {
                listBox3.Items.Add(i.ObjectiveValue.ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ObjectiveFunction obj = new ObjectiveFunction();
            List<Individual> inds = _ga.EvaluateTourViolations(obj.TourViolation);
            listBox4.Items.Clear();
            foreach(var i in inds)
            {
                listBox4.Items.Add(i.TourViolation.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // show first individuals tours
            listBox6.Items.Clear();
            List<Tour> tours = _ga.GetPopulation()[0].TourPopulation;
            foreach(var i in tours)
            {
                string cities = "";
                foreach(var s in i.TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox6.Items.Add(cities);
            }
            listBox5.Items.Clear();
            Tour selectedTour = _ga.RouletteSelectTour(tours);
            string selected = "";
            foreach(var s in selectedTour.TravelOrder)
            {
                selected += s + ", ";
            }
            listBox5.Items.Add(selected);
            listBox5.Items.Add(selectedTour.Violation);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            // just do first ind
            Individual ind = _ga.GetPopulation()[0];
            for (int j = 0; j < numericUpDown2.Value; j++)
            {
                ind = _opt.OptimiseTour(ind, "tournament");
                backgroundWorker1.ReportProgress((j+1 / Convert.ToInt32(numericUpDown2.Value))*100);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
            listBox7.Items.Clear();
            listBox6.Items.Clear();
            List<Tour> tours = _ga.GetPopulation()[0].TourPopulation;
            foreach (var i in tours)
            {
                string cities = "";
                foreach (var s in i.TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox6.Items.Add(cities);
            }
            foreach (var t in tours)
            {
                listBox7.Items.Add(t.Violation);
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            listBox8.Items.Clear();
            int[,] distMatrix = _dbh.GetDistanceMatrix();
            for(int i = 0; i < distMatrix.GetLength(0); i++)
            {
                string row = "";
                for(int j=0;j< distMatrix.GetLength(0); j++)
                {
                    row += "  " + distMatrix[i, j].ToString() + "  ,";
                }
                listBox8.Items.Add(row);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            backgroundWorker2.RunWorkerAsync();
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            // just do first ind
            Individual ind = _ga.GetPopulation()[0];
            for (int j = 0; j < numericUpDown3.Value; j++)
            {
                ind = _opt.OptimiseTour(ind, "tournament",_dbh);
                backgroundWorker2.ReportProgress((j + 1 / Convert.ToInt32(numericUpDown3.Value)) * 100);
            }
        }

        private void backgroundWorker2_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar2.Value = e.ProgressPercentage;
            listBox7.Items.Clear();
            listBox6.Items.Clear();
            List<Tour> tours = _ga.GetPopulation()[0].TourPopulation;
            foreach (var i in tours)
            {
                string cities = "";
                foreach (var s in i.TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox6.Items.Add(cities);
            }
            foreach (var t in tours)
            {
                listBox7.Items.Add(t.Violation);
            }
        }
    }
}
