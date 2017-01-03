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

        public Form1()
        {
            InitializeComponent();
            _dbh = new DatabaseHelper();
            _ga = new GeneticAlgorithm();
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
            List<Individual> inds = _ga.EvaluateTourDistances(obj.TourDistance);
            listBox4.Items.Clear();
            foreach(var i in inds)
            {
                listBox4.Items.Add(i.TourDistance.ToString());
            }
        }
    }
}
