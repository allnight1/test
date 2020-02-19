using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using RestSharp;
using Newtonsoft.Json;
using System.Collections;

namespace test
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }
        DataSet result;
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog of = new OpenFileDialog() { Filter = "Excel Files|*.xls;*.xlsx;", ValidateNames = true })
                {
                    if (of.ShowDialog() == DialogResult.OK)
                    {
                        FileStream fs = File.Open(of.FileName, FileMode.Open, FileAccess.Read);
                        IExcelDataReader reader;
                        if (of.FilterIndex == 1)
                            reader = ExcelReaderFactory.CreateReader(fs);
                        else
                            reader = ExcelReaderFactory.CreateOpenXmlReader(fs);
                        result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                            {
                                UseHeaderRow = true
                            }
                        });                     
                        reader.Close();                                              
                        MessageBox.Show("Файл успешно загружен", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                   
                }
                var res = result.Tables[0];
                dataGridView1.DataSource = res;           
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private string IFinfo(string strIP)
        {
            string strIpInf = string.Empty;
            var client = new RestClient("http://ip-api.com/json/" + strIP+ "?fields=status,message,continent,continentCode,country,countryCode,region,regionName,city,zip,lat,lon,timezone,currency,isp,org,as,asname,reverse,mobile,proxy,hosting,query");
            var request = new RestRequest()
            {
                Method = Method.POST

            };
            var response = client.Execute(request);
            var dictionary = JsonConvert.DeserializeObject<IDictionary>(response.Content);
            foreach (var key in dictionary.Keys)
            {
                strIpInf += key.ToString() + ": " + dictionary[key] + "\r\n";
            }
            return strIpInf;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            txtIPINFOResult.Text = IFinfo(dataGridView1.CurrentCell.Value.ToString());           
        }
    }
}
