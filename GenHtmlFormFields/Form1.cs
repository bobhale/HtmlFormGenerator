using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.IO;
using System.Text.RegularExpressions;

namespace GenHtmlFormFields
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string strAccessConn = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=data\\risdirectorypring_2.mdb";

            string strAccessSelect = "SELECT * FROM arr_ther";
 
            // Create the dataset and add the Categories table to it:
            DataSet myDataSet = new DataSet();
            OleDbConnection myAccessConn = null;
            try
            {
                  myAccessConn = new OleDbConnection(strAccessConn);
            }
            catch(Exception ex)
            {
                  Console.WriteLine("Error: Failed to create a database connection. \n{0}", ex.Message);
                  return;
            }
 
            try
            {
            
                  OleDbCommand myAccessCommand = new OleDbCommand(strAccessSelect,myAccessConn);
                  OleDbDataAdapter myDataAdapter = new OleDbDataAdapter(myAccessCommand);
 
                  myAccessConn.Open();
                  myDataAdapter.Fill(myDataSet,"Thers");
 
            }
            catch (Exception ex)
            {
                  Console.WriteLine("Error: Failed to retrieve the required data from the DataBase.\n{0}", ex.Message);
                  return;
            }
            finally
            {
                  myAccessConn.Close();
            }
 
            // A dataset can contain multiple tables, so let's get them
            // all into an array:
            DataTableCollection dta = myDataSet.Tables;
            foreach (DataTable dt in dta)
            {
            Console.WriteLine("Found data table {0}", dt.TableName);
            }
          
            // The next two lines show two different ways you can get the
            // count of tables in a dataset:
            Console.WriteLine("{0} tables in data set", myDataSet.Tables.Count);
            Console.WriteLine("{0} tables in data set", dta.Count);
            // The next several lines show how to get information on
            // a specific table by name from the dataset:
            Console.WriteLine("{0} rows in Categories table", myDataSet.Tables["arr_ther"].Rows.Count);
            // The column info is automatically fetched from the database,
            // so we can read it here:
            Console.WriteLine("{0} columns in Categories table", myDataSet.Tables["Categories"].Columns.Count);
            DataColumnCollection drc = myDataSet.Tables["Categories"].Columns;
            int i = 0;
            foreach (DataColumn dc in drc)
            {
                  // Print the column subscript, then the column's name
                  // and its data type:
                  Console.WriteLine("Column name[{0}] is {1}, of type {2}",i++ , dc.ColumnName, dc.DataType);
            }
            DataRowCollection dra = myDataSet.Tables["Categories"].Rows;
            foreach (DataRow dr in dra)
            {
                  // Print the CategoryID as a subscript, then the CategoryName:
                  Console.WriteLine("CategoryName[{0}] is {1}", dr[0], dr[1]);
            }
      
   }

                    
        private void button2_Click(object sender, EventArgs e)
        {

            StreamWriter file = new System.IO.StreamWriter(@"formoutput.txt");

       OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=risdirectoryprint_2.mdb");
            
       OleDbCommand custCMD = new OleDbCommand( "select * from tbl_thersforweb", connection);
  
             connection.Open();

OleDbDataReader custReader = custCMD.ExecuteReader();


  //Console.WriteLine("Orders for " + custReader.GetString(1)); 
  // custReader.GetString(1) = CompanyName
string tester = string.Empty;

while(custReader.Read())
{
    
  string category = custReader.GetString(0).ToLower();
  if (category != tester) file.WriteLine(setOtherFieldSection(category));
   tester = category;
  string therapy = custReader.GetString(2); 
  // custReader.GetValue(2) = Orders chapter as DataReader

    file.Write(setFormatHtml(category,therapy));

  //Console.WriteLine(category +  " This is it ");
 }
// Make sure to always close readers and connections.
custReader.Close();

          

       

        }

        private string setFormatHtml(string category, string therapy)
        {
            string htmlName = category.ToLower() + therapy.Substring(0,6);
            htmlName = Regex.Replace(htmlName, @"\s+", "");
            string htmlModel = "org." + htmlName;
            string htmlNameCnt = htmlName + "_cnt";
            string htmlModelCnt = htmlModel + "_cnt";

            StringBuilder sb = new StringBuilder();

            sb.Append( "<div class=\"form-group\">" + Environment.NewLine + "<label class=\"col-md-offset-2 col-md-2 checkbox-inline white-color\">" + Environment.NewLine);
            sb.Append("<input type=\"checkbox\" name=\"" + htmlName + "\" ng-model=\""+ htmlModel + "\">" + therapy );
            sb.Append("</label>" + Environment.NewLine + "<div class=\"col-md-5\">" + Environment.NewLine);
            sb.Append("<input type=\"text\" name=\"" + htmlNameCnt + "\" ng-model=\"" + htmlModelCnt + "\"  placeholder=\"Study Count\">" + Environment.NewLine);
            sb.Append("</div>" + Environment.NewLine + "</div>" + Environment.NewLine);
                
            /*
                <label class="col-md-offset-2 col-md-2 checkbox-inline white-color">
                    <input type="checkbox" name="aud_vertigo" ng-model="org.aud_vertigo" value="">Dysrythmias
                </label>
                <div class="col-md-5">
                    <input type="text" name="aud_vertigo_cnt" ng-model="org.aud_vertigo_cnt" value="0" placeholder="Study Count">
                </div>
             </div> */
            return sb.ToString();
        }

        private string setOtherFieldSection(string category)
        {
            string catsmall = category + "_other";
            string catModel = "org." + catsmall + "_exp";

            string generic = "OTHER AREAS: Type each additional area on a seperate line. Also, list the number of studies completed after each area name. Example:  Area of Expertise (5)";
            StringBuilder sb = new StringBuilder();

            sb.Append("<div class=\"form-group\">" + Environment.NewLine + "<label class=\"col-md-offset-2 col-md-2 checkbox-inline white-color\">Other Areas </label>" + Environment.NewLine);

            sb.Append(" <div class=\"col-md-6\">" + Environment.NewLine);
             sb.Append("<textarea class=\"form-control\" name=\"" + catsmall + "\" ng-model=\"" + catModel + "\" rows=\"3\" placeholder=\"" + generic + "\"></textarea>");
             sb.Append(Environment.NewLine + "</div>" + Environment.NewLine + "</div>" + Environment.NewLine);

            return sb.ToString();
        }
       /* <div class="form-group">
                <label class="col-md-offset-2 col-md-2 checkbox-inline white-color">
                    Other Areas
                </label>
                <div class="col-md-6">
                    <textarea class="form-control" name="aud_other" ng-model="org.aud_other_exp" rows="3" placeholder="OTHER AREAS: Type each additional area on a seperate line.
Also, list the number of studies completed after each area name. Example:  Area of Expertise (5)"></textarea>
                </div>
            </div>  */
    }
}
