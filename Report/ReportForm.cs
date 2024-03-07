using MySql.Data.MySqlClient;

namespace Report
{
    public partial class ReportForm : Form
    {
        public ReportForm()
        {
            InitializeComponent();
            // Disable the buttons initially
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button6.Enabled = false;
            comboBox1.Enabled = false;
        }



        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // database server for user to input 
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // username of the user 
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            // Password of the user 
            // Change the text to asterisks
            textBox3.PasswordChar = '*';

        }

        private void button5_Click(object sender, EventArgs e)
        {
            // click event when user input all details above
            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password

            string connectionString = $"Server={server};Uid={username};Pwd={password};";

            try
            {
                // Connect to the MySQL server
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    connection.Open();

                    // Get the list of databases
                    MySqlCommand cmd = new MySqlCommand("SHOW DATABASES", connection);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        comboBox1.Items.Clear(); // Clear existing items

                        // Add databases to the dropdown list
                        while (reader.Read())
                        {
                            comboBox1.Items.Add(reader.GetString(0));
                        }
                    }

                    // Enable the buttons for reports
                    //button1.Enabled = true;
                    //button2.Enabled = true;
                    //button3.Enabled = true;
                    //button4.Enabled = true;
                    comboBox1.Enabled = true;

                    MessageBox.Show("Connection successful!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // list of database iin dropdown if all success
            // Enable the buttons for reports when a database is selected
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
            button6.Enabled = true;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password
            string selectedDatabase = comboBox1.SelectedItem.ToString(); // Selected database from comboBox1

            // Instantiate SalesForm and pass connection information
            SalesForm salesForm = new SalesForm(server, username, password, selectedDatabase);

            // Show the new form
            salesForm.Show();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password
            string selectedDatabase = comboBox1.SelectedItem.ToString(); // Selected database from comboBox1

            // Instantiate SalesForm and pass connection information


            PurchaseInvoices purchase = new PurchaseInvoices(server, username, password, selectedDatabase);
            //show new form
            purchase.Show();




        }

        private void button3_Click(object sender, EventArgs e)
        {

            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password
            string selectedDatabase = comboBox1.SelectedItem.ToString(); // Selected database from comboBox1

            // Instantiate SalesForm and pass connection information


            NominalAccounts purchase = new NominalAccounts(server, username, password, selectedDatabase);
            //show new form
            purchase.Show();

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password
            string selectedDatabase = comboBox1.SelectedItem.ToString(); // Selected database from comboBox1

            // Instantiate SalesForm and pass connection information


            InvoicedSales purchase = new InvoicedSales(server, username, password, selectedDatabase);
            //show new form
            purchase.Show();

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {

            // Representative reports
            string server = textBox1.Text; // MySQL server name
            string username = textBox2.Text; // Username
            string password = textBox3.Text; // Password
            string selectedDatabase = comboBox1.SelectedItem.ToString(); // Selected database from comboBox1

            // Instantiate SalesForm and pass connection information


            RepresentativeReport purchase = new RepresentativeReport(server, username, password, selectedDatabase);
            //show new form
            purchase.Show();

        }
    }
}