﻿using MySql.Data.MySqlClient;
using Syncfusion.Windows.Forms.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatNRunProject
{
    public partial class ENR_MainForm : Form
    {
        //panel classes
        private MainFormCard MFpanelManager;
        private LoginPanelCard LoginpanelManager;
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

        //per user salt and employee id generator
        string ID;
        private int minTextLength = 5; // Minimum required text length

        public ENR_MainForm()
        {
            InitializeComponent();

            //add gender combo box
            AddEmplGenderComboBox.Items.AddRange(genders);
            AddEmplGenderComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //add job position combo box
            AddEmplPositionComboBox.Items.AddRange(position);
            AddEmplPositionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //Panel Manager
            MFpanelManager = new MainFormCard(LoginPanel, AdminPanel, ManagerPanel, CashierPanel);
            LoginpanelManager = new LoginPanelCard(UserSelector, AdminLoginFormPanel, MngrLoginFormPanel, CashierLoginFormPanel);
            AdminPanelManager = new AdminPanelCard(FoodItemPanel, SalesPanel, AccountsPanel);
            AdminFoodPanelManager = new AdminFoodPanelCard(NewItemPanel, UpdateItemPanel, CreateNewFoodBtnPanel);
            AdminAccPanelManager = new AdminAccPanelCard(NewAccPanel, UpdateAccPanel, CreateAccBtnPanel);

            MFpanelManager.MFShow(LoginPanel);
            LoginpanelManager.LoginFormShow(UserSelector);

            AccountListTable.DataError += new DataGridViewDataErrorEventHandler(AccountListTable_DataError);
            AccountListTable.RowPostPaint += new DataGridViewRowPostPaintEventHandler(AccountListTable_RowPostPaint);


            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }

        private void ENR_MainForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeAcc();
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
                        imageColumn.HeaderText = "AccountPfp";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        AccountListTable.Columns.Clear();

                        // Add the image column to the DataGridView
                        AccountListTable.Columns.Add(imageColumn);

                        // Hide the row header
                        AccountListTable.RowHeadersVisible = false;

                        AccountListTable.DataSource = dataTable;
                        AccountListTable.Columns[0].Visible = false; // hashedpass
                        AccountListTable.Columns[11].Visible = false; // hashedpass
                        AccountListTable.Columns[12].Visible = false; // fixedsalt
                        AccountListTable.Columns[13].Visible = false; // perusersalt
                        AccountListTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

                        // Handle the CellClick event to select the whole row
                        AccountListTable.CellClick += (sender, e) =>
                        {
                            if (e.RowIndex >= 0)
                            {
                                AccountListTable.Rows[e.RowIndex].Selected = true;
                            }
                        };
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




        private void AdminPB_MouseDown(object sender, MouseEventArgs e)
        {
            //Admin Picture Box
            LoginpanelManager.LoginFormShow(AdminLoginFormPanel);
            AdminUserLbl.Text = "Admin Login";

        }

        private void ManagerPB_MouseDown(object sender, MouseEventArgs e)
        {
            //Manager Picture Box

            LoginpanelManager.LoginFormShow(MngrLoginFormPanel);
            MngrUserLbl.Text = "Manager Login";


        }

        private void CashierPB_MouseDown(object sender, MouseEventArgs e)
        {
            //Cashier Picture Box

            LoginpanelManager.LoginFormShow(CashierLoginFormPanel);
            CashierUserLbl.Text = "Cashier Login";

        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (AdminLoginFormPanel.Visible)
            {
                AdminLoginFormPanel.Visible = false;
                UserSelector.Visible = true;
                AdminUserLbl.Text = "";
            }

            else
            {
                AdminLoginFormPanel.Visible = true;
                UserSelector.Visible = false;
                AdminUserLbl.Text = "";

            }
        }

        private void AdminLoginBtn_Click(object sender, EventArgs e)
        {
            if (AdminEmpIDBox.Text == "Admin" && AdminEmpPassBox.Text == "Admin123")
            {
                MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(AdminPanel);
                AdminPanelManager.AdminFormShow(FoodItemPanel);
                AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                AdminEmpIDBox.Text = "";
                AdminEmpPassBox.Text = "";
            }
            else if (string.IsNullOrEmpty(AdminEmpIDBox.Text) || string.IsNullOrEmpty(AdminEmpPassBox.Text))
            {
                MessageBox.Show("Missing text on required Field.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string emplID = AdminEmpIDBox.Text;
                string emplPass = AdminEmpPassBox.Text;
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
                                        AdminEmpIDBox.Text = "";
                                        AdminEmpPassBox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
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
            AdminEmpPassBox.UseSystemPasswordChar = !AdminEmpShowPass.Checked;
        }

        private void ADFoodItemBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(FoodItemPanel);
            AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);


        }

        private void ADAccountsBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(AccountsPanel);
            AdminAccPanelManager.AdminAccFormShow(CreateAccBtnPanel);
            LoadEmployeeAcc();

        }

        private void ADSalesBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(SalesPanel);

        }

        private void AdminSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
                LoginpanelManager.LoginFormShow(UserSelector);
            }
        }

        private void ManagerSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
                LoginpanelManager.LoginFormShow(UserSelector);
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
                LoginpanelManager.LoginFormShow(UserSelector);
            }
        }

        private void CreateNewFoodBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to add a new food item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdminFoodPanelManager.AdminFoodFormShow(NewItemPanel);
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (NewItemPanel.Visible)
            {
                NewItemPanel.Visible = false;
                CreateNewFoodBtnPanel.Visible = true;
            }

            else
            {
                NewItemPanel.Visible = true;
                CreateNewFoodBtnPanel.Visible = false;
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

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

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
                            //// Insert data into the database
                            //RetrieveDataApprovedDB(selectedRow);

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

            AdminAccPanelManager.AdminAccFormShow(UpdateAccPanel);
            
        }

        //private void RetrieveDataApprovedDB(DataGridViewRow selectedRow)
        //{
        //    try
        //    {
        //        using (MySqlConnection connection = new MySqlConnection(mysqlconn))
        //        {
        //            connection.Open();

        //            // Get the student number from the selected row
        //            string studentNumber = selectedRow.Cells["StudNum"].Value.ToString();

        //            // Check if the student number exists in the database
        //            string selectQuery = "SELECT * FROM mapproveddb WHERE StudNum = @StudNum";
        //            MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
        //            selectCmd.Parameters.AddWithValue("@StudNum", studentNumber);

        //            using (MySqlDataReader reader = selectCmd.ExecuteReader())
        //            {
        //                if (reader.Read())
        //                {
        //                    // Retrieve data from the database
        //                    string name = reader["Name"].ToString();
        //                    string sn = reader["StudNum"].ToString();
        //                    string rp = reader["RecoveryPin"].ToString();
        //                    string course = reader["Course"].ToString();
        //                    string age = reader["Age"].ToString();
        //                    string gender = reader["Gender"].ToString();
        //                    string email = reader["Email"].ToString();

        //                    // Populate the text boxes with the retrieved data
        //                    UpdateNameBox.Text = name;
        //                    UpdateSNBox.Text = sn;
        //                    UpdateRPBox.Text = rp;
        //                    UpdateCourseBox.Text = course;
        //                    UpdateAgeBox.Text = age;
        //                    UpdateGenderComboBox.Text = gender;
        //                    UpdateEmailBox.Text = email;
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Handle database exception (e.g., connection error or duplicate entry)
        //        MessageBox.Show("Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    finally
        //    {
        //        connection.Close();
        //    }
        //}

        private void UpdateAccExitBtn_Click(object sender, EventArgs e)
        {
            if (UpdateAccPanel.Visible)
            {
                CreateAccBtnPanel.Visible = true;
                UpdateAccPanel.Visible = false;
            }
            else
            {
                CreateAccBtnPanel.Visible = false;
                UpdateAccPanel.Visible = true;
            }
        }

        private void NewAccExitBtn_Click(object sender, EventArgs e)
        {
            if (NewAccPanel.Visible)
            {
                CreateAccBtnPanel.Visible = true;
                NewAccPanel.Visible = false;
                EmplIDRefresher();
            }
            else
            {
                CreateAccBtnPanel.Visible = false;
                NewAccPanel.Visible = true;
            }
        }




        private void MngrEmplShowPass_CheckedChanged(object sender, EventArgs e)
        {
            MngrEmplPassBox.UseSystemPasswordChar = !MngrEmplShowPass.Checked;

        }

        private void MngrLoginBtn_Click(object sender, EventArgs e)
        {

            if (MngrEmplIDBox.Text == "Manager" && MngrEmplPassBox.Text == "Manager123")
            {
                MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(ManagerPanel);
                MngrEmplIDBox.Text = "";
                MngrEmplPassBox.Text = "";
            }
            else if (string.IsNullOrEmpty(MngrEmplIDBox.Text) || string.IsNullOrEmpty(MngrEmplPassBox.Text))
            {
                MessageBox.Show("Missing text on required Field.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string emplID = MngrEmplIDBox.Text;
                string emplPass = MngrEmplPassBox.Text;
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
                                if (employeePosition == "Manager")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(ManagerPanel);
                                        MngrEmplIDBox.Text = "";
                                        MngrEmplPassBox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
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

        private void MngrExitButton_Click(object sender, EventArgs e)
        {
            LoginpanelManager.LoginFormShow(UserSelector);
        }

        private void CashierExitBtn_Click(object sender, EventArgs e)
        {
            LoginpanelManager.LoginFormShow(UserSelector);

        }

        private void CashierEmplShowPass_CheckedChanged(object sender, EventArgs e)
        {
            CashierEmplPassBox.UseSystemPasswordChar = !CashierEmplShowPass.Checked;

        }

        private void CashierLoginBtn_Click(object sender, EventArgs e)
        {

            if (CashierEmplIDBox.Text == "Cashier" && CashierEmplPassBox.Text == "Cashier123")
            {
                MessageBox.Show("Welcome back Admin.", "Greetings", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(CashierPanel);
                CashierEmplIDBox.Text = "";
                CashierEmplPassBox.Text = "";
            }
            else if (string.IsNullOrEmpty(CashierEmplIDBox.Text) || string.IsNullOrEmpty(CashierEmplPassBox.Text))
            {
                MessageBox.Show("Missing text on required Field.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                string emplID = CashierEmplIDBox.Text;
                string emplPass = CashierEmplPassBox.Text;
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
                                        MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(CashierPanel);
                                        CashierEmplIDBox.Text = "";
                                        CashierEmplPassBox.Text = "";
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
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

    }
}
