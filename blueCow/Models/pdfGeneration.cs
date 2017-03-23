using blueCow.Lib;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace blueCow.Models
{
    class pdfGeneration
    {
        
        public virtual Document generatePdfParameters(string poptype, decimal parentPopSize)
        {
            
            Document doc = new Document(PageSize.A4);
            string fileName = SysConfig.pdfPath;
            fileName = fileName + "\\ResultReport_" + DateTime.Now.ToString("HH-mm-ss_dd-MM-yyyy") + ".pdf";
            Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            var output = new FileStream(fileName, FileMode.Create);
            var writer = PdfWriter.GetInstance(doc, output);

            PdfPTable tableMainParam = createTable(4);           
            PdfPTable tableContinent = createTable(4);
            PdfPTable tableBannedTravel = createTable(1);
                        
            PdfPTable tableGenetic = createTable(2);           
          
        
            string bannedList = "";
            foreach (string[] pair in SysConfig.illegalHops)
                bannedList += pair[0] + "-" + pair[1] + "    ";

            string[] line0 = new string[] { "!header", "Main Parameters", "Minimum Value", "Maximum Value", "Penalty" };
            string[] line1 = new string[] { "City Range for a valid tour", SysConfig.minCities.ToString(), SysConfig.maxCities.ToString(), "None" };
            string[] line2 = new string[] { "Total distance a valid tour", SysConfig.minTotalDist.ToString(), SysConfig.maxTotalDist.ToString(), SysConfig.totalDistPenalty.ToString() };
            string[] line3 = new string[] { "Distance between cities for a valid hop", SysConfig.minHopDist.ToString(), SysConfig.maxHopDist.ToString(), SysConfig.hopDistPenalty.ToString() };
            string[][] allLines1 = new string[][] { line0, line1, line2, line3};

            string[] lineGen = new string[] { "!merge", "Parameters of genetic algorythm|2" };
            string[] linePop = new string[] { "type of generation", poptype };
            string[] lineSize = new string[] { "Parent pop size", parentPopSize.ToString() };
            string[] lineTour = new string[] { "Tour Pop size", SysConfig.tourPopSize.ToString() };
            string[] lineCross = new string[] { "Crossover rate", SysConfig.crossOverRate.ToString() };
            string[] lineMuta = new string[] { "Mutation rate", SysConfig.mutationRate.ToString() };
            string[] lineOpti = new string[] { "Tour optimisation", SysConfig.maxTourGenerations.ToString() };
            string[][] allLinesGenet = new string[][] { lineGen, linePop, lineSize, lineTour, lineCross, lineMuta, lineOpti };


            string[] line4 = new string[] { "!merge", "Continents|4" };
            string[] line5 = new string[] { "Cities required in North America", SysConfig.namericaCityLimit.ToString(), SysConfig.namericaCityLimitMax.ToString(), SysConfig.continentPenalty.ToString() };
            string[] line6 = new string[] { "Cities required in South America", SysConfig.samericanCityLimit.ToString(), SysConfig.samericanCityLimitMax.ToString(), "same" };
            string[] line7 = new string[] { "Cities required in Europe", SysConfig.europeanCityLimit.ToString(), SysConfig.europeanCityLimitMax.ToString(), "same" };
            string[] line8 = new string[] { "Cities required in Africa", SysConfig.africanCityLimit.ToString(), SysConfig.africanCityLimitMax.ToString(), "same" };
            string[] line9 = new string[] { "Cities required in Asia", SysConfig.asiaCityLimit.ToString(), SysConfig.asiaCityLimitMax.ToString(), "same" };
            string[] line10 = new string[] { "Cities required in Oceania", SysConfig.oceaniaCityLimit.ToString(), SysConfig.oceaniaCityLimitMax.ToString(), "same" };
            string[][] allLines2 = new string[][] { line4, line5, line6, line7, line8, line9, line10 };

            string[] line11 = new string[] {"!header", "Banned travel" };
            string[] line12 = new string[] { bannedList };   
            string[][] allLines3 = new string[][] { line11, line12 };
            
            tableMainParam = tableGenerator(tableMainParam, allLines1);
            tableContinent = tableGenerator(tableContinent, allLines2);
            tableBannedTravel = tableGenerator(tableBannedTravel, allLines3);
            
            tableGenetic = tableGenerator(tableGenetic, allLinesGenet); 

            doc.Open();
            doc.AddTitle("The best pdf");
            var TitleFont = FontFactory.GetFont("Arial", 16, BaseColor.BLACK);
            var paraFont = FontFactory.GetFont("Arial", 11, BaseColor.BLACK);
            var normalFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
            doc.Add(new Paragraph("Generated report", TitleFont));
            doc.Add(new Paragraph(" "));
            doc.Add(new Paragraph("Parameters used in the last run :", paraFont));
            doc.Add(new Paragraph(" "));
            doc.Add(tableMainParam);
            doc.Add(new Paragraph(" "));
            doc.Add(tableContinent);
            doc.Add(new Paragraph(" "));
            doc.Add(tableBannedTravel);
            doc.Add(new Paragraph(" "));
            doc.Add(tableGenetic);
            doc.NewPage();
            doc.Add(new Paragraph("Generated Result", TitleFont));

            return doc;
            

        }

        public virtual Document generatePdfResult(Document doc,string[] bestResultLines, string stage, string[] optimizeParams, iTextSharp.text.Image pic)
        {            
            var normalFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
            var paraFont = FontFactory.GetFont("Arial", 11, BaseColor.BLACK);
            doc.Add(new Paragraph("Best result "+ stage + " :", paraFont));
            doc.Add(new Paragraph(" "));

            if (optimizeParams != null)
            {
                PdfPTable tableSummary = createTable(2);
                string[] lineHead = new string[] { "!merge", "Parameters of optimisation|2" };
                string[] lineSel = new string[] { "Selection Method", optimizeParams[0].ToString() };
                string[] lineRep = new string[] { "Replacement Method", optimizeParams[1].ToString() };
                string[] lineTou = new string[] { "Tournament size", optimizeParams[2].ToString() };
                string[] lineIte = new string[] { "Number of iteration", optimizeParams[3].ToString() };
                string[][] allLines = new string[][] { lineHead, lineSel, lineRep, lineTou, lineIte };
                tableSummary = tableGenerator(tableSummary, allLines);
                doc.Add(tableSummary);
                doc.Add(new Paragraph(" "));
                doc.Add(pic);
            }
            int cpt = 0;
            foreach (string line in bestResultLines)
            {
                cpt++;
                if(cpt == 1)
                {
                    doc.Add(new Paragraph("Route taken : "+line, normalFont));
                }                   
                else
                {
                    doc.Add(new Paragraph(line, normalFont));
                }
            }            
            doc.Add(new Paragraph("____________________________________________________________________________"));
            doc.Add(new Paragraph(" "));
            return doc;
        }

        public virtual Document generateListFitness(Document doc, ListBox fitnessListBox)
        {
            var paraFont = FontFactory.GetFont("Arial", 11, BaseColor.BLACK);
            var normalFont = FontFactory.GetFont("Arial", 11, BaseColor.BLACK);
            doc.Add(new Paragraph("All fitness of optimization :", paraFont));
            doc.Add(new Paragraph(" "));
            foreach (string line in fitnessListBox.Items)
            {
                doc.Add(new Paragraph(line, normalFont));
            }
            return doc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <param name="allLines"></param>
        /// <returns></returns>
        private PdfPTable tableGenerator(PdfPTable table, string[][] allLines)
        {
            string[] stringSeparators = new string[] { "|" };
            int defaultPadding = 5;
            var defaultFont = FontFactory.GetFont("Arial", 9, BaseColor.BLACK);
            int headerPadding = 10;
            var headerFont = FontFactory.GetFont("Arial", 12, BaseColor.BLACK);            
            var mergedFont = FontFactory.GetFont("Arial", 12, BaseColor.BLACK);
            foreach (string[] line in allLines)
            {
                foreach (string cellContent in line)
                {

                    if (line[0] == "!merge")
                    {
                        if (cellContent != "!merge")
                        {

                            string[] text = line[1].Split(stringSeparators, StringSplitOptions.None);
                            PdfPCell cell = new PdfPCell();
                            cell.Colspan = Int32.Parse(text[1]);
                            cell.AddElement(new Paragraph(text[0], mergedFont));
                            cell.VerticalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = defaultPadding;
                            table.AddCell(cell);
                        }
                    }
                    else if (line[0] == "!header")
                    {
                        if (cellContent != "!header")
                        {
                            PdfPCell cell = new PdfPCell();
                            cell.AddElement(new Paragraph(cellContent, headerFont));
                            cell.VerticalAlignment = Element.ALIGN_CENTER;
                            cell.Padding = headerPadding;
                            table.AddCell(cell);
                        }
                    }
                    else
                    {
                       
                        PdfPCell cell = new PdfPCell();
                        
                        cell.AddElement(new Paragraph(cellContent, defaultFont));
                        cell.VerticalAlignment = Element.ALIGN_CENTER;
                        cell.Padding = defaultPadding;
                        table.AddCell(cell);
                    }
                    
                }
            }
            return table;
        }


        private PdfPTable createTable(int nbrColumn)
        {
            PdfPTable table = new PdfPTable(nbrColumn);
            table.DefaultCell.Border = 1;
            table.WidthPercentage = 100;
            return table;
        }
    }
}


