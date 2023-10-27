using EatNRunProject.Properties;
using MySql.Data.MySqlClient;
using Syncfusion.Windows.Forms.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatNRunProject
{
    public partial class ENRMainForm : Form
    {
        //panel classes
        private MainFormCard MFpanelManager;
        private AdminPanelCard AdminPanelManager;
        private AdminFoodPanelCard AdminFoodPanelManager;
        private AdminAccPanelCard AdminAccPanelManager;

        //db connection
        public static string mysqlconn = "server=localhost;user=root;database=eatnrun;password=";
        public MySqlConnection connection = new MySqlConnection(mysqlconn);

        //gender combo box
        private string[] genders = { "Male", "Female", "Prefer Not to Say" };

        //job position combo box
        private string[] position = { "Admin", "Manager", "Cashier" };

        //Food Item combo box
        private string[] itemType = { "Set Meals", "Burger", "Sides", "Drinks" };

        //per user salt and employee id generator
        string ID;
        private int minTextLength = 5; // Minimum required text length



        public ENRMainForm()
        {
            InitializeComponent();



            //add acc gender combo box
            AddEmplGenderComboBox.Items.AddRange(genders);
            AddEmplGenderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //add acc job position combo box
            AddEmplPositionComboBox.Items.AddRange(position);
            AddEmplPositionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //update acc gender combo box
            UpdateEmplGenderComboBox.Items.AddRange(genders);
            UpdateEmplGenderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //update acc job position combo box
            UpdateEmplPositionComboBox.Items.AddRange(position);
            UpdateEmplPositionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //add item type combo box
            AddItemTypeComboBox.Items.AddRange(itemType);
            AddItemTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //Panel Manager
            MFpanelManager = new MainFormCard(LoginPanel, AdminPanel, ManagerPanel, CashierPanel);
            AdminPanelManager = new AdminPanelCard(FoodItemPanel, SalesPanel, AccountsPanel);
            AdminFoodPanelManager = new AdminFoodPanelCard(AddItemPanel, UpdateItemPanel, CreateNewFoodBtnPanel);
            AdminAccPanelManager = new AdminAccPanelCard(NewAccPanel, UpdateEmplAccPanel, CreateAccBtnPanel);

            MFpanelManager.MFShow(LoginPanel);


            AccountListTable.DataError += new DataGridViewDataErrorEventHandler(AccountListTable_DataError);
            AccountListTable.RowPostPaint += new DataGridViewRowPostPaintEventHandler(AccountListTable_RowPostPaint);


            FoodItemListTable.DataError += new DataGridViewDataErrorEventHandler(FoodItemListTable_DataError);
            FoodItemListTable.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemListTable_RowPostPaint);


            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }

        private void ENR_MainForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeAcc();
            LoadItemMenu();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

            if (e.CloseReason == CloseReason.UserClosing)
            {
                // Prevent the form from closing.
                e.Cancel = true;

                DialogResult result = MessageBox.Show("Do you want to close the application?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    this.Dispose();

                }


            }
        }

        private void AccountListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                AccountListTable[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void AccountListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            AccountListTable.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }

        private void FoodItemListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                FoodItemListTable[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void FoodItemListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            FoodItemListTable.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }



        public class HashHelper
        {
            public static string HashString(string input)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(inputBytes);
                    string hashedString = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                    return hashedString;
                }
            }
        }
        public class HashHelper_Salt
        {
            public static string HashString_Salt(string input_Salt)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes_Salt = Encoding.UTF8.GetBytes(input_Salt);
                    byte[] hashBytes_Salt = sha256.ComputeHash(inputBytes_Salt);
                    string hashedString_Salt = BitConverter.ToString(hashBytes_Salt).Replace("-", "").ToLower();
                    return hashedString_Salt;
                }
            }
        }
        public class HashHelper_SaltperUser
        {
            public static string HashString_SaltperUser(string input_SaltperUser)
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] inputBytes_SaltperUser = Encoding.UTF8.GetBytes(input_SaltperUser);
                    byte[] hashBytes_SaltperUser = sha256.ComputeHash(inputBytes_SaltperUser);
                    string hashedString_SaltperUser = BitConverter.ToString(hashBytes_SaltperUser).Replace("-", "").ToLower();
                    return hashedString_SaltperUser;
                }
            }
        }
        public class RandomNumberGenerator
        {
            private static Random random = new Random();

            public static string GenerateRandomNumber()
            {
                var digits = Enumerable.Range(0, 10).ToList();

                for (int i = 0; i < digits.Count; i++)
                {
                    int j = random.Next(i, digits.Count);
                    int temp = digits[i];
                    digits[i] = digits[j];
                    digits[j] = temp;
                }
                string randomNumber = string.Join("", digits.Take(4));

                return randomNumber;
            }
        }

        public void LoadEmployeeAcc()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();
                    string sql = "SELECT * FROM `accounts`";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "AccountPfp" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Employee Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        AccountListTable.Columns.Clear();

                        // Add the image column to the DataGridView
                        AccountListTable.Columns.Add(imageColumn);

                        AccountListTable.DataSource = dataTable;
                        AccountListTable.Columns[0].Visible = false; // hashedpass
                        AccountListTable.Columns[11].Visible = false; // hashedpass
                        AccountListTable.Columns[12].Visible = false; // fixedsalt
                        AccountListTable.Columns[13].Visible = false; // perusersalt
                        AccountListTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                // Make sure to close the connection (if it's open)
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            // Rest of your code for configuring DataGridView to display images without distortion
        }


        public void LoadItemMenu()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();
                    string sql = "SELECT * FROM `foodmenu`";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "AccountPfp" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Item Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        FoodItemListTable.Columns.Clear();

                        // Add the image column to the DataGridView
                        FoodItemListTable.Columns.Add(imageColumn);
                        FoodItemListTable.Columns[0].Visible = false; // hashedpass
                        FoodItemListTable.DataSource = dataTable;

                        FoodItemListTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                // Make sure to close the connection (if it's open)
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            // Rest of your code for configuring DataGridView to display images without distortion
        }


        private void ENREmplPassBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (ENREmplIDBox.Text == "Admin" && ENREmplPassBox.Text == "Admin123")
                {
                    MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MFpanelManager.MFShow(AdminPanel);
                    AdminPanelManager.AdminFormShow(FoodItemPanel);
                    AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                    ENREmplIDBox.Text = "";
                    ENREmplPassBox.Text = "";
                    SalesDatePicker.Visible = false;
                    return;
                }
                else if (ENREmplIDBox.Text == "Manager" && ENREmplPassBox.Text == "Manager123")
                {
                    MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MFpanelManager.MFShow(ManagerPanel);

                    ENREmplIDBox.Text = "";
                    ENREmplPassBox.Text = "";
                    return;
                }
                else if (ENREmplIDBox.Text == "Cashier" && ENREmplPassBox.Text == "Cashier123")
                {
                    MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MFpanelManager.MFShow(CashierPanel);

                    ENREmplIDBox.Text = "";
                    ENREmplPassBox.Text = "";
                    return;
                }
                else if (string.IsNullOrEmpty(ENREmplIDBox.Text) || string.IsNullOrEmpty(ENREmplPassBox.Text))
                {
                    MessageBox.Show("Missing text on required Field.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    string emplID = ENREmplIDBox.Text;
                    string emplPass = ENREmplPassBox.Text;
                    string passchecker = HashHelper.HashString(emplPass); // Assuming "enteredPassword" is supposed to be "emplPass"

                    MySqlConnection connection = null;

                    try
                    {
                        connection = new MySqlConnection(mysqlconn);
                        connection.Open();

                        // Query the database for the provided Employee ID in the accounts table
                        string queryApproved = "SELECT EmployeeName, EmployeeID, EmployeePosition, HashedPass FROM accounts WHERE EmployeeID = @EmplID";

                        using (MySqlCommand cmdApproved = new MySqlCommand(queryApproved, connection))
                        {
                            cmdApproved.Parameters.AddWithValue("@EmplID", emplID);

                            using (MySqlDataReader readerApproved = cmdApproved.ExecuteReader())
                            {
                                if (readerApproved.Read())
                                {
                                    // Retrieve user information
                                    string name = readerApproved["EmployeeName"].ToString();
                                    string employeePosition = readerApproved["EmployeePosition"].ToString();

                                    // Check if the entered EmployeePosition matches the one in the database
                                    if (employeePosition == "Admin")
                                    {
                                        // Retrieve the HashedPass column
                                        string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                        // Check if the entered password matches
                                        bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                        if (passwordMatches)
                                        {
                                            MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            MFpanelManager.MFShow(AdminPanel);
                                            AdminPanelManager.AdminFormShow(FoodItemPanel);
                                            AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                                            ENREmplIDBox.Text = "";
                                            ENREmplPassBox.Text = "";
                                            SalesDatePicker.Visible = false;
                                        }
                                        else
                                        {
                                            MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        return;
                                    }
                                    else if (employeePosition == "Manager")
                                    {
                                        // Retrieve the HashedPass column
                                        string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                        // Check if the entered password matches
                                        bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                        if (passwordMatches)
                                        {
                                            MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            MFpanelManager.MFShow(ManagerPanel);

                                            ENREmplIDBox.Text = "";
                                            ENREmplPassBox.Text = "";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        return;
                                    }
                                    else if (employeePosition == "Cashier")
                                    {
                                        // Retrieve the HashedPass column
                                        string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                        // Check if the entered password matches
                                        bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                        if (passwordMatches)
                                        {
                                            MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                            MFpanelManager.MFShow(CashierPanel);

                                            ENREmplIDBox.Text = "";
                                            ENREmplPassBox.Text = "";
                                        }
                                        else
                                        {
                                            MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        }
                                        return;
                                    }
                                    else
                                    {
                                        // The EmployeePosition doesn't match the expected "Admin"
                                        MessageBox.Show("Account not found.", "Ooooops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                    }
                                }
                                else
                                {
                                    // The entered Employee ID does not exist in the database
                                    MessageBox.Show("Account not found.", "Ooooops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        connection?.Close();
                    }
                }

                e.SuppressKeyPress = true;
            }
            
        }

        private void AdminLoginBtn_Click(object sender, EventArgs e)
        {
            if (ENREmplIDBox.Text == "Admin" && ENREmplPassBox.Text == "Admin123")
            {
                MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(AdminPanel);
                AdminPanelManager.AdminFormShow(FoodItemPanel);
                AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                ENREmplIDBox.Text = "";
                ENREmplPassBox.Text = "";
                SalesDatePicker.Visible = false;
                return;
            }
            else if (ENREmplIDBox.Text == "Manager" && ENREmplPassBox.Text == "Manager123")
            {
                MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(ManagerPanel);

                ENREmplIDBox.Text = "";
                ENREmplPassBox.Text = "";
                return;
            }
            else if (ENREmplIDBox.Text == "Cashier" && ENREmplPassBox.Text == "Cashier123")
            {
                MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(CashierPanel);

                ENREmplIDBox.Text = "";
                ENREmplPassBox.Text = "";
                return;
            }
            else if (string.IsNullOrEmpty(ENREmplIDBox.Text) || string.IsNullOrEmpty(ENREmplPassBox.Text))
            {
                MessageBox.Show("Missing text on required Field.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string emplID = ENREmplIDBox.Text;
                string emplPass = ENREmplPassBox.Text;
                string passchecker = HashHelper.HashString(emplPass); // Assuming "enteredPassword" is supposed to be "emplPass"

                MySqlConnection connection = null;

                try
                {
                    connection = new MySqlConnection(mysqlconn);
                    connection.Open();

                    // Query the database for the provided Employee ID in the accounts table
                    string queryApproved = "SELECT EmployeeName, EmployeeID, EmployeePosition, HashedPass FROM accounts WHERE EmployeeID = @EmplID";

                    using (MySqlCommand cmdApproved = new MySqlCommand(queryApproved, connection))
                    {
                        cmdApproved.Parameters.AddWithValue("@EmplID", emplID);

                        using (MySqlDataReader readerApproved = cmdApproved.ExecuteReader())
                        {
                            if (readerApproved.Read())
                            {
                                // Retrieve user information
                                string name = readerApproved["EmployeeName"].ToString();
                                string employeePosition = readerApproved["EmployeePosition"].ToString();

                                // Check if the entered EmployeePosition matches the one in the database
                                if (employeePosition == "Admin")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(AdminPanel);
                                        AdminPanelManager.AdminFormShow(FoodItemPanel);
                                        AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                                        ENREmplIDBox.Text = "";
                                        ENREmplPassBox.Text = "";
                                        SalesDatePicker.Visible = false;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    return;
                                }
                                else if (employeePosition == "Manager")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(ManagerPanel);

                                        ENREmplIDBox.Text = "";
                                        ENREmplPassBox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    return;
                                }
                                else if (employeePosition == "Cashier")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(CashierPanel);

                                        ENREmplIDBox.Text = "";
                                        ENREmplPassBox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                    return;
                                }
                                else
                                {
                                    // The EmployeePosition doesn't match the expected "Admin"
                                    MessageBox.Show("Account not found.", "Ooooops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                            }
                            else
                            {
                                // The entered Employee ID does not exist in the database
                                MessageBox.Show("Account not found.", "Ooooops", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection?.Close();
                }
            }

        }

        private void EmpShowPass_CheckedChanged(object sender, EventArgs e)
        {
            ENREmplPassBox.UseSystemPasswordChar = !ENREmplShowPass.Checked;
        }

        private void ADFoodItemBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(FoodItemPanel);
            AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
            LoadItemMenu();
            SalesDatePicker.Visible = false;


        }

        private void ADAccountsBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(AccountsPanel);
            AdminAccPanelManager.AdminAccFormShow(CreateAccBtnPanel);
            SalesDatePicker.Visible = false;
            LoadEmployeeAcc();

        }

        private void ADSalesBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(SalesPanel);
            SalesDatePicker.Visible = true;

        }

        private void AdminSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
            }
        }

        private void ManagerSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
            }
            else
            {

            }
        }

        private void CashierSwtichBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
            }
        }

        private void CreateNewFoodBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to add a new food item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdminFoodPanelManager.AdminFoodFormShow(AddItemPanel);
                AddItemBoxClear();
                ItemIDRefresher();
            }
        }

        private void UpdateItemExitBtn_Click(object sender, EventArgs e)
        {
            if (UpdateItemPanel.Visible)
            {
                CreateNewFoodBtnPanel.Visible = true;
                UpdateItemPanel.Visible = false;
            }

            else
            {
                CreateNewFoodBtnPanel.Visible = false;
                UpdateItemPanel.Visible = true;
            }
        }

        private void FoodItemEditBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to edit this food item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdminFoodPanelManager.AdminFoodFormShow(UpdateItemPanel);

            }

        }

        private void CreateNewAccBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to add a new Employee Acount?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdminAccPanelManager.AdminAccFormShow(NewAccPanel);
                EmplIDRefresher();

            }
        }

        private void UpdateAccBtn_Click(object sender, EventArgs e)
        {

            if (AccountListTable.SelectedRows.Count > 0)
            {
                DialogResult dialogResult = MessageBox.Show("Do you want to edit the selected data?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (dialogResult == DialogResult.Yes)
                {
                    // Iterate through selected rows in PendingTable
                    foreach (DataGridViewRow selectedRow in AccountListTable.SelectedRows)
                    {
                        try
                        {
                            //// Re data into the database
                            RetrieveEmployeeDataFromDB(selectedRow);
                            RetrieveImageFromDB(selectedRow);
                            AdminAccPanelManager.AdminAccFormShow(UpdateEmplAccPanel);

                        }
                        catch (Exception ex)
                        {
                            // Handle any database-related errors here
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }


                }
                else if (dialogResult == DialogResult.No)
                {

                }
            }
            else
            {
                MessageBox.Show("Select a table row first.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }


        }

        private void RetrieveEmployeeDataFromDB(DataGridViewRow selectedRow)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    string employeeID = selectedRow.Cells[10].Value.ToString();

                    string selectQuery = "SELECT * FROM accounts WHERE EmployeeID = @EmployeeID";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
                    selectCmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string emplName = reader["EmployeeName"].ToString();
                            string emplGender = reader["EmployeeGender"].ToString();
                            string emplBday = reader["EmployeeBday"].ToString();
                            string emplAge = reader["EmployeeAge"].ToString();
                            string emplAdd = reader["EmployeeAddress"].ToString();
                            string emplEmail = reader["EmployeeEmail"].ToString();
                            string emplPosition = reader["EmployeePosition"].ToString();
                            string emplID = reader["EmployeeID"].ToString();

                            // Assuming emplBday is in the "MM-DD-YYYY DDDD" format
                            string dateFormat = "MM-dd-yyyy dddd";

                            if (DateTime.TryParseExact(emplBday, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                            {
                                UpdateEmplBdayPicker.Value = dateValue;
                            }
                            else
                            {
                                MessageBox.Show("Invalid date format in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }


                            UpdateEmplNameBox.Text = emplName;
                            UpdateEmplGenderComboBox.Text = emplGender;
                            UpdateEmplAgeBox.Text = emplAge;
                            UpdateEmplAddBox.Text = emplAdd;
                            UpdateEmplEmailBox.Text = emplEmail;
                            UpdateEmplPositionComboBox.Text = emplPosition;
                            UpdateEmplIDBox.Text = emplID;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            finally
            {
                connection.Close();
            }
        }

        private void RetrieveImageFromDB(DataGridViewRow selectedRow)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    string employeeID = selectedRow.Cells[10].Value.ToString();

                    string selectQuery = "SELECT * FROM accounts WHERE EmployeeID = @EmployeeID";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
                    selectCmd.Parameters.AddWithValue("@EmployeeID", employeeID);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if the "AccountPfp" column contains a blob
                            if (!reader.IsDBNull(reader.GetOrdinal("AccountPfp")))
                            {
                                byte[] imgData = (byte[])reader["AccountPfp"];
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    // Display the image in the PictureBox control
                                    UpdateEmplPicBox.Image = Image.FromStream(ms);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Close the database connection
                connection.Close();
            }
        }


        private void NewAccExitBtn_Click(object sender, EventArgs e)
        {
            if (NewAccPanel.Visible)
            {
                CreateAccBtnPanel.Visible = true;
                NewAccPanel.Visible = false;
                EmplIDRefresher();
                AddNewAccBoxClear();
            }
            else
            {
                CreateAccBtnPanel.Visible = false;
                NewAccPanel.Visible = true;
            }
        }

        private void NewAddAccBtn_Click(object sender, EventArgs e)
        {
            //Create Acc Btn
            DateTime selectedDate = AddEmplBdayPicker.Value;

            string emplName = AddEmplNameBox.Text;
            string emplGender = AddEmplGenderComboBox.Text;
            string emplBday = selectedDate.ToString("MM-dd-yyyy dddd");
            string emplAge = AddEmplAgeBox.Text;
            string emplAdd = AddEmplAddressBox.Text;
            string emplEmail = AddEmplEmailBox.Text;
            string emplPosition = AddEmplPositionComboBox.Text;
            string emplID = AddEmplIDBox.Text;
            string emplPass = AddEmplPassBox.Text;

            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");
            Regex courseRegex = new Regex("^[A-Za-z]+(?: [A-Za-z]+)*$");
            Regex passwordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]{8,}$");
            Regex gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]*\d*@gmail\.com$");


            string hashedPassword = HashHelper.HashString(emplPass);    // Password hashed
            string fixedSalt = HashHelper_Salt.HashString_Salt("EatNRun" + emplPass + "2023");    //Fixed Salt
            string perUserSalt = HashHelper_SaltperUser.HashString_SaltperUser(emplPass + ID);    //Per User salt



            int age = DateTime.Now.Year - selectedDate.Year;
            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }


            if (string.IsNullOrEmpty(emplName) || string.IsNullOrEmpty(emplGender) || string.IsNullOrEmpty(emplBday) ||
            string.IsNullOrEmpty(emplAge) || string.IsNullOrEmpty(emplAdd) || string.IsNullOrEmpty(emplEmail) ||
            string.IsNullOrEmpty(emplID) || string.IsNullOrEmpty(emplPass) || string.IsNullOrEmpty(emplPosition))
            {
                MessageBox.Show("Missing text in required fields.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method since there's an error
            }
            else if (emplName.Contains("Admin") || emplID.Contains("Admin") || emplPass.Contains("Admin123"))
            {
                MessageBox.Show("This student already has an account.");
                return;
            }
            else if (age < 18)
            {
                MessageBox.Show("Employee must be at least 18 years old to create an account.", "Age Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Validate fields using regex patterns
            else if (!nameRegex.IsMatch(emplName))
            {
                MessageBox.Show("Name must start with a capital letter and only contain alphabetic values.");
                return;
            }
            else if (!int.TryParse(emplAge, out _))
            {
                MessageBox.Show("Age must only contain numeric values.");
                return;
            }
            //else if (!int.TryParse(BtnSN, out _))
            //{
            //    MessageBox.Show("Incorrect Student Number.");
            //    return;
            //}
            else if (!gmailRegex.IsMatch(emplEmail))
            {
                MessageBox.Show("Invalid Gmail address format.");
                return;
            }
            else if (!passwordRegex.IsMatch(emplPass))
            {
                MessageBox.Show("Password must be at least 8 characters long and contain a combination of alphabetic characters, numeric digits, and special characters like (!, @, #, $, %, ^, &, *).");
                return;
            }

            // Check if an image has been selected
            else if (AddEmplPicBox.Image == null)
            {
                MessageBox.Show("Please select an image for the employee.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Convert the image to bytes
                        byte[] imageData;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            AddEmplPicBox.Image.Save(ms, ImageFormat.Jpeg); // You can choose the format you want
                            imageData = ms.ToArray();
                        }

                        // Insert data into the accounts table, including the image (AccountPfp in the first position)
                        string insertQuery = "INSERT INTO accounts (AccountPfp, EmployeeName, EmployeePosition, EmployeeAge, EmployeeBday, " +
                            "EmployeeGender, EmployeeAddress, EmployeeEmail, UID, EmployeeID, HashedPass, SaltedPass, PerEmplSaltedPass) " +
                            "VALUES (@Image, @EmplName, @EmplPosition, @EmplAge, @EmplBday, @EmplGender, @EmplAdd, @EmplEmail, @UID, @EmplID, @Password, @FixedSalt, @PerUserSalt)";

                        MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                        cmd.Parameters.AddWithValue("@Image", imageData);
                        cmd.Parameters.AddWithValue("@EmplName", emplName);
                        cmd.Parameters.AddWithValue("@EmplPosition", emplPosition);
                        cmd.Parameters.AddWithValue("@EmplAge", age); // Assuming age is the correct variable
                        cmd.Parameters.AddWithValue("@EmplBday", emplBday);
                        cmd.Parameters.AddWithValue("@EmplGender", emplGender);
                        cmd.Parameters.AddWithValue("@EmplAdd", emplAdd);
                        cmd.Parameters.AddWithValue("@EmplEmail", emplEmail);
                        cmd.Parameters.AddWithValue("@UID", ID);
                        cmd.Parameters.AddWithValue("@EmplID", emplID);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);
                        cmd.Parameters.AddWithValue("@FixedSalt", fixedSalt);
                        cmd.Parameters.AddWithValue("@PerUserSalt", perUserSalt);

                        cmd.ExecuteNonQuery();
                    }

                    // Successful insertion
                    MessageBox.Show("Welcome to Eat N' Run. \n Employee Account successfully created.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    EmplIDRefresher();
                    AddNewAccBoxClear();
                    LoadEmployeeAcc();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Make sure to close the connection
                    connection.Close();
                }
            }

            
        }




        private void AddNewAccBoxClear()
        {
            AddEmplPicBox.Image = null;
            AddEmplNameBox.Text = "";
            AddEmplGenderComboBox.SelectedIndex = -1;
            AddEmplBdayPicker.Value = DateTime.Now;
            AddEmplAgeBox.Text = "";
            AddEmplAddressBox.Text = "";
            AddEmplEmailBox.Text = "";
            AddEmplPositionComboBox.SelectedIndex = -1;
            AddEmplIDBox.Text = "";
            AddEmplPassBox.Text = "";
        }



        private void EmplIDRefresher()
        {
            AddEmplIDBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            string BtnSN = AddEmplIDBox.Text;
            AddEmplIDBox.Text = ID + "-" + BtnSN;
        }



        private void AddEmplGenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AddEmplGenderComboBox.SelectedItem != null)
            {
                AddEmplGenderComboBox.Text = AddEmplGenderComboBox.SelectedItem.ToString();
            }
        }

        private void AddEmplIDBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                // If the current length is less than or equal to the minimum required length, prevent Backspace
                if (AddEmplIDBox.Text.Length <= minTextLength)
                {
                    e.SuppressKeyPress = true; // Prevent Backspace
                }
            }
        }

        private void AddEmplBdayPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = AddEmplBdayPicker.Value;
            int age = DateTime.Now.Year - selectedDate.Year;

            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            AddEmplAgeBox.Text = age.ToString();
        }

        private void AddEmplPositionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (AddEmplPositionComboBox.SelectedItem != null)
            {
                AddEmplPositionComboBox.Text = AddEmplPositionComboBox.SelectedItem.ToString();
            }
        }

        private void AddEmplShowPass_CheckedChanged(object sender, EventArgs e)
        {
            AddEmplPassBox.UseSystemPasswordChar = !AddEmplShowPass.Checked;
        }

        private void AddEmplPicBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected image into the PictureBox
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (selectedImage.Width == 64 && selectedImage.Height == 64)
                    {
                        AddEmplPicBox.Image = selectedImage;
                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 64x64 pixels.");
                    }
                }
            }
        }

        private void UpdateEmplPicBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected image into the PictureBox
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (selectedImage.Width == 64 && selectedImage.Height == 64)
                    {
                        UpdateEmplPicBox.Image = selectedImage;
                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 64x64 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateEmplGenderComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UpdateEmplGenderComboBox.SelectedItem != null)
            {
                UpdateEmplGenderComboBox.Text = UpdateEmplGenderComboBox.SelectedItem.ToString();
            }
        }

        private void UpdateEmplBdayPicker_ValueChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = UpdateEmplBdayPicker.Value;
            int age = DateTime.Now.Year - selectedDate.Year;

            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }

            UpdateEmplAgeBox.Text = age.ToString();
        }

        private void UpdateEmplPositionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (UpdateEmplPositionComboBox.SelectedItem != null)
            {
                UpdateEmplPositionComboBox.Text = UpdateEmplPositionComboBox.SelectedItem.ToString();
            }
        }

        private void UpdateEmplShowPass_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEmplPassBox.UseSystemPasswordChar = !UpdateEmplShowPass.Checked;

        }

        private void UpdateEmplUpdateBtn_Click(object sender, EventArgs e)
        {
            //Update Acc Btn
            DateTime selectedDate = UpdateEmplBdayPicker.Value;

            string emplName = UpdateEmplNameBox.Text;
            string emplGender = UpdateEmplGenderComboBox.Text;
            string emplBday = selectedDate.ToString("MM-dd-yyyy dddd");
            string emplAge = UpdateEmplAgeBox.Text;
            string emplAdd = UpdateEmplAddBox.Text;
            string emplEmail = UpdateEmplEmailBox.Text;
            string emplPosition = UpdateEmplPositionComboBox.Text;
            string emplID = UpdateEmplIDBox.Text;
            string emplPass = UpdateEmplPassBox.Text;

            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");
            Regex courseRegex = new Regex("^[A-Za-z]+(?: [A-Za-z]+)*$");
            Regex passwordRegex = new Regex("^(?=.*[A-Z])(?=.*[a-z])(?=.*\\d)(?=.*[!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?])[A-Za-z\\d!@#$%^&*()_+\\-=\\[\\]{};':\"\\\\|,.<>\\/?]{8,}$");
            Regex gmailRegex = new Regex(@"^[A-Za-z0-9._%+-]*\d*@gmail\.com$");


            string hashedPassword = HashHelper.HashString(emplPass);    // Password hashed
            string fixedSalt = HashHelper_Salt.HashString_Salt("EatNRun" + emplPass + "2023");    //Fixed Salt
            string perUserSalt = HashHelper_SaltperUser.HashString_SaltperUser(emplPass + ID);    //Per User salt



            int age = DateTime.Now.Year - selectedDate.Year;
            if (DateTime.Now < selectedDate.AddYears(age))
            {
                age--; // Subtract 1 if the birthday hasn't occurred yet this year
            }


            if (string.IsNullOrEmpty(emplName) || string.IsNullOrEmpty(emplGender) || string.IsNullOrEmpty(emplBday) ||
                string.IsNullOrEmpty(emplAge) || string.IsNullOrEmpty(emplAdd) || string.IsNullOrEmpty(emplEmail) ||
                string.IsNullOrEmpty(emplPass) || string.IsNullOrEmpty(emplPosition))
            {
                MessageBox.Show("Missing text in required fields.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method since there's an error
            }
            else if (emplName.Contains("Admin") || emplID.Contains("Admin") || emplPass.Contains("Admin123"))
            {
                MessageBox.Show("This student already has an account.");
                return;
            }
            else if (age < 18)
            {
                MessageBox.Show("Employee must be at least 18 years old to create an account.", "Age Verification Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            // Validate fields using regex patterns
            else if (!nameRegex.IsMatch(emplName))
            {
                MessageBox.Show("Name must start with a capital letter and only contain alphabetic values.");
                return;
            }
            else if (!int.TryParse(emplAge, out _))
            {
                MessageBox.Show("Age must only contain numeric values.");
                return;
            }
            //else if (!int.TryParse(BtnSN, out _))
            //{
            //    MessageBox.Show("Incorrect Student Number.");
            //    return;
            //}
            else if (!gmailRegex.IsMatch(emplEmail))
            {
                MessageBox.Show("Invalid Gmail address format.");
                return;
            }
            else if (!passwordRegex.IsMatch(emplPass))
            {
                MessageBox.Show("Password must be at least 8 characters long and contain a combination of alphabetic characters, numeric digits, and special characters like (!, @, #, $, %, ^, &, *).");
                return;
            }

            // Check if an image has been selected
            else if (UpdateEmplPicBox.Image == null)
            {
                MessageBox.Show("Please select an image for the employee.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrEmpty(emplID))
            {
                MessageBox.Show("Employee ID is required to update an account.", "Missing Employee ID", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Check if the employee with the given Employee ID exists
                        string checkExistQuery = "SELECT COUNT(*) FROM accounts WHERE EmployeeID = @EmplID";
                        MySqlCommand checkExistCmd = new MySqlCommand(checkExistQuery, connection);
                        checkExistCmd.Parameters.AddWithValue("@EmplID", emplID);
                        int employeeCount = Convert.ToInt32(checkExistCmd.ExecuteScalar());

                        if (employeeCount == 0)
                        {
                            MessageBox.Show("Employee with the provided ID does not exist in the database.", "Employee Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        byte[] imageData = null;

                        // Check if the image in the PictureBox has been modified
                        if (UpdateEmplPicBox.Image != null)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                UpdateEmplPicBox.Image.Save(ms, ImageFormat.Jpeg); // Replace with the correct image format
                                imageData = ms.ToArray();
                            }
                        }


                        // Update data in the accounts table, including the image (AccountPfp in the first position)
                        string updateQuery = "UPDATE accounts SET EmployeeName = @EmplName, EmployeePosition = @EmplPosition, " +
                            "EmployeeAge = @EmplAge, EmployeeBday = @EmplBday, EmployeeGender = @EmplGender, EmployeeAddress = @EmplAdd, " +
                            "EmployeeEmail = @EmplEmail, HashedPass = @Password, SaltedPass = @FixedSalt, PerEmplSaltedPass = @PerUserSalt ";

                        // Add the image update if imageData is not null
                        if (imageData != null)
                        {
                            updateQuery += ", AccountPfp = @Image ";
                        }

                        updateQuery += "WHERE EmployeeID = @EmplID";

                        MySqlCommand updateCmd = new MySqlCommand(updateQuery, connection);

                        if (imageData != null)
                        {
                            updateCmd.Parameters.AddWithValue("@Image", imageData);
                        }

                        updateCmd.Parameters.AddWithValue("@EmplName", emplName);
                        updateCmd.Parameters.AddWithValue("@EmplPosition", emplPosition);
                        updateCmd.Parameters.AddWithValue("@EmplAge", age); // Assuming age is the correct variable
                        updateCmd.Parameters.AddWithValue("@EmplBday", emplBday);
                        updateCmd.Parameters.AddWithValue("@EmplGender", emplGender);
                        updateCmd.Parameters.AddWithValue("@EmplAdd", emplAdd);
                        updateCmd.Parameters.AddWithValue("@EmplEmail", emplEmail);
                        updateCmd.Parameters.AddWithValue("@EmplID", emplID);
                        updateCmd.Parameters.AddWithValue("@Password", hashedPassword);
                        updateCmd.Parameters.AddWithValue("@FixedSalt", fixedSalt);
                        updateCmd.Parameters.AddWithValue("@PerUserSalt", perUserSalt);

                        updateCmd.ExecuteNonQuery();
                    }

                    // Successful update
                    MessageBox.Show("Employee account has been successfully updated.", "Update Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateAccBoxClear();
                    LoadEmployeeAcc();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Make sure to close the connection
                    connection.Close();
                }

            }


        }



        private void UpdateAccBoxClear()
        {
            UpdateEmplPicBox.Image = null;
            UpdateEmplNameBox.Text = "";
            UpdateEmplGenderComboBox.SelectedIndex = -1;
            UpdateEmplBdayPicker.Value = DateTime.Now;
            UpdateEmplAgeBox.Text = "";
            UpdateEmplAddBox.Text = "";
            UpdateEmplEmailBox.Text = "";
            UpdateEmplPositionComboBox.SelectedIndex = -1;
            UpdateEmplIDBox.Text = "";
            UpdateEmplPassBox.Text = "";
        }

        private void UpdateEmplAccExitBtn_Click(object sender, EventArgs e)
        {
            if (UpdateEmplAccPanel.Visible)
            {
                CreateAccBtnPanel.Visible = true;
                UpdateEmplAccPanel.Visible = false;
                UpdateAccBoxClear();
            }
            else
            {
                CreateAccBtnPanel.Visible = false;
                UpdateEmplAccPanel.Visible = true;
            }
        }

        private void AdminPB_Click(object sender, EventArgs e)
        {

        }

        private void ManagerPB_Click(object sender, EventArgs e)
        {

        }

        private void CashierPB_Click(object sender, EventArgs e)
        {

        }

        private void AddItemExitBtn_Click(object sender, EventArgs e)
        {
            if (AddItemPanel.Visible)
            {
                AddItemPanel.Visible = false;
                CreateNewFoodBtnPanel.Visible = true;
                AddItemBoxClear();            }

            else
            {
                AddItemPanel.Visible = true;
                CreateNewFoodBtnPanel.Visible = false;
            }
        }

        private void AddItemCreatedDatePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void AddItemPicBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected image into the PictureBox
                    Image selectedImage = Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (selectedImage.Width == 64 && selectedImage.Height == 64)
                    {
                        AddItemPicBox.Image = selectedImage;
                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 64x64 pixels.");
                    }
                }
            }
        }

        private void AddItemTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (AddItemTypeComboBox.SelectedItem != null)
            {
                string selectedType = AddItemTypeComboBox.SelectedItem.ToString();
                string resultType = ID + "-" + selectedType;
                AddItemCodeBox.Text = resultType;
            }
            else
            {
                // Handle the case where AddItemTypeComboBox.SelectedItem is null
                // You might want to display an error message or take appropriate action.
                // For now, you can set AddItemCodeBox.Text to a default value or leave it empty.
                AddItemCodeBox.Text = "No item selected";
            }

        }

        private void AddItemBtn_Click(object sender, EventArgs e)
        {
            //Create Acc Btn
            DateTime selectedDate = AddItemCreatedDatePicker.Value;

            string itemName = AddItemNameBox.Text;
            string itemCode = AddItemCodeBox.Text;
            string itemCreated = selectedDate.ToString("MM-dd-yyyy dddd");
            string itemType = AddItemTypeComboBox.Text;
            string itemPrice = AddItemPriceBox.Text;

            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");


            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(itemCreated) ||
            string.IsNullOrEmpty(itemType) || string.IsNullOrEmpty(itemPrice))
            {
                MessageBox.Show("Missing text in required fields.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method since there's an error
            }
            else if (itemName.Contains("Admin") || itemPrice.Contains("Admin") || itemCode.Contains("Admin123") || itemPrice.Contains("Admin123"))
            {
                MessageBox.Show("The word 'Admin' cannot be used as a Food Item credentials.");
                return;
            }
            // Validate fields using regex patterns
            else if (!nameRegex.IsMatch(itemName))
            {
                MessageBox.Show("Name must start with a capital letter and only contain alphabetic values.");
                return;
            }
            //else if (!int.TryParse(itemPrice, out _))
            //{
            //    MessageBox.Show("Price must only contain numeric values.");
            //    return;
            //}
            // Check if an image has been selected
            else if (AddItemPicBox.Image == null)
            {
                MessageBox.Show("Please select an image for the employee.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Convert the image to bytes
                        byte[] imageData;
                        using (MemoryStream ms = new MemoryStream())
                        {
                            AddItemPicBox.Image.Save(ms, ImageFormat.Jpeg); // You can choose the format you want
                            imageData = ms.ToArray();
                        }

                        // Insert data into the accounts table, including the image (AccountPfp in the first position)
                        string insertQuery = "INSERT INTO foodmenu (FoodPic, FoodName, FoodCode, FoodType, FoodPrice, FoodDateCreated)"  +
                            "VALUES (@Image, @itemName, @UID, @itemType, @itemPrice, @itemCreated)";

                        MySqlCommand cmd = new MySqlCommand(insertQuery, connection);
                        cmd.Parameters.AddWithValue("@Image", imageData);
                        cmd.Parameters.AddWithValue("@itemName", itemName);
                        cmd.Parameters.AddWithValue("@UID", itemCode);
                        cmd.Parameters.AddWithValue("@itemType", itemType);
                        cmd.Parameters.AddWithValue("@itemPrice", itemPrice);
                        cmd.Parameters.AddWithValue("@itemCreated", itemCreated);


                        cmd.ExecuteNonQuery();
                    }

                    // Successful insertion
                    MessageBox.Show("Welcome to Eat N' Run. \n Employee Account successfully created.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ItemIDRefresher();
                    AddItemBoxClear();
                    LoadItemMenu();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    // Make sure to close the connection
                    connection.Close();
                }
            }
        }

        private void AddItemBoxClear()
        {
            AddItemPicBox.Image = null;
            AddItemNameBox.Text = "";
            AddItemCodeBox.Text = "";
            AddItemCreatedDatePicker.Value = DateTime.Now;
            AddItemTypeComboBox.SelectedIndex = -1 ;
            AddItemPriceBox.Text = "";

        }

        private void ItemIDRefresher()
        {
            AddItemCodeBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            string BtnSN = AddItemTypeComboBox.Text;
            AddItemCodeBox.Text = ID + "-" + BtnSN;
        }

        private void AddItemTypeComboBox_SelectedValueChanged(object sender, EventArgs e)
        {

        }
    }
}
