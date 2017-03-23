using blueCow.Lib;
using System;
using System.Collections;
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
    public partial class friendlyUi : Form
    {
        public friendlyUi()
        {
            InitializeComponent();
        }

        private void friendlyUi_Load(object sender, EventArgs e)
        {
            // TODO: cette ligne de code charge les données dans la table 'distances_vs2015DataSet.continents'. Vous pouvez la déplacer ou la supprimer selon les besoins.
            this.continentsTableAdapter.Fill(this.distances_vs2015DataSet.continents);

        }

        /// <summary>
        /// Validation function for all min/max textbox
        /// </summary>
        /// I removed the forced focus if wrong, to enable it de-comment e.Cancel = true
        /// <param name="comparaison">0 is minimum, 1 is max</param>
        /// <param name="TextBoxMin">The minimum textbox</param>
        /// <param name="TextBoxMax">The maximum textbox</param>
        /// <param name="type">The type of data, more data can be added easily in dictinnary and ranges</param>
        /// <param name="e">Cancel event arg</param>
        private void minMaxValidation(int comparaison, TextBox TextBoxMin, TextBox TextBoxMax, string type, CancelEventArgs e)
        {
            //initialisation of ranges
            int[] rangeCity = new int[2] { 1, 203 };
            int[] rangeTour = new int[2] { 0, 1000000000 };
            int[] rangeDistance = new int[2] { 0, 1000000000 };
            int[] rangeNA = new int[2] { 1, 23 };
            int[] rangeSA = new int[2] { 1, 12 };
            int[] rangeAF = new int[2] { 1, 54 };
            int[] rangeAS = new int[2] { 1, 44 };
            int[] rangeEU = new int[2] { 1, 47 };
            int[] rangeOC = new int[2] { 1, 14 };
            int[][] allRanges = new int[][] { rangeCity, rangeTour, rangeDistance, rangeNA, rangeSA, rangeAF, rangeAS, rangeEU, rangeOC };

            //other variable
            string errorMsg;
            int tryParse;
            Dictionary<string, int> typeAssociation = new Dictionary<string, int>();
            typeAssociation.Add("city", 0);
            typeAssociation.Add("tour", 1);
            typeAssociation.Add("distance", 2);
            typeAssociation.Add("rangeNA", 3);
            typeAssociation.Add("rangeSA", 4);
            typeAssociation.Add("rangeAF", 5);
            typeAssociation.Add("rangeAS", 6);
            typeAssociation.Add("rangeEU", 7);
            typeAssociation.Add("rangeOC", 8);

            // if 0 then it's minimum else its max
            if (comparaison == 0)
            {
                //if not empty textbox
                if (TextBoxMin.Text != "")
                {
                    //if it can be parsed to an int
                    if (int.TryParse(TextBoxMin.Text, out tryParse))
                    {
                        //if it is lower than the minimum
                        if (int.Parse(TextBoxMin.Text) < allRanges[typeAssociation[type]][0])
                        {
                            errorMsg = "This field can't be lower than " + allRanges[typeAssociation[type]][0];
                            e.Cancel = true;
                            this.errorProvider1.SetError(TextBoxMin, errorMsg);

                        }

                        //if the max is not empty and the min is higher than the max
                        if (TextBoxMax.Text != "" && Int32.Parse(TextBoxMin.Text) > Int32.Parse(TextBoxMax.Text))
                        {
                            errorMsg = "This field can't be higher than the maximun";
                            e.Cancel = true;
                            this.errorProvider1.SetError(TextBoxMin, errorMsg);
                        }
                        //if it's not an int
                    }
                    else
                    {
                        errorMsg = "This field must be a numeric";
                        e.Cancel = true;
                        this.errorProvider1.SetError(TextBoxMin, errorMsg);
                    }
                    //if the field is empty
                }
                else
                {
                    errorMsg = "This field is mandatory";
                    e.Cancel = true;
                    this.errorProvider1.SetError(TextBoxMin, errorMsg);
                }
            }
            else
            {
                //if not empty textbox
                if (TextBoxMax.Text != "" || !int.TryParse(TextBoxMax.Text, out tryParse))
                {
                    //if it can be parsed to an int
                    if (int.TryParse(TextBoxMax.Text, out tryParse))
                    {
                        //if it is higher than the maximum
                        if (int.Parse(TextBoxMax.Text) > allRanges[typeAssociation[type]][1])
                        {
                            errorMsg = "This field can't be higher than " + allRanges[typeAssociation[type]][1];
                            e.Cancel = true;
                            this.errorProvider1.SetError(TextBoxMax, errorMsg);

                        }

                        //if the min is not empty and the max is lower than the min
                        if (TextBoxMin.Text != "" && Int32.Parse(TextBoxMax.Text) < Int32.Parse(TextBoxMin.Text))
                        {
                            errorMsg = "This field can't be lower than the minimum";
                            e.Cancel = true;
                            this.errorProvider1.SetError(TextBoxMax, errorMsg);
                        }
                        //if it's not an int
                    }
                    else
                    {
                        errorMsg = "This field must be a numeric";
                        e.Cancel = true;
                        this.errorProvider1.SetError(TextBoxMax, errorMsg);
                    }
                    //if the field is empty
                }
                else
                {
                    errorMsg = "This field is mandatory";
                    e.Cancel = true;
                    this.errorProvider1.SetError(TextBoxMax, errorMsg);
                }
            }
        }


        /// <summary>
        /// Check if the field is a float
        /// </summary>
        /// <param name="textBoxWeigth">The textBox to check</param>
        /// <param name="e"> Cancel event args</param>
        private void floatValidation(TextBox textBoxWeigth, CancelEventArgs e)
        {
            float tryParse;
            if (!float.TryParse(textBoxWeigth.Text, out tryParse))
            {
                e.Cancel = true;
                this.errorProvider1.SetError(textBoxWeigth, "This need to be a numeric value");
            }
        }

        /// <summary>
        /// OBSOLETE Check the capital per continent field
        /// </summary>
        /// <param name="textBoxContinent">The textBox to check</param>
        /// <param name="continent">the continent (NA,EU,SA,AS,OC,AF)</param>
        /// <param name="e">Cancel event args</param>
        private void maxCapitalValidation(TextBox textBoxContinent, string continent, CancelEventArgs e)
        {
            Dictionary<string, int> continentAssociation = new Dictionary<string, int>();
            continentAssociation.Add("NA", 23);
            continentAssociation.Add("AF", 54);
            continentAssociation.Add("AS", 44);
            continentAssociation.Add("EU", 47);
            continentAssociation.Add("OC", 14);
            continentAssociation.Add("SA", 12);

            int tryParse;
            if (!int.TryParse(textBoxContinent.Text, out tryParse))
            {
                e.Cancel = true;
                this.errorProvider1.SetError(textBoxContinent, "This need to be a numeric value");
            }
            else if (int.Parse(textBoxContinent.Text) > continentAssociation[continent])
            {
                e.Cancel = true;
                this.errorProvider1.SetError(textBoxContinent, "There is only " + continentAssociation[continent] + " capitals in this continent");
            }
            else if (int.Parse(textBoxContinent.Text) < 0)
            {
                e.Cancel = true;
                this.errorProvider1.SetError(textBoxContinent, "There must at least be 1 city per continent");
            }
        }



        // all validators
        private void minCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxCityNumberMin, txtBoxCityNumberMax, "city", e);
        }
        private void minCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxCityNumberMin, "");
        }


        private void maxCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxCityNumberMin, txtBoxCityNumberMax, "city", e);
        }
        private void maxCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxCityNumberMax, "");
        }


        private void minTourValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxTourMin, txtBoxTourMax, "tour", e);
        }
        private void minTourValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxTourMin, "");
        }


        private void maxTourValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxTourMin, txtBoxTourMax, "tour", e);
        }
        private void maxTourValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxTourMax, "");
        }


        private void minDistanceValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxDistanceMin, txtBoxDistanceMax, "distance", e);
        }
        private void minDistanceValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxDistanceMin, "");
        }


        private void maxDistanceValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxDistanceMin, txtBoxDistanceMax, "distance", e);
        }
        private void maxDistanceValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxDistanceMin, "");
        }


       


        private void weigthDistanceValidating(object sender, CancelEventArgs e)
        {
            this.floatValidation(txtBoxDistanceWeigth, e);
        }
        private void weigthDistanceValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxDistanceWeigth, "");
        }


        private void weigthTourValidating(object sender, CancelEventArgs e)
        {
            this.floatValidation(txtBoxTourWeigth, e);
        }
        private void weigthTourValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxTourWeigth, "");
        }


        private void NACityValidating(object sender, CancelEventArgs e)
        {            
            this.minMaxValidation(0, txtBoxNACity, txtBoxNAMax, "rangeNA", e);
        }
        private void NACityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxNACity, "");
        }
        private void NACityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxNACity, txtBoxNAMax, "rangeNA", e);
        }
        private void NACityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxNAMax, "");
        }


        private void EUCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxEUCity, txtBoxEUMax, "rangeEU", e); 
        }
        private void EUCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxEUCity, "");
        }
        private void EUCityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxEUCity, txtBoxEUMax, "rangeNA", e);
        }
        private void EUCityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxEUMax, "");
        }


        private void SACityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxSACity, txtBoxSAMax, "rangeSA", e);            
        }
        private void SACityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxSACity, "");
        }
        private void SACityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxSACity, txtBoxSAMax, "rangeSA", e);
        }
        private void SACityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxSAMax, "");
        }


        private void ASCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxASCity, txtBoxASMax, "rangeAS", e);
        }
        private void ASCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxASCity, "");
        }
        private void ASCityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxASCity, txtBoxASMax, "rangeSA", e);
        }
        private void ASCityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxASMax, "");
        }


        private void OCCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxOCCity, txtBoxOCMax, "rangeOC", e);
        }
        private void OCCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxOCCity, "");
        }
        private void OCCityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxOCCity, txtBoxOCMax, "rangeOC", e);
        }
        private void OCCityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxOCMax, "");
        }


        private void AFCityValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(0, txtBoxAFCity, txtBoxAFMax, "rangeAF", e);
        }
        private void AFCityValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxAFCity, "");
        }
        private void AFCityMaxValidating(object sender, CancelEventArgs e)
        {
            this.minMaxValidation(1, txtBoxAFCity, txtBoxAFMax, "rangeAF", e);
        }
        private void AFCityMaxValidated(object sender, EventArgs e)
        {
            this.errorProvider1.SetError(txtBoxAFMax, "");
        }

        private void btnExceptionAdd_Click(object sender, EventArgs e)
        {
            IEnumerator selectedItems;
            DataGridViewSelectedCellCollection selectedCells = dataGridViewException.SelectedCells;

            selectedItems = selectedCells.GetEnumerator();
            int[] rowIndex = new int[400]; ;
            int nbrItem = 0;
            while (selectedItems.MoveNext())
            {
                rowIndex[nbrItem] = dataGridViewException.SelectedCells[nbrItem].RowIndex;
                nbrItem++;
            }
            lblErrorException.Text = "";

            if (nbrItem > 2)
            {
                lblErrorException.Text += "Exception status: Too many items selected, please select two cities";
            }
            else if (nbrItem == 1)
            {
                lblErrorException.Text = "Exception status: Only one city selected please select another one";
            }
            else if (nbrItem == 0)
            {
                lblErrorException.Text = "Exception status: no city selected";

                //good number of selection
            }
            else
            {
                object exception1 = dataGridViewException.SelectedCells[0].Value;
                object exception2 = dataGridViewException.SelectedCells[1].Value;
                
                object exceptionNum = exception1 + "|" + exception2;

                int rowId = dataGridViewExceptionDisplay.Rows.Add();

                DataGridViewRow row = dataGridViewExceptionDisplay.Rows[rowId];

                row.Cells["exceptionColumn1"].Value = exception1;
                row.Cells["exceptionColumn2"].Value = exception2;
                row.Cells["ExceptionIds"].Value = exceptionNum;
                row.Cells["deleteColumn"].Value = "Delete";
            }

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                dataGridViewExceptionDisplay.Rows.RemoveAt(e.RowIndex);
            }
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            DataGridViewRow row;
            DataRowView city2;
            DataRowView city1;

            IList listBindingSource;

            int count1;
            int count2;
            int tryParse;
            int rowId;

            object exception1;
            object exception2;
            object exceptionNum;

            if (int.TryParse(randomExceptionNumber.Text, out tryParse))
            {
                for (int i = 0; i < Int32.Parse(randomExceptionNumber.Text); i++)
                {
                    count1 = rnd.Next(1, 203);
                    count2 = rnd.Next(1, 203);

                    while (count1 == count2)
                    {
                        count2 = rnd.Next(1, 203);
                    }

                    listBindingSource = continentsBindingSource1.List;

                    city1 = (DataRowView)listBindingSource[count1];
                    city2 = (DataRowView)listBindingSource[count2];

                    exception1 = city1["ida"];
                    exception2 = city2["ida"];
                    exceptionNum = city1["ida"] + "|" + city2["ida"];

                    rowId = dataGridViewExceptionDisplay.Rows.Add();

                    row = dataGridViewExceptionDisplay.Rows[rowId];

                    row.Cells["exceptionColumn1"].Value = exception1;
                    row.Cells["exceptionColumn2"].Value = exception2;
                    row.Cells["ExceptionIds"].Value = exceptionNum;
                    row.Cells["deleteColumn"].Value = "Delete";
                }

            }

        }

        private void btnRunSalesman_Click(object sender, EventArgs e)
        {
            //all minimum city to visit
            SysConfig.africanCityLimit = Int32.Parse(txtBoxAFCity.Text);
            SysConfig.europeanCityLimit = Int32.Parse(txtBoxEUCity.Text);
            SysConfig.namericaCityLimit = Int32.Parse(txtBoxNACity.Text);
            SysConfig.samericanCityLimit = Int32.Parse(txtBoxSACity.Text);
            SysConfig.asiaCityLimit = Int32.Parse(txtBoxASCity.Text);
            SysConfig.oceaniaCityLimit = Int32.Parse(txtBoxOCCity.Text);

            SysConfig.africanCityLimitMax = Int32.Parse(txtBoxAFMax.Text);
            SysConfig.europeanCityLimitMax = Int32.Parse(txtBoxEUMax.Text);
            SysConfig.namericaCityLimitMax = Int32.Parse(txtBoxNAMax.Text);
            SysConfig.oceaniaCityLimitMax = Int32.Parse(txtBoxOCMax.Text);
            SysConfig.asiaCityLimitMax = Int32.Parse(txtBoxASMax.Text);
            SysConfig.samericanCityLimitMax = Int32.Parse(txtBoxSAMax.Text);

            //number of city for a valid tour
            int minimumCityNumber = Int32.Parse(txtBoxCityNumberMin.Text);
            int maximumCityNumber = Int32.Parse(txtBoxCityNumberMax.Text);

            if (SysConfig.africanCityLimitMax + SysConfig.europeanCityLimitMax + SysConfig.namericaCityLimitMax + SysConfig.oceaniaCityLimitMax + SysConfig.asiaCityLimitMax + SysConfig.samericanCityLimitMax < minimumCityNumber)
            {
                lblWarning.Text = "The combined maximum city per continent is lower than the minimum city required for a valid tour";
            }else if(SysConfig.africanCityLimit + SysConfig.europeanCityLimit + SysConfig.namericaCityLimit + SysConfig.samericanCityLimit + SysConfig.asiaCityLimit + SysConfig.oceaniaCityLimit > maximumCityNumber)
            {
                lblWarning.Text = "The combined minimul city per continent is higher than the maximum city required for a valid tour";
            }else
            {
                lblWarning.Text = "";
                long continentWeigth = long.Parse(txtBoxContinentWeigth.Text);


                //float weigthCityNumber = float.Parse(txtBoxCityNumberWeigth.Text);

                //distance for a valid tour
                int minimumTourLength = Int32.Parse(txtBoxTourMin.Text);
                int maximumTourLength = Int32.Parse(txtBoxTourMax.Text);
                long weigthTourLength = long.Parse(txtBoxTourWeigth.Text);

                //distance between cities
                int minimumDistance = Int32.Parse(txtBoxDistanceMin.Text);
                int maximumDistance = Int32.Parse(txtBoxDistanceMax.Text);
                long weigthDistance = long.Parse(txtBoxDistanceWeigth.Text);

                long exceptionWeigth = long.Parse(txtBoxExceptionWeigth.Text);

                string[] stringSeparators = new string[] { "|" };
                List<string[]> illegalHops = new List<string[]>();
                string[] idArrayCombo;

                for (int i = 0; i < dataGridViewExceptionDisplay.RowCount; i++)
                {
                    string valueId = dataGridViewExceptionDisplay[2, i].Value.ToString();
                    idArrayCombo = new string[2];
                    idArrayCombo = valueId.Split(stringSeparators, StringSplitOptions.None);
                    illegalHops.Add(idArrayCombo);
                }

                SysConfig.minCities = minimumCityNumber;
                SysConfig.maxCities = maximumCityNumber;
                SysConfig.maxTotalDist = maximumTourLength;
                SysConfig.minTotalDist = minimumTourLength;
                SysConfig.maxHopDist = maximumDistance;
                SysConfig.minHopDist = minimumDistance;
                SysConfig.illegalHops = illegalHops;

                SysConfig.illegalHopePenalty = exceptionWeigth;
                SysConfig.hopDistPenalty = weigthDistance;
                SysConfig.totalDistPenalty = weigthTourLength;
                SysConfig.continentPenalty = continentWeigth;

                advancedUI advancedSettings = new advancedUI();
                advancedSettings.Show();
            }           

        }
    }
      
}
