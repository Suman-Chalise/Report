﻿using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Report
{
    public partial class PurchaseInvoices : Form
    {
        private MySqlConnection connection;
        private string selectedDatabase;
        public PurchaseInvoices(string server, string username, string password, string database)
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
            SELECT finance.code AS finCode,
                finance.ams  AS finIndex,
                finance.date AS finDate,
                finance.rptdate AS rptDate,
                finance.pedID AS pedID,
                period.code   AS pedCode,
                period.ams    AS pedIndex,
                finance.refINT AS internal,
                finance.refEXT AS external,
                finance.type AS type,
                finance.accID AS supID,
                finance.ledger AS FinanceLedger,

                supplier.code AS supCode,
                supplier.ams  AS supIndex,
                IF(finance.type < '3', +1, -1) * SUM(journal.net) AS net,
                IF(finance.type < '3', +1, -1) * SUM(journal.vat) AS vat,
                IF(finance.type < '3', +1, -1) * SUM(journal.gross) AS gross,
                IF(finance.type < '3', +1, -1) * finance.balance AS balance,
                finance.gross AS value,
                finance.active AS active,
                finance.status AS status 
            FROM journal, finance
            LEFT JOIN company AS supplier ON finance.accID = supplier.ID
            LEFT JOIN period AS period ON finance.pedID = period.ID	
            WHERE finance.ledger = '2'
            AND journal.finID  = finance.ID
            AND finance.date BETWEEN @FromDate AND @ToDate
            GROUP BY finance.ID;";

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



        // -- counting below 

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

        private void label3_Click(object sender, EventArgs e)
        {
            UpdateRecordCount();
        }
    }
}
