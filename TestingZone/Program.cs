using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using FormulaParser;
using System.Windows;



namespace TestingZone
{


    public class Program
    {
        static void Main(string[] args)
        {
            string test = @"

## FRACTALFRAME
Left: -3.1
Right: 1
Bottom: -1.2
Top: 1.2
Iterations: 100
Bail: 2
## ITERATOR
FormulaString: z^2 + c
## BASICPAINTER
InSetColour: #FFFFFFFF
MainColour: #AAAAAAAA
Type: 1";

            test = test.Trim();
            List<string> lines = test.Split("\n").ToList();
            

            // Remove whitespace
            for (int l = 0; l < lines.Count; l++)
            {
                lines[l] = Regex.Replace(lines[l], @"\s+", "");
            }

            lines.ForEach(o => Console.WriteLine(o));

            #region Fractal Frame Parse
            int ffDefLines = 6;
            int ffIndex = lines.IndexOf("##FRACTALFRAME");
            List<string> ffDef = new List<string>();
            for (int index = ffIndex + 1; index < ffIndex + ffDefLines + 1; index++)
            {
                ffDef.Add(lines[index]);
            }

            Dictionary<string, string> ffDic = new Dictionary<string, string>();
            foreach (string defLine in ffDef)
            {
                string propName = defLine.Split(":")[0];
                string val = defLine.Split(":")[1];
                ffDic.Add(propName, val);
            }

            float left = float.Parse(ffDic["Left"]);
            float right = float.Parse(ffDic["Right"]);
            float top = float.Parse(ffDic["Top"]);
            float bottom = float.Parse(ffDic["Bottom"]);
            uint iterations = uint.Parse(ffDic["Iterations"]);
            int bail = int.Parse(ffDic["Bail"]);

            // FractalFrame fractalFrame = new FractalFrame(asdfsdf)
            #endregion

            #region Iterator Parse
            int iterDefLines = 1;
            int iterIndex = lines.IndexOf("##ITERATOR");
            List<string> iterDef = new List<string>();
            for (int index = iterIndex + 1; index < iterIndex + iterDefLines + 1; index++)
            {
                iterDef.Add(lines[index]);
            }

            Dictionary<string, string> iterDic = new Dictionary<string, string>();
            foreach (string defLine in iterDef)
            {
                string propName = defLine.Split(":")[0];
                string val = defLine.Split(":")[1];
                iterDic.Add(propName, val);
            }

            string formulaString = iterDic["FormulaString"];
            #endregion

            #region Painter
            if (lines.Contains("##BASICPAINTER"))
            {
                // This fractal is using a basic painter
                int bpDefLines = 3;
                int bpIndex = lines.IndexOf("##BASICPAINTER");
                List<string> bpDef = new List<string>();
                for (int index = bpIndex + 1; index < bpIndex + bpDefLines + 1; index++)
                {
                    bpDef.Add(lines[index]);
                }

                Dictionary<string, string> bpDic = new Dictionary<string, string>();
                foreach (string defLine in bpDef)
                {
                    string propName = defLine.Split(":")[0];
                    string val = defLine.Split(":")[1];
                    bpDic.Add(propName, val);
                }

                string inSetColour = bpDic["InSetColour"];
                
                string mainColour = bpDic["MainColour"];

                bool type = (bpDic["Type"] == "1");
            }

            #endregion

        }
    }
}
