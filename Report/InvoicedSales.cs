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

namespace Report
{
    public partial class InvoicedSales : Form
    {
    
        private MySqlConnection connection;
        private string selectedDatabase;
        public InvoicedSales(string server, string username, string password, string database)
        {
            InitializeComponent();
            // Initialize the MySQL connection using the provided information
            string connectionString = $"Server={server};Uid={username};Pwd={password};Database={database};";
            connection = new MySqlConnection(connectionString);

            // Store the selected database
            selectedDatabase = database;
            dataGridView1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Right;
        }


        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            // When user Clicks It should appears on dataGridView1
            PopulateDataGridView();
            UpdateRecordCount();


        }

        private void PopulateDataGridView()
        {
            try
            {
                // Open the connection
                connection.Open();
                // Get the selected dates from the DateTimePicker controls
                string fromDate = dateTimePicker1.Value.ToString("yyyy/MM/dd");
                string toDate = dateTimePicker2.Value.ToString("yyyy/MM/dd");

                // Create SQL query
                string query = @" 
                    SELECT 
                            finance.code AS finCode,
                            finance.ams AS finIndex,
                            finance.date AS finDate,
                            finance.accID AS cusID,
                            customer.code AS cusCode,
                            job.ordID AS ordID,
                            salesorder.code AS ordCode,
                            item.jobID AS jobID,
                            job.code AS jobCode,
                            job.title AS title,
                            job.sale AS sale,
                            SUM(journal.net) AS net
                        FROM 
                            finance
                        JOIN 
                            company AS customer ON customer.ID = finance.accID
                        JOIN 
                            journal ON journal.finID = finance.ID
                        JOIN 
                            item ON item.ID = journal.itemID
                        JOIN 
                            job ON job.ID = item.jobID
                        JOIN 
                            project AS salesorder ON salesorder.ID = job.ordID 
                        WHERE 
                            finance.ledger = '1'
                            AND finance.date BETWEEN @FromDate AND @ToDate
                        GROUP BY 
                                finance.ID, 
                                finance.code,
                                finance.ams,
                                finance.date,
                                finance.accID,
                                customer.code,
                                job.ordID,
                                salesorder.code,
                                item.jobID,
                                job.code,
                                job.title,
                                job.sale";


                // Create command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@FromDate", fromDate);
                cmd.Parameters.AddWithValue("@ToDate", toDate);

                // Create DataAdapter
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                DataTable dataTable = new DataTable();

                // Fill DataTable
                adapter.Fill(dataTable);

                // Bind DataGridView to DataTable
                dataGridView1.DataSource = dataTable;

                UpdateRecordCount();
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

        private void label3_Click(object sender, EventArgs e)
        {
            UpdateRecordCount();
        }

        private void UpdateRecordCount()
        {
            int recordCount = dataGridView1.RowCount;
            // If there's only one row and it's empty, adjust the count
            if (recordCount == 1 && dataGridView1.Rows[0].IsNewRow)
            {
                recordCount = 0;
            }
            label3.Text = $"Total Records: {recordCount}";
        }

        private void button2_Click(object sender, EventArgs e)
        {
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
