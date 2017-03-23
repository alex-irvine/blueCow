using blueCow.Lib;
using blueCow.Models;
using iTextSharp.text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace blueCow
{
    public partial class advancedUI : Form
    {
        private DatabaseHelper _dbh;
        private GeneticAlgorithm _ga;
        private OptimisationController _opt;

        //variable for pdf
        private string typeOfPop;
        private List<string> beforeOptimize = new List<string>();
        private List<string[]> optimizeParameter = new List<string[]>();
        private List<iTextSharp.text.Image> allGraph = new List<iTextSharp.text.Image>();
        private Document report;
        private List<string[]> afterOptimize = new List<string[]>();

       

        public advancedUI()
        {
            InitializeComponent();
            _dbh = new DatabaseHelper();
            _ga = new GeneticAlgorithm();
            _opt = new OptimisationController(_ga,new ObjectiveFunction());
        }

        //generate Bid
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

        //show Bids
        private void button2_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            var bids = _dbh.GetBids();
            foreach (var kvp in bids)
            {
                listBox1.Items.Add(kvp.Key + ": " + kvp.Value);
            }
        }

        //get distance matrix
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

        //Initialize the population
        private void button9_Click(object sender, EventArgs e)
        {
            typeOfPop = "Generate Pop Quick";
            SysConfig.tourPopSize = Convert.ToInt32(numericUpDown1.Value);
            SysConfig.crossOverRate = Convert.ToInt32(numericUpDown2.Value);
            SysConfig.mutationRate = Convert.ToInt32(numericUpDown5.Value);
            SysConfig.stepSize = Convert.ToInt32(numericUpDown9.Value);
            SysConfig.maxTourGenerations = Convert.ToInt32(numericUpDown7.Value);
            progressBar1.Maximum = Convert.ToInt32(numericUpDown4.Value);
            List<Individual> inds = _opt.InitialisePopulation(Convert.ToInt32(numericUpDown4.Value), _dbh, progressBar1);
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
            //pdf info storing
            ListBox Holder = null;
            Holder = ShowEvaluations();
            foreach (string line in Holder.Items)
            {
                beforeOptimize.Add(line);
            }
        }
               
        //Initialize a diverse population I guess?
        private void button3_Click(object sender, EventArgs e)
        {
            typeOfPop = "Generate Pop Diverse";
            SysConfig.tourPopSize = Convert.ToInt32(numericUpDown1.Value);
            SysConfig.crossOverRate = Convert.ToInt32(numericUpDown2.Value);
            SysConfig.mutationRate = Convert.ToInt32(numericUpDown5.Value);
            SysConfig.stepSize = Convert.ToInt32(numericUpDown9.Value);
            SysConfig.maxTourGenerations = Convert.ToInt32(numericUpDown7.Value);
            progressBar1.Maximum = Convert.ToInt32(numericUpDown4.Value);
            List<Individual> inds = _opt.InitialiseDiversePopulation(Convert.ToInt32(numericUpDown4.Value), _dbh, progressBar1);
            chart1.Series[0].Points.Clear();
            chart1.ChartAreas[0].AxisX.Minimum = 0;
            chart2.ChartAreas[0].AxisY.IsStartedFromZero = false;
            chart2.ChartAreas[0].AxisY.Minimum = _ga.GetFittestIndividual().ObjectiveValue;
            chart2.Series[0].Points.AddXY(0, _ga.GetFittestIndividual().ObjectiveValue);
            chart1.Series[0].Points.AddXY(0, ((Tour)_ga.GetFittestTour(_ga.GetPopulation()[0].TourPopulation)).Violation);
            listBox2.Items.Clear();
            listBox9.Items.Clear();
            Dictionary<string, int> bids = _dbh.GetBids();
            var sortedDict = (from entry in bids orderby entry.Value descending select entry)
                .ToDictionary(pair => pair.Key, pair => pair.Value).Take(SysConfig.maxCities);
            int maxPossibleBids = sortedDict.Sum(x => x.Value);
            listBox10.Items.Clear();
            listBox10.Items.Add(maxPossibleBids);
            foreach (var i in inds)
            {
                string cities = "";
                foreach (var c in i.TravelOrder)
                {
                    cities += c + ", ";
                }
                listBox2.Items.Add(cities);
            }

            //pdf info storing
            ListBox Holder = null;
            Holder = ShowEvaluations();
            foreach (string line in Holder.Items)
                beforeOptimize.Add(line);
            

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

        //Optimize fitness
        private void button4_Click(object sender, EventArgs e)
        {
            SysConfig.selectionMethod = comboBox1.Text;
            SysConfig.replacementMethod = comboBox2.Text;
            chart2.ChartAreas[0].AxisX.Minimum = 0;
            progressBar4.Maximum = Convert.ToInt32(numericUpDown6.Value);
            backgroundWorker3.RunWorkerAsync(chart2);
            
        }

        //force display of the last value, show value and store all info for the pdf
        private void button5_Click(object sender, EventArgs e)
        {
            ListBox Holder = null;
            Holder = ShowEvaluations();
            List<string> tempHolder = new List<string>();
            foreach (string line in Holder.Items)
                tempHolder.Add(line);                
           
            afterOptimize.Add(tempHolder.ToArray());                       
            string[] temp = { SysConfig.selectionMethod, SysConfig.replacementMethod, Convert.ToInt32(numericUpDown8.Value).ToString(), Convert.ToInt32(numericUpDown6.Value).ToString() };
            optimizeParameter.Add(temp);            
            var chartimage = new MemoryStream();            
            chart2.SaveImage(chartimage, ChartImageFormat.Png);
            iTextSharp.text.Image picture = iTextSharp.text.Image.GetInstance(chartimage.GetBuffer());
            allGraph.Add(picture);

        }


        //Optimize tour (matrix)
        private void button8_Click(object sender, EventArgs e)
        {
            SysConfig.selectionMethod = comboBox1.Text;
            SysConfig.replacementMethod = comboBox2.Text;
            SysConfig.stepSize = Convert.ToInt32(numericUpDown9.Value);
            progressBar2.Maximum = Convert.ToInt32(numericUpDown3.Value);
            backgroundWorker2.RunWorkerAsync(chart1);
        }

        //added a listbox return to retrieve result during runtime at different stages (before optimisation/ after)
        private ListBox ShowEvaluations()
        {
            ObjectiveFunction obj = new ObjectiveFunction();
            List<Individual> inds = _ga.EvaluatePopulation(obj.Evaluate, _dbh);
            listBox3.Items.Clear();
            listBox4.Items.Clear();
            dataGridView2.Rows.Clear();
            dataGridView2.ColumnCount = SysConfig.chromeLength;
            
            for (var i = 0; i < inds.Count; i++)
            {
                listBox3.Items.Add(inds[i].ObjectiveValue.ToString());
                listBox4.Items.Add("Tour: " + inds[i].TourViolation.ToString() + " Continent: " + inds[i].ContinentViolation.ToString());
                var row = new DataGridViewRow();
                for (int j = 0; j < inds[i].Cities.Length; j++)
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
            foreach (var c in best.TravelOrder)
            {
                cities += c + ", ";
            }

            listBox5.Items.Add(cities);
            listBox5.Items.Add("Fitness: " + best.ObjectiveValue);
            listBox5.Items.Add("Tour Violation: " + best.TourViolation);
            listBox5.Items.Add("Continent Violation: " + best.ContinentViolation);
            listBox5.Items.Add("Num cities: " + best.CountriesVisited);
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
            return listBox5;
        }

        private void backgroundWorker2_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var chart = (Chart)e.Argument;
            // do best ind
            Individual ind = _ga.GetFittestIndividual();
            for (int j = 0; j < numericUpDown3.Value; j++)
            {
                ind = _opt.OptimiseTour(ind, SysConfig.selectionMethod, _dbh, Convert.ToInt32(numericUpDown8.Value));
                backgroundWorker2.ReportProgress((j + 1));
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
            for (var i = 0; i < tours.Count; i++)
            {
                string cities = "";
                foreach (var s in tours[i].TravelOrder)
                {
                    cities += s + ", ";
                }
                listBox6.Items.Add(cities);
            }
            for (var t = 0; t < tours.Count; t++)
            {
                listBox7.Items.Add(tours[t].Violation);
            }
        }

        private void backgroundWorker3_DoWork(object sender, DoWorkEventArgs e)
        {
            var worker = (BackgroundWorker)sender;
            var chart = (Chart)e.Argument;
            for (int j = 0; j < numericUpDown6.Value; j++)
            {
                _opt.OptimiseBidsWithConstraints(_ga.GetPopulation(), _dbh,SysConfig.selectionMethod, Convert.ToInt32(numericUpDown8.Value));
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

        private void button6_Click(object sender, EventArgs e)
        {
            _dbh.SortOutTheGoddamMissingCountryCodesRandomly(new Random());
        }

        //on key down in a list box all item will be copied so you can paste them elsewhere, can be deleted as we have the report now
        public void CopyListBox(ListBox list)
        {

            StringBuilder sb = new StringBuilder();
            foreach (string item in list.SelectedItems)
            {
                sb.Append(item + ",\n");
            }

            Clipboard.SetDataObject(sb.ToString());

        }

        private void copyPastaPopulation(object sender, KeyEventArgs e)
        {
            CopyListBox(listBox2);
        }

        private void copyPastaFitness(object sender, KeyEventArgs e)
        {
            CopyListBox(listBox3);
        }

        private void copyPastaConstraintViol(object sender, KeyEventArgs e)
        {
            CopyListBox(listBox4);
        }

        private void copyPastaBest(object sender, KeyEventArgs e)
        {
            CopyListBox(listBox5);
        }

        private void copyPastaBestGene(object sender, KeyEventArgs e)
        {
            CopyListBox(listBox9);
        }

        //pdf gen
        private void btnPdf_Click(object sender, EventArgs e)
        {
            var dResult = saveFileDialog1.ShowDialog();
            SysConfig.pdfPath = Path.GetDirectoryName(saveFileDialog1.FileName);
            if (dResult == DialogResult.Cancel) { return; }
            pdfGeneration pdfDoc = new pdfGeneration();
            report = pdfDoc.generatePdfParameters(typeOfPop, numericUpDown4.Value);

            if(beforeOptimize != null)
            {
                report = pdfDoc.generatePdfResult(report, beforeOptimize.ToArray(), "before optimisation",null,null);
            }                     

            if (afterOptimize.Count != 0)
            {
                int cpt = 0;                
                foreach (string[] result in afterOptimize)
                {
                    int tempValue = cpt;
                    cpt++;
                    report = pdfDoc.generatePdfResult(report, result, "After " + cpt + " optimisation cycle", optimizeParameter.ElementAt(tempValue), allGraph.ElementAt(tempValue));
                    
                }
            }
            report = pdfDoc.generateListFitness(report, listBox9);
            report.Close();
                        
        }
    }
}
