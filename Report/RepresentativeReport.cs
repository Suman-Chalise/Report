using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.ComponentModel.Design.ObjectSelectorEditor;

namespace Report
{
    public partial class RepresentativeReport : Form
    {

        private MySqlConnection connection;
        private string selectedDatabase;
        public RepresentativeReport(string server, string username, string password, string database)
        {
            InitializeComponent();

            // Initialize the MySQL connection using the provided information
            string connectionString = $"Server={server};Uid={username};Pwd={password};Database={database};";
            connection = new MySqlConnection(connectionString);

            // Store the selected database
            selectedDatabase = database;

            // Populate the ComboBox when the form is loaded
            PopulateComboBox1();

            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;

        }

        private void PopulateComboBox1()
        {
            try
            {
                // Open the connection
                connection.Open();

                // Create SQL query
                string query = "SELECT distinct code FROM contact";

                // Create command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                // Create DataReader
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    // Clear existing items
                    comboBox1.Items.Clear();

                    // Add items to the ComboBox
                    while (reader.Read())
                    {
                        string code = reader.GetString(0); // Use index instead of column name
                        comboBox1.Items.Add(code);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error while fetching data from company table: " + ex.Message);
            }
            finally
            {
                // Close the connection
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            PopulateDataGridView();

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        private void PopulateDataGridView()
        {
            try
            {
                // Open the connection
                connection.Open();

                // Create SQL query
                string query = @"
                            SELECT 
                                IF(GROUPING(executive.code) = 1, 'Total', executive.code) AS repCode,
                                ROUND(SUM(job.quantity)) AS totalQuantity,
                                SUM(job.sale) AS totalSale,
                                SUM(job.cost) AS totalCost,
                                SUM(job.sale - job.cost) AS totalProfit
                            FROM 
                                project AS salesorder
                            INNER JOIN 
                                company AS customer ON salesorder.accID = customer.ID
                            INNER JOIN 
                                contact AS executive ON salesorder.repID = executive.ID
                            INNER JOIN 
                                contact AS support ON customer.sptID = support.ID
                            INNER JOIN 
                                job ON salesorder.ID = job.ordID
                            LEFT JOIN 
                                product ON job.prdID = product.ID 
                            WHERE 
    
                                executive.code LIKE @RepresentativeCode
                                 AND salesorder.type = 'S'
                                 AND salesorder.date BETWEEN @FromDate AND @ToDate
                            GROUP BY 
                                executive.code WITH ROLLUP";


                // Create command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                cmd.Parameters.AddWithValue("@RepresentativeCode", "%" + comboBox1.Text + "%");
                cmd.Parameters.AddWithValue("@FromDate", dateTimePicker1.Value.ToString("yyyy/MM/dd"));
                cmd.Parameters.AddWithValue("@ToDate", dateTimePicker2.Value.ToString("yyyy/MM/dd"));

                // Create DataAdapter
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                // Fill DataTable
                adapter.Fill(dataTable);

                // Bind DataGridView to DataTable
                dataGridView1.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Close the connection
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Call the method to create CSV file
            ExportToCSV(dataGridView1);
        }

        private void ExportToCSV(DataGridView dataGridView)
        {
            // Choose the file path for the CSV file
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV (*.csv)|*.csv";
            saveFileDialog.Title = "Save CSV File";
            saveFileDialog.ShowDialog();

            // If the file path is not empty and the user clicks OK
            if (saveFileDialog.FileName != "")
            {
                try
                {
                    // Create a StreamWriter to write to the CSV file
                    StreamWriter writer = new StreamWriter(saveFileDialog.FileName, false, Encoding.UTF8);

                    // Write the column headers to the CSV file
                    foreach (DataGridViewColumn column in dataGridView.Columns)
                    {
                        writer.Write(column.HeaderText + ",");
                    }
                    writer.WriteLine();

                    // Write the data rows to the CSV file
                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        foreach (DataGridViewCell cell in row.Cells)
                        {
                            writer.Write(cell.Value + ",");
                        }
                        writer.WriteLine();
                    }

                    // Close the StreamWriter
                    writer.Close();

                    // Show a success message to the user
                    MessageBox.Show("CSV file saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    // Show an error message if an exception occurs
                    MessageBox.Show("An error occurred while saving the CSV file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
