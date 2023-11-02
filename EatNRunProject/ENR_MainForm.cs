using CafeDeLunaSystem;
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
        private MngrPanelCard MngrPanelManager;
        private MngrItemPanelCard MngrItemPanelManager;
        private MngrOrderPanelCard MngrOrderPanelManager;

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

        //basta image
        Image selectedImage;

        //Remember Account dictionary
        private Dictionary<string, string> accountData = new Dictionary<string, string>();

        public ENRMainForm()
        {
            InitializeComponent();

            //Mngr Order View
            InitializeDataGridView();

            //Placeholder Text
            //TxtPlaceholder.SetPlaceholder(ENREmplIDBox, "Enter Employee ID");
            //TxtPlaceholder.SetPlaceholder(ENREmplPassBox, "Enter Employee Password");

            //Main Form Panel Manager
            MFpanelManager = new MainFormCard(LoginPanel, AdminPanel, ManagerPanel, CashierPanel);

            //Admin Form Manager
            AdminPanelManager = new AdminPanelCard(FoodItemPanel, SalesPanel, AccountsPanel);
            AdminFoodPanelManager = new AdminFoodPanelCard(AddItemPanel, UpdateItemPanel, CreateNewFoodBtnPanel);
            AdminAccPanelManager = new AdminAccPanelCard(NewAccPanel, UpdateEmplAccPanel, CreateAccBtnPanel);

            //Mngr Form Manager
            MngrPanelManager = new MngrPanelCard(MngrNewOrderBtnPanel, MngrOrderDashboardPanel, MngrSalesPanel);
            MngrItemPanelManager = new MngrItemPanelCard(MngrItemBurgerPanel, MngrItemSidesPanel, MngrItemSetMealsPanel, MngrItemDrinksPanel);
            MngrOrderPanelManager = new MngrOrderPanelCard(MngrOrderViewPanel, MngrCheckoutViewPanel, MngrVoidViewPanel); 

            MFpanelManager.MFShow(LoginPanel);

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

            //update item type combo box
            UpdateItemTypeComboBox.Items.AddRange(itemType);
            UpdateItemTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            //DGV Error Handlers Admin
            AccountListTable.DataError += new DataGridViewDataErrorEventHandler(AccountListTable_DataError);
            AccountListTable.RowPostPaint += new DataGridViewRowPostPaintEventHandler(AccountListTable_RowPostPaint);
            FoodItemListTable.DataError += new DataGridViewDataErrorEventHandler(FoodItemListTable_DataError);
            FoodItemListTable.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemListTable_RowPostPaint);

            //DGV Error Handlers Manager
            MngrItemBurgerView.DataError += new DataGridViewDataErrorEventHandler(FoodItemBurgerListTable_DataError);
            MngrItemBurgerView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemBurgerListTable_RowPostPaint);
            MngrItemSidesView.DataError += new DataGridViewDataErrorEventHandler(FoodItemSideListTable_DataError);
            MngrItemSidesView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemSideListTable_RowPostPaint);
            MngrItemSetMealView.DataError += new DataGridViewDataErrorEventHandler(FoodItemSetListTable_DataError);
            MngrItemSetMealView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemSetListTable_RowPostPaint);
            MngrItemDrinkView.DataError += new DataGridViewDataErrorEventHandler(FoodItemDrinkListTable_DataError);
            MngrItemDrinkView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(FoodItemDrinkListTable_RowPostPaint);

            //DGV Error Handlers Cashier



            //Order Buttons
            MngrOrderViewTable.CellContentClick += MngrOrderViewTable_CellContentClick;

            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }

        private void ENR_MainForm_Load(object sender, EventArgs e)
        {
            LoadEmployeeAcc();
            LoadItemMenu();
            LoadBurgerItemMenu();
            LoadSideItemMenu();
            LoadDrinksItemMenu();
            LoadSetItemMenu();


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

        //DGV Error Handlers Admin
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

        //Mngr DGV Error Handlers
        //Burger
        private void FoodItemBurgerListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                MngrItemBurgerView[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void FoodItemBurgerListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrItemBurgerView.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }
        //Side
        private void FoodItemSideListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                MngrItemSidesView[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void FoodItemSideListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrItemSidesView.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }
        //Set
        private void FoodItemSetListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                MngrItemSetMealView[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void FoodItemSetListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrItemSetMealView.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }
        //Drinks
        private void FoodItemDrinkListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                MngrItemDrinkView[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }
        private void FoodItemDrinkListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrItemDrinkView.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }


        //Order View Table
        private void InitializeDataGridView()
        {
            DataGridViewButtonColumn trashColumn = new DataGridViewButtonColumn();
            trashColumn.Name = "Bin";
            trashColumn.Text = "T";
            trashColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            trashColumn.Width = 10;
            MngrOrderViewTable.Columns.Add(trashColumn);

            DataGridViewTextBoxColumn itemNameColumn = new DataGridViewTextBoxColumn();
            itemNameColumn.Name = "ItemName";
            MngrOrderViewTable.Columns.Add(itemNameColumn);

            DataGridViewButtonColumn minusColumn = new DataGridViewButtonColumn();
            minusColumn.Name = "-";
            minusColumn.Text = "-";
            minusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            minusColumn.Width = 10;
            MngrOrderViewTable.Columns.Add(minusColumn);

            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.Name = "Qty";
            quantityColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            quantityColumn.Width = 15;
            MngrOrderViewTable.Columns.Add(quantityColumn);

            DataGridViewButtonColumn plusColumn = new DataGridViewButtonColumn();
            plusColumn.Name = "+";
            plusColumn.Text = "+";
            plusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            plusColumn.Width = 10;
            MngrOrderViewTable.Columns.Add(plusColumn);

            DataGridViewTextBoxColumn itemCostColumn = new DataGridViewTextBoxColumn();
            itemCostColumn.Name = "ItemCost";
            MngrOrderViewTable.Columns.Add(itemCostColumn);
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


        public void LoadBurgerItemMenu()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `foodmenu` WHERE FoodType = 'Burger' ORDER BY FoodType";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "FoodPic" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Item Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        MngrItemBurgerView.Columns.Clear();

                        // Add the image column to the DataGridView
                        MngrItemBurgerView.Columns.Add(imageColumn);

                        MngrItemBurgerView.DataSource = dataTable;

                        MngrItemBurgerView.Columns[0].Visible = false;  // Assuming 0 is the index of "Food Pic"
                        MngrItemBurgerView.Columns[4].Visible = false;  // Assuming 3 is the index of "FoodType"
                        MngrItemBurgerView.Columns[6].Visible = false;  // Assuming 5 is the index of "FoodCreated"
                        MngrItemBurgerView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        public void LoadSideItemMenu()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `foodmenu` WHERE FoodType = 'Sides' ORDER BY FoodType";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "FoodPic" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Item Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        MngrItemSidesView.Columns.Clear();

                        // Add the image column to the DataGridView
                        MngrItemSidesView.Columns.Add(imageColumn);

                        MngrItemSidesView.DataSource = dataTable;

                        MngrItemSidesView.Columns[0].Visible = false;  // Assuming 0 is the index of "Food Pic"
                        MngrItemSidesView.Columns[4].Visible = false;  // Assuming 3 is the index of "FoodType"
                        MngrItemSidesView.Columns[6].Visible = false;  // Assuming 5 is the index of "FoodCreated"
                        MngrItemSidesView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void LoadDrinksItemMenu()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `foodmenu` WHERE FoodType = 'Drinks' ORDER BY FoodType";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "FoodPic" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Item Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        MngrItemDrinkView.Columns.Clear();

                        // Add the image column to the DataGridView
                        MngrItemDrinkView.Columns.Add(imageColumn);

                        MngrItemDrinkView.DataSource = dataTable;

                        MngrItemDrinkView.Columns[0].Visible = false;  // Assuming 0 is the index of "Food Pic"
                        MngrItemDrinkView.Columns[4].Visible = false;  // Assuming 3 is the index of "FoodType"
                        MngrItemDrinkView.Columns[6].Visible = false;  // Assuming 5 is the index of "FoodCreated"
                        MngrItemDrinkView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        public void LoadSetItemMenu()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Filter and sort the data by FoodType
                    string sql = "SELECT * FROM `foodmenu` WHERE FoodType = 'Set Meals' ORDER BY FoodType";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        // Create the "FoodPic" column with the specified settings
                        DataGridViewImageColumn imageColumn = new DataGridViewImageColumn();
                        imageColumn.HeaderText = "Item Picture";
                        imageColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                        imageColumn.ImageLayout = DataGridViewImageCellLayout.Zoom;

                        // Clear any existing columns to remove the extra "AccountPfp" column
                        MngrItemSetMealView.Columns.Clear();

                        // Add the image column to the DataGridView
                        MngrItemSetMealView.Columns.Add(imageColumn);

                        MngrItemSetMealView.DataSource = dataTable;

                        MngrItemSetMealView.Columns[0].Visible = false;  // Assuming 0 is the index of "Food Pic"
                        MngrItemSetMealView.Columns[4].Visible = false;  // Assuming 3 is the index of "FoodType"
                        MngrItemSetMealView.Columns[6].Visible = false;  // Assuming 5 is the index of "FoodCreated"
                        MngrItemSetMealView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred: " + e.Message);
            }
            finally
            {
                connection.Close();
            }
        }


        private void ENREmplPassBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LoginVerifier();

                e.SuppressKeyPress = true;
            }

        }

        private void AdminLoginBtn_Click(object sender, EventArgs e)
        {
            LoginVerifier();
        }

        private void LoginVerifier()
        {
            if (ENREmplIDBox.Text == "Admin" && ENREmplPassBox.Text == "Admin123")
            {
                MessageBox.Show("Welcome back, Admin.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(AdminPanel);
                AdminPanelManager.AdminFormShow(FoodItemPanel);
                AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                rememberAccount();
                logincredclear();
                AdminSalesStartDatePicker.Visible = false;
                return;
            }
            else if (ENREmplIDBox.Text == "Manager" && ENREmplPassBox.Text == "Manager123")
            {
                MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(ManagerPanel);
                MngrPanelManager.MngrFormShow(MngrNewOrderBtnPanel);
                rememberAccount();
                logincredclear();
                return;
            }
            else if (ENREmplIDBox.Text == "Cashier" && ENREmplPassBox.Text == "Cashier123")
            {
                MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MFpanelManager.MFShow(CashierPanel);
                rememberAccount();
                logincredclear();
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
                                        rememberAccount();
                                        logincredclear();
                                        AdminSalesStartDatePicker.Visible = false;
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
                                        MessageBox.Show($"Welcome back, Manager {name}.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(ManagerPanel);
                                        MngrPanelManager.MngrFormShow(MngrNewOrderBtnPanel);
                                        MngrNameLbl.Text = "| Manager Name: " + name;
                                        MngrSessionNumRefresh();
                                        rememberAccount();
                                        logincredclear();

                                        return;
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect Password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }


                                else if (employeePosition == "Cashier")
                                {
                                    // Retrieve the HashedPass column
                                    string hashedPasswordFromDB = readerApproved["HashedPass"].ToString();

                                    // Check if the entered password matches
                                    bool passwordMatches = hashedPasswordFromDB.Equals(passchecker);

                                    if (passwordMatches)
                                    {
                                        MessageBox.Show($"Welcome back, Cashier {name}.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        MFpanelManager.MFShow(CashierPanel);
                                        CashierLbl.Text = "Cashier " + name;
                                        rememberAccount();
                                        logincredclear();


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

        private void MngrSessionNumRefresh()
        {
            MngrSessionNum.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            MngrSessionNum.Text = "| Session Number: " + ID;
        }

        private void MngrOrderNumRefresh()
        {
            MngrOrderNumLbl.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            MngrOrderNumLbl.Text = "| Order Number: " + ID;
        }

        private void logincredclear()
        {
            ENREmplIDBox.Text = "";
            ENREmplPassBox.Text = "";
        }

        private void rememberAccount()
        {
            string newItem = ENREmplIDBox.Text.Trim();
            string newPassword = ENREmplPassBox.Text.Trim();

            bool itemExists = ENREmplIDBox.Items.Contains(newItem);

            if (RmbrAccCheckbox.Checked == true && !itemExists)
            {
                // Store username and password in the dictionary
                accountData[newItem] = newPassword;

                // Add the username to the combo box
                ENREmplIDBox.Items.Add(newItem);

                // Clear the textboxes
                ENREmplIDBox.SelectedIndex = ENREmplIDBox.Items.IndexOf(newItem);
                ENREmplIDBox.Text = "";
                ENREmplPassBox.Text = "";
            }
        }

        private void ENREmplIDBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedItem = ENREmplIDBox.SelectedItem as string;
            if (selectedItem != null && accountData.ContainsKey(selectedItem))
            {
                ENREmplPassBox.Text = accountData[selectedItem];
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
            AdminSalesStartDatePicker.Visible = false;


        }

        private void ADAccountsBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(AccountsPanel);
            AdminAccPanelManager.AdminAccFormShow(CreateAccBtnPanel);
            AdminSalesStartDatePicker.Visible = false;
            LoadEmployeeAcc();

        }

        private void ADSalesBtn_Click(object sender, EventArgs e)
        {
            AdminPanelManager.AdminFormShow(SalesPanel);
            AdminSalesStartDatePicker.Visible = true;

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

        private void CashierSwitchBtn_Click(object sender, EventArgs e)
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

        private void FoodItemEditBtn_Click(object sender, EventArgs e)
        {

            if (FoodItemListTable.SelectedRows.Count > 0)
            {
                DialogResult result = MessageBox.Show("Do you want to edit this food item?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Iterate through selected rows in PendingTable
                    foreach (DataGridViewRow selectedRow in FoodItemListTable.SelectedRows)
                    {
                        try
                        {
                            //// Re data into the database
                            RetrieveItemDataFromDB(selectedRow);
                            RetrieveItemImageFromDB(selectedRow);
                            AdminFoodPanelManager.AdminFoodFormShow(UpdateItemPanel);

                        }
                        catch (Exception ex)
                        {
                            // Handle any database-related errors here
                            MessageBox.Show("Error: " + ex.Message);
                        }
                    }


                }
                else if (result == DialogResult.No)
                {

                }
            }
            else
            {
                MessageBox.Show("Select a table row first.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
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
                            RetrieveEmplImageFromDB(selectedRow);
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

        private void RetrieveEmplImageFromDB(DataGridViewRow selectedRow)
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
                    if (selectedImage.Width == 128 && selectedImage.Height == 128)
                    {
                        AddEmplPicBox.Image = selectedImage;
                    }
                    else if (selectedImage.Width != 128 && selectedImage.Height != 128)
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                    if (selectedImage.Width == 128 && selectedImage.Height == 128)
                    {
                        UpdateEmplPicBox.Image = selectedImage;
                    }
                    else if (selectedImage.Width != 128 && selectedImage.Height != 128)
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                byte[] imageData = null;

                // Check if the image in the PictureBox has been modified
                if (selectedImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        selectedImage.Save(ms, ImageFormat.Jpeg); // Replace with the correct image format
                        imageData = ms.ToArray();
                    }
                }

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

                        // Update data in the accounts table, including the image conditionally
                        if (imageData != null)
                        {
                            // Update with image
                            string updateWithImageQuery = "UPDATE accounts SET EmployeeName = @EmplName, EmployeePosition = @EmplPosition, " +
                                "EmployeeAge = @EmplAge, EmployeeBday = @EmplBday, EmployeeGender = @EmplGender, EmployeeAddress = @EmplAdd, " +
                                "EmployeeEmail = @EmplEmail, HashedPass = @Password, SaltedPass = @FixedSalt, PerEmplSaltedPass = @PerUserSalt, " +
                                "AccountPfp = @Image " +
                                "WHERE EmployeeID = @EmplID";
                            MySqlCommand updateWithImageCmd = new MySqlCommand(updateWithImageQuery, connection);
                            updateWithImageCmd.Parameters.AddWithValue("@Image", imageData);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplName", emplName);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplPosition", emplPosition);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplAge", age);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplBday", emplBday);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplGender", emplGender);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplAdd", emplAdd);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplEmail", emplEmail);
                            updateWithImageCmd.Parameters.AddWithValue("@EmplID", emplID);
                            updateWithImageCmd.Parameters.AddWithValue("@Password", hashedPassword);
                            updateWithImageCmd.Parameters.AddWithValue("@FixedSalt", fixedSalt);
                            updateWithImageCmd.Parameters.AddWithValue("@PerUserSalt", perUserSalt);
                            updateWithImageCmd.ExecuteNonQuery();
                            selectedImage = null;
                        }
                        else
                        {
                            // Update without image
                            string updateWithoutImageQuery = "UPDATE accounts SET EmployeeName = @EmplName, EmployeePosition = @EmplPosition, " +
                                "EmployeeAge = @EmplAge, EmployeeBday = @EmplBday, EmployeeGender = @EmplGender, EmployeeAddress = @EmplAdd, " +
                                "EmployeeEmail = @EmplEmail, HashedPass = @Password, SaltedPass = @FixedSalt, PerEmplSaltedPass = @PerUserSalt " +
                                "WHERE EmployeeID = @EmplID";
                            MySqlCommand updateWithoutImageCmd = new MySqlCommand(updateWithoutImageQuery, connection);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplName", emplName);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplPosition", emplPosition);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplAge", age);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplBday", emplBday);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplGender", emplGender);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplAdd", emplAdd);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplEmail", emplEmail);
                            updateWithoutImageCmd.Parameters.AddWithValue("@EmplID", emplID);
                            updateWithoutImageCmd.Parameters.AddWithValue("@Password", hashedPassword);
                            updateWithoutImageCmd.Parameters.AddWithValue("@FixedSalt", fixedSalt);
                            updateWithoutImageCmd.Parameters.AddWithValue("@PerUserSalt", perUserSalt);
                            updateWithoutImageCmd.ExecuteNonQuery();
                        }
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

        private void AddItemExitBtn_Click(object sender, EventArgs e)
        {
            if (AddItemPanel.Visible)
            {
                AddItemPanel.Visible = false;
                CreateNewFoodBtnPanel.Visible = true;
                AddItemBoxClear();
            }

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
                    if (selectedImage.Width == 128 && selectedImage.Height == 128)
                    {
                        AddItemPicBox.Image = selectedImage;
                    }
                    else if (selectedImage.Width != 128 && selectedImage.Height != 128)
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Please select an image for this item.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                        string insertQuery = "INSERT INTO foodmenu (FoodPic, FoodName, FoodCode, FoodType, FoodPrice, FoodDateCreated)" +
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
                    MessageBox.Show("Food item successfully created.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            AddItemTypeComboBox.SelectedIndex = -1;
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

        private void UpdateItemExitBtn_Click_1(object sender, EventArgs e)
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

        private void UpdateItemPicBox_Click(object sender, EventArgs e)
        {

        }

        private void UpdateItemPicBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Load the selected image into the PictureBox
                    selectedImage = Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (selectedImage.Width == 128 && selectedImage.Height == 128)
                    {
                        UpdateItemPicBox.Image = selectedImage;
                        return;
                    }
                    else if (selectedImage.Width != 128 && selectedImage.Height != 128)
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    }
                    else
                    {
                        MessageBox.Show("Please select an image with dimensions of 128x128 pixels.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateItemBtn_Click(object sender, EventArgs e)
        {
            //Update Acc Btn
            DateTime selectedDate = UpdateItemCreatedDatePicker.Value;

            string itemName = UpdateItemNameBox.Text;
            string itemCode = UpdateItemCodeBox.Text;
            string itemType = UpdateItemTypeComboBox.Text;
            string itemPrice = UpdateItemPriceBox.Text;
            string itemCreated = selectedDate.ToString("MM-dd-yyyy dddd");
            
            Regex nameRegex = new Regex("^[A-Z][a-zA-Z]+(?: [a-zA-Z]+)*$");

            if (string.IsNullOrEmpty(itemName) || string.IsNullOrEmpty(itemCode) || string.IsNullOrEmpty(itemType) ||
                string.IsNullOrEmpty(itemPrice) || string.IsNullOrEmpty(itemCreated))
            {
                MessageBox.Show("Missing text in required fields.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return; // Exit the method since there's an error
            }
            // Validate fields using regex patterns
            else if (!nameRegex.IsMatch(itemName))
            {
                MessageBox.Show("Item Name must start with a capital letter and only contain alphabetic values.");
                return;
            }
            // Check if an image has been selected
            else if (UpdateItemPicBox.Image == null)
            {
                MessageBox.Show("Please select an image for this item.", "Image Missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else if (string.IsNullOrEmpty(itemCode))
            {
                MessageBox.Show("Item Code is required to update an item.", "Missing Item Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            else
            {
                byte[] imageData = null;

                // Check if the image in the PictureBox has been modified
                if (selectedImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        selectedImage.Save(ms, ImageFormat.Jpeg); // Replace with the correct image format
                        imageData = ms.ToArray();
                    }
                }

                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        // Check if the item with the given Item Code exists
                        string checkExistQuery = "SELECT COUNT(*) FROM foodmenu WHERE FoodCode = @itemCode";
                        MySqlCommand checkExistCmd = new MySqlCommand(checkExistQuery, connection);
                        checkExistCmd.Parameters.AddWithValue("@itemCode", itemCode);
                        int itemCount = Convert.ToInt32(checkExistCmd.ExecuteScalar());

                        if (itemCount == 0)
                        {
                            MessageBox.Show("Item with the provided CODE does not exist in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        // Update data in the foodmenu table, including the image conditionally
                        if (imageData != null)
                        {
                            // Update with image
                            string updateWithImageQuery = "UPDATE foodmenu SET FoodName = @itemName, FoodType = @itemType, FoodPrice = @itemPrice, FoodPic = @imageData WHERE FoodCode = @itemCode";
                            MySqlCommand updateWithImageCmd = new MySqlCommand(updateWithImageQuery, connection);
                            updateWithImageCmd.Parameters.AddWithValue("@itemName", itemName);
                            updateWithImageCmd.Parameters.AddWithValue("@itemCode", itemCode);
                            updateWithImageCmd.Parameters.AddWithValue("@itemType", itemType);
                            updateWithImageCmd.Parameters.AddWithValue("@itemPrice", itemPrice);
                            updateWithImageCmd.Parameters.AddWithValue("@imageData", imageData);
                            updateWithImageCmd.ExecuteNonQuery();
                            selectedImage = null;
                        }
                        else
                        {
                            // Update without image
                            string updateWithoutImageQuery = "UPDATE foodmenu SET FoodName = @itemName, FoodType = @itemType, FoodPrice = @itemPrice WHERE FoodCode = @itemCode";
                            MySqlCommand updateWithoutImageCmd = new MySqlCommand(updateWithoutImageQuery, connection);
                            updateWithoutImageCmd.Parameters.AddWithValue("@itemName", itemName);
                            updateWithoutImageCmd.Parameters.AddWithValue("@itemCode", itemCode);
                            updateWithoutImageCmd.Parameters.AddWithValue("@itemType", itemType);
                            updateWithoutImageCmd.Parameters.AddWithValue("@itemPrice", itemPrice);
                            updateWithoutImageCmd.ExecuteNonQuery();
                        }
                    }

                    // Successful update
                    MessageBox.Show("Item has been successfully updated.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateItemBoxClear();
                    LoadItemMenu();
                }
                catch (MySqlException ex)
                {
                    // Handle MySQL database exception
                    MessageBox.Show("MySQL Error: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                        connection.Close();
                }

            }
        }

        private void UpdateItemTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void UpdateItemCreatedDatePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void UpdateItemBoxClear()
        {
            UpdateItemPicBox.Image = null;
            UpdateItemNameBox.Text = "";
            UpdateItemCodeBox.Text = "";
            UpdateItemCreatedDatePicker.Value = DateTime.Now;
            UpdateItemTypeComboBox.SelectedIndex = -1;
            UpdateItemPriceBox.Text = "";

        }

        private void RetrieveItemDataFromDB(DataGridViewRow selectedRow)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    string ItemCode = selectedRow.Cells[3].Value.ToString();

                    string selectQuery = "SELECT * FROM foodmenu WHERE FoodCode = @ItemCode";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
                    selectCmd.Parameters.AddWithValue("@ItemCode", ItemCode);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string itemName = reader["FoodName"].ToString();
                            string itemCode = reader["FoodCode"].ToString();
                            string itemType = reader["FoodType"].ToString();
                            string itemPrice = reader["FoodPrice"].ToString();
                            string itemCreated = reader["FoodDateCreated"].ToString();

                            // Assuming emplBday is in the "MM-DD-YYYY DDDD" format
                            string dateFormat = "MM-dd-yyyy dddd";

                            if (DateTime.TryParseExact(itemCreated, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateValue))
                            {
                                UpdateItemCreatedDatePicker.Value = dateValue;
                            }
                            else
                            {
                                MessageBox.Show("Invalid date format in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            UpdateItemNameBox.Text = itemName;
                            UpdateItemCodeBox.Text = itemCode;
                            UpdateItemTypeComboBox.Text = itemType;
                            UpdateItemPriceBox.Text = itemPrice;
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

        private void RetrieveItemImageFromDB(DataGridViewRow selectedRow)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    string ItemCode = selectedRow.Cells[3].Value.ToString();

                    string selectQuery = "SELECT * FROM foodmenu WHERE FoodCode = @ItemCode";
                    MySqlCommand selectCmd = new MySqlCommand(selectQuery, connection);
                    selectCmd.Parameters.AddWithValue("@ItemCode", ItemCode);

                    using (MySqlDataReader reader = selectCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            // Check if the "AccountPfp" column contains a blob
                            if (!reader.IsDBNull(reader.GetOrdinal("FoodPic")))
                            {
                                byte[] imgData = (byte[])reader["FoodPic"];
                                using (MemoryStream ms = new MemoryStream(imgData))
                                {
                                    // Display the image in the PictureBox control
                                    UpdateItemPicBox.Image = Image.FromStream(ms);
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

        private void MngrCreateNewOrderBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to create a new order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MngrPanelManager.MngrFormShow(MngrOrderDashboardPanel);
                MngrItemPanelManager.MngrItemFormShow(MngrItemBurgerPanel);
                MngrOrderPanelManager.MngrOrderFormShow(MngrOrderViewPanel);
                MngrOrderNumRefresh();
            }
        }

        private void MngrOrderExitBtn_Click(object sender, EventArgs e)
        {
            if (MngrOrderDashboardPanel.Visible)
            {
                DialogResult result = MessageBox.Show("Do you want to cancel this order?", "Order Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MngrOrderDashboardPanel.Visible = false;
                    MngrNewOrderBtnPanel.Visible = true;
                    MngrOrderViewTable.Rows.Clear();
                }

            }

            else
            {
                MngrOrderDashboardPanel.Visible = true;
                MngrNewOrderBtnPanel.Visible = false;
            }
        }

 
        private void MngrItemBurgerBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemBurgerPanel);

        }

        private void MngrItemSideBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemSidesPanel);

        }

        private void MngrItemDrinksBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemDrinksPanel);

        }

        private void MngrItemSetBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemSetMealsPanel);

        }

        private void MngrCheckoutBtn_Click(object sender, EventArgs e)
        {
            MngrOrderPanelManager.MngrOrderFormShow(MngrCheckoutViewPanel);

        }

        private void MngrPaymentButton_Click(object sender, EventArgs e)
        {

        }

        private void MngrCheckoutExitBtn_Click(object sender, EventArgs e)
        {
            if (MngrCheckoutViewPanel.Visible)
            {

                MngrCheckoutViewPanel.Visible = false;
                MngrOrderViewPanel.Visible = true;

            }

            else
            {
                MngrCheckoutViewPanel.Visible = true;
                MngrOrderViewPanel.Visible = false;
            }
        }

        private void MngrItemSidesView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Do you want to add this in the order?", "Add Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                    // Assuming you want to get data from the first and second columns of the selected row.
                    string cellValue1 = selectedRow.Cells[2].Value.ToString(); //Item Name
                    string cellValue3 = selectedRow.Cells[5].Value.ToString(); //Item Price

                    bool itemExists = false;

                    foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
                    {
                        if (row.Cells["ItemName"].Value != null && row.Cells["ItemName"].Value.ToString() == cellValue1)
                        {
                            itemExists = true;
                            break;
                        }
                    }

                    if (!itemExists)
                    {
                        MngrOrderViewTable.Rows.Add("T", cellValue1, "-", "1", "+", cellValue3);
                        //MngrOrderViewTable.CalculateTotalItemCost();
                    }
                    else
                    {
                        MessageBox.Show("Item already exists in the list.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MngrItemBurgerView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Do you want to add this in the order?", "Add Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                    // Assuming you want to get data from the first and second columns of the selected row.
                    string cellValue1 = selectedRow.Cells[2].Value.ToString(); //Item Name
                    string cellValue3 = selectedRow.Cells[5].Value.ToString(); //Item Price

                    bool itemExists = false;

                    foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
                    {
                        if (row.Cells["ItemName"].Value != null && row.Cells["ItemName"].Value.ToString() == cellValue1)
                        {
                            itemExists = true;
                            break;
                        }
                    }

                    if (!itemExists)
                    {
                        MngrOrderViewTable.Rows.Add("T", cellValue1, "-", "1", "+", cellValue3);
                        //MngrOrderViewTable.CalculateTotalItemCost();
                    }
                    else
                    {
                        MessageBox.Show("Item already exists in the list.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MngrItemDrinkView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Do you want to add this in the order?", "Add Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                    // Assuming you want to get data from the first and second columns of the selected row.
                    string cellValue1 = selectedRow.Cells[2].Value.ToString(); //Item Name
                    string cellValue3 = selectedRow.Cells[5].Value.ToString(); //Item Price

                    bool itemExists = false;

                    foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
                    {
                        if (row.Cells["ItemName"].Value != null && row.Cells["ItemName"].Value.ToString() == cellValue1)
                        {
                            itemExists = true;
                            break;
                        }
                    }

                    if (!itemExists)
                    {
                        MngrOrderViewTable.Rows.Add("T", cellValue1, "-", "1", "+", cellValue3);
                        //MngrOrderViewTable.CalculateTotalItemCost();
                    }
                    else
                    {
                        MessageBox.Show("Item already exists in the list.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MngrItemSetMealView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DialogResult result = MessageBox.Show("Do you want to add this in the order?", "Add Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    DataGridView dgv = (DataGridView)sender;
                    DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                    // Assuming you want to get data from the first and second columns of the selected row.
                    string cellValue1 = selectedRow.Cells[2].Value.ToString(); //Item Name
                    string cellValue3 = selectedRow.Cells[5].Value.ToString(); //Item Price

                    bool itemExists = false;

                    foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
                    {
                        if (row.Cells["ItemName"].Value != null && row.Cells["ItemName"].Value.ToString() == cellValue1)
                        {
                            itemExists = true;
                            break;
                        }
                    }

                    if (!itemExists)
                    {
                        MngrOrderViewTable.Rows.Add("T", cellValue1, "-", "1", "+", cellValue3);
                        //MngrOrderViewTable.CalculateTotalItemCost();
                    }
                    else
                    {
                        MessageBox.Show("Item already exists in the list.", "Duplicate Item", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
        }

        private void MngrOrderViewTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void SearchAccDB(string searchText)
        {
            connection.Open();
            // Modify your MySQL query to search in specific columns of the table
            string query = "SELECT * FROM accounts WHERE " +
                           "EmployeeName LIKE @searchText OR " +
                           "EmployeePosition LIKE @searchText OR " +
                           "EmployeeAge LIKE @searchText OR " +
                           "EmployeeBday LIKE @searchText OR " +
                           "EmployeeGender LIKE @searchText OR " +
                           "EmployeeAddress LIKE @searchText OR " +
                           "EmployeeEmail LIKE @searchText OR " +
                           "UID LIKE @searchText OR " +
                           "EmployeeID LIKE @searchText";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%"); // Adjust the parameter name and value accordingly

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataGridView (PendingTable) to the search results
                    AccountListTable.DataSource = dataTable;
                }
            }
            connection.Close();
        }

        private void SearchFoodDB(string searchText)
        {
            connection.Open();
            // Modify your MySQL query to search in specific columns of the table
            string query = "SELECT * FROM foodmenu WHERE " +
                           "FoodName LIKE @searchText OR " +
                           "FoodCode LIKE @searchText OR " +
                           "FoodType LIKE @searchText OR " +
                           "FoodPrice LIKE @searchText OR " +
                           "FoodDateCreated LIKE @searchText";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%"); // Adjust the parameter name and value accordingly

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataGridView (PendingTable) to the search results
                    FoodItemListTable.DataSource = dataTable;
                }
            }
            connection.Close();
        }

        private void AdminFoodItemSearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = AdminFoodItemSearchBox.Text;
            SearchFoodDB(searchText);
            SearchAccDB(searchText);

        }

        private void AdminFoodItemSearchBtn_Click(object sender, EventArgs e)
        {
            string searchText = AdminFoodItemSearchBox.Text;
            SearchFoodDB(searchText);
            SearchAccDB(searchText);

        }

        private void MngrSalesBtn_Click(object sender, EventArgs e)
        {
            MngrPanelManager.MngrFormShow(MngrSalesPanel);

        }

        private void MngrSalesExitBtn_Click(object sender, EventArgs e)
        {
            if (MngrSalesPanel.Visible)
            {
                MngrSalesPanel.Visible = false;
                MngrNewOrderBtnPanel.Visible = true;
            }

            else
            {
                MngrSalesPanel.Visible = true;
                MngrNewOrderBtnPanel.Visible = false;
            }
        }


    }
}
