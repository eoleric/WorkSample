using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SQLite;
using System.IO;

namespace CodeTest
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        string dataFilePath;
        string dataBaseName = "DataBase.sqlite";
        List<EmailCount> emailCountList = new List<EmailCount>();//could maybe be a dictionary. Dont know how the DataGrid of WPF works with dictionaries
        

        public Home()
        {
            InitializeComponent();
        }

        public void SetUpDatabase()
        {
            SQLiteConnection.CreateFile(dataBaseName);//should check if a db is already precent and idealy not have a static name

            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + dataBaseName + ";");
            dbConnection.Open();

            string sqlCreate = "CREATE TABLE dataTable (first_name VARCHAR(50), last_name VARCHAR(50), company_name VARCHAR(50), address VARCHAR(50), city VARCHAR(50), county VARCHAR(50), state VARCHAR(50), zip INT, phone1 VARCHAR(50), phone2 VARCHAR(50), emailStart VARCHAR(50), emailEnd VARCHAR(50), web VARCHAR(50))"; // should be dynamic from first line in data file
            SQLiteCommand commandCreate = new SQLiteCommand(sqlCreate, dbConnection);
            commandCreate.ExecuteNonQuery();

            using (var fileReader = new StreamReader(dataFilePath))
            {
                fileReader.ReadLine();
                while (!fileReader.EndOfStream)
                {
                    var line = fileReader.ReadLine();
                    string newLine = line.Replace("\"", "'").Replace("@", "','");

                    string sqlAddLine = "INSERT into dataTable (first_name, last_name, company_name, address, city, county, state, zip, phone1, phone2, emailStart, emailEnd, web) values (" + newLine + ")"; // would be best to have columns stored in a array and build the string
                    SQLiteCommand commandAddLine = new SQLiteCommand(sqlAddLine, dbConnection); // Should use string builder to speed up building sql commands since it is currently way to slow
                    commandAddLine.ExecuteNonQuery();
                }
            }
            dbConnection.Close();

        }

        public void ShowCommonEmailDomains()
        {
            SQLiteConnection dbConnection = new SQLiteConnection("Data Source=" + dataBaseName + ";");
            dbConnection.Open();
            string sqlGetData = "SELECT emailEnd, COUNT(emailEnd) FROM dataTable GROUP BY emailEnd HAVING COUNT(emailEnd) >= 1 ORDER BY COUNT(emailEnd) desc";
            SQLiteCommand commandGetData = new SQLiteCommand(sqlGetData, dbConnection);
            SQLiteDataReader dataReader = commandGetData.ExecuteReader();

            while (dataReader.Read())
            {
                emailCountList.Add(new EmailCount((string)dataReader["emailEnd"], (long)dataReader["COUNT(emailEnd)"]));
            }

            DataTable.ItemsSource = emailCountList;

            dbConnection.Close();
        }

        private void Button_Click_Load_File(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.DefaultExt = ".csv";
            dlg.Filter = "CSV Files (*.csv)|*.csv";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                dataFilePath = dlg.FileName;

                ShowCommonEmailDomains(); // should be called depending on data type selected from list and not just from loading a file
            }
        }
    }
}
