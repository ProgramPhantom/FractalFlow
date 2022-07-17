using System;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Serialization;
using FormulaParser;


namespace TestingZone
{


    public class Program
    {
        static void Main(string[] args)
        {
            using (SqlConnection sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB2;AttachDbFilename=""C: \Users\henry\OneDrive - Xaverian College\Computer Science\NEA\FractalFlow\FractalGeneratorMVVM\Database\FractalFlowDB.mdf"";Integrated Security=True"))
            {
                sqlConnection.Open();

               

                
                
                

            }
        }
    }
}
