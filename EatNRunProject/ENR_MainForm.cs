﻿using CafeDeLunaSystem;
using EatNRunProject.Properties;

using MySql.Data.MySqlClient;
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

using PdfSharp.Pdf;
using PdfSharp.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf;



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
        private CashierPanelCard CashierPanelManager;
        private CashierItemPanelCard CashierItemPanelManager;
        private CashierOrderPanelCard CashierOrderPanelManager;
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

        //Image Stored
        System.Drawing.Image ItemSelectedImage;
        System.Drawing.Image EmplSelectedImage;


        //Remember Account dictionary
        private Dictionary<string, string> accountData = new Dictionary<string, string>();

        //Ordered Discount Value
        private decimal originalGrossAmount; // Store the original value
        private bool discountApplied = false; // Flag to track if the discount has been applied

        
        //dgv
        private DataGridView CashierOrderView;
        private DataGridView MngrOrderView;

        public ENRMainForm()
        {
            InitializeComponent();

            //Mngr Order View
            MngrInitializeDataGridView();
            
            //Cashier Order View
            CashierInitializeDataGridView();


            //Main Form Panel Manager
            MFpanelManager = new MainFormCard(LoginPanel, AdminPanel, ManagerPanel, CashierPanel);

            //Admin Form Manager
            AdminPanelManager = new AdminPanelCard(FoodItemPanel, SalesPanel, AccountsPanel);
            AdminFoodPanelManager = new AdminFoodPanelCard(AddItemPanel, UpdateItemPanel, CreateNewFoodBtnPanel);
            AdminAccPanelManager = new AdminAccPanelCard(NewAccPanel, UpdateEmplAccPanel, CreateAccBtnPanel);

            //Mngr Form Manager
            MngrPanelManager = new MngrPanelCard(MngrNewOrderBtnPanel, MngrOrderDashboardPanel, MngrSalesPanel);
            MngrItemPanelManager = new MngrItemPanelCard(MngrItemBurgerPanel, MngrItemSidesPanel, MngrItemSetPanel, MngrItemDrinksPanel);
            MngrOrderPanelManager = new MngrOrderPanelCard(MngrOrderViewPanel, MngrCheckoutViewPanel);

            //Cashier Form Manager
            CashierPanelManager = new CashierPanelCard(CashierNewOrderBtnPanel, CashierOrderDashboardPanel);
            CashierItemPanelManager = new CashierItemPanelCard(CashierItemBurgerPanel, CashierItemSidesPanel, CashierItemSetPanel, CashierItemDrinksPanel);
            CashierOrderPanelManager = new CashierOrderPanelCard(CashierOrderViewPanel, CashierCheckoutViewPanel, CashierVoidViewPanel);

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
            CashierItemBurgerView.DataError += new DataGridViewDataErrorEventHandler(CashierFoodItemBurgerListTable_DataError);
            CashierItemBurgerView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(CashierFoodItemBurgerListTable_RowPostPaint);
            CashierItemSidesView.DataError += new DataGridViewDataErrorEventHandler(CashierFoodItemSideListTable_DataError);
            CashierItemSidesView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(CashierFoodItemSideListTable_RowPostPaint);
            CashierItemSetView.DataError += new DataGridViewDataErrorEventHandler(CashierFoodItemSetListTable_DataError);
            CashierItemSetView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(CashierFoodItemSetListTable_RowPostPaint);
            CashierItemDrinksView.DataError += new DataGridViewDataErrorEventHandler(CashierFoodItemDrinkListTable_DataError);
            CashierItemDrinksView.RowPostPaint += new DataGridViewRowPostPaintEventHandler(CashierFoodItemDrinkListTable_RowPostPaint);


            //DGVs
            MngrOrderView = MngrOrderViewTable; // Replace yourDataGridView with the actual DataGridView instance
            CashierOrderView = CashierOrderViewTable; // Replace yourDataGridView with the actual DataGridView instance

            MngrSalesStartDatePicker.ValueChanged += MngrSalesStartDatePicker_ValueChanged;
            MngrSalesEndDatePicker.ValueChanged += MngrSalesEndDatePicker_ValueChanged;


            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

        }

        private void ENR_MainForm_Load(object sender, EventArgs e)
        {
            this.Size = new System.Drawing.Size(517, 545);
            
            DBRefresher();

            DateTimePickerTimer.Interval = 1000;
            DateTimePickerTimer.Start();
        }

        private void DBRefresher()
        {
            LoadEmployeeAcc();
            LoadItemMenu();
            LoadBurgerItemMenu();
            LoadSideItemMenu();
            LoadDrinksItemMenu();
            LoadSetItemMenu();
            CashierLoadBurgerItemMenu();
            CashierLoadSideItemMenu();
            CashierLoadDrinksItemMenu();
            CashierLoadSetItemMenu();
            MngrLoadSalesDB();
            MngrLoadOrderHistoryDB();
            string bestSellerName = GetBestSellingItemForToday();
            MngrBestSellerBox.Text = bestSellerName;
        
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

        // DGV Error Handlers
        private void HandleDataError(DataGridView dgv, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                dgv[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }

        private void HandleRowPostPaint(DataGridView dgv, DataGridViewRowPostPaintEventArgs e)
        {
            dgv.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }

        // Account List Table
        private void AccountListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataError(AccountListTable, e);
        }

        private void AccountListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            HandleRowPostPaint(AccountListTable, e);
        }

        // Food Item List Table
        private void FoodItemListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            HandleDataError(FoodItemListTable, e);
        }

        private void FoodItemListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            HandleRowPostPaint(FoodItemListTable, e);
        }

        // Mngr DGV Error Handlers (Burger, Side, Set, Drinks)
        private void MngrDGVDataError(DataGridView dgv, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                dgv[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }

        private void MngrDGVRowPostPaint(DataGridView dgv, DataGridViewRowPostPaintEventArgs e)
        {
            dgv.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }

        // Burger List Table
        private void FoodItemBurgerListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MngrDGVDataError(MngrItemBurgerView, e);
        }

        private void FoodItemBurgerListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrDGVRowPostPaint(MngrItemBurgerView, e);
        }

        // Side List Table
        private void FoodItemSideListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MngrDGVDataError(MngrItemSidesView, e);
        }

        private void FoodItemSideListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrDGVRowPostPaint(MngrItemSidesView, e);
        }

        // Set List Table
        private void FoodItemSetListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MngrDGVDataError(MngrItemSetMealView, e);
        }

        private void FoodItemSetListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrDGVRowPostPaint(MngrItemSetMealView, e);
        }

        // Drinks List Table
        private void FoodItemDrinkListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MngrDGVDataError(MngrItemDrinkView, e);
        }

        private void FoodItemDrinkListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            MngrDGVRowPostPaint(MngrItemDrinkView, e);
        }

        private void CashierDGVDataError(DataGridView dgv, DataGridViewDataErrorEventArgs e)
        {
            if (e.ColumnIndex == 0) // Assuming column index for "AccountPfp" is 1
            {
                // Set the cell value to null to display an empty cell
                e.ThrowException = false;
                dgv[e.ColumnIndex, e.RowIndex].Value = null;
            }
        }

        private void CashierDGVRowPostPaint(DataGridView dgv, DataGridViewRowPostPaintEventArgs e)
        {
            dgv.AutoResizeRow(e.RowIndex, DataGridViewAutoSizeRowMode.AllCells);
        }

        // Burger List Table
        private void CashierFoodItemBurgerListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            CashierDGVDataError(CashierItemBurgerView, e);
        }

        private void CashierFoodItemBurgerListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            CashierDGVRowPostPaint(CashierItemBurgerView, e);
        }

        // Side List Table
        private void CashierFoodItemSideListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            CashierDGVDataError(CashierItemSidesView, e);
        }

        private void CashierFoodItemSideListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            CashierDGVRowPostPaint(CashierItemSidesView, e);
        }

        // Set List Table
        private void CashierFoodItemSetListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            CashierDGVDataError(CashierItemSetView, e);
        }

        private void CashierFoodItemSetListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            CashierDGVRowPostPaint(CashierItemSetView, e);
        }

        // Drinks List Table
        private void CashierFoodItemDrinkListTable_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            CashierDGVDataError(CashierItemDrinksView, e);
        }

        private void CashierFoodItemDrinkListTable_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            CashierDGVRowPostPaint(CashierItemDrinksView, e);
        }



        //Order View Table
        private void MngrInitializeDataGridView()
        {
            //DataGridViewButtonColumn trashColumn = new DataGridViewButtonColumn();
            //trashColumn.Name = "Void";
            //trashColumn.Text = "T";
            //trashColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //trashColumn.Width = 10;
            //MngrOrderViewTable.Columns.Add(trashColumn);

            DataGridViewTextBoxColumn itemNameColumn = new DataGridViewTextBoxColumn();
            itemNameColumn.Name = "Item Name";
            //itemNameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
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

            DataGridViewTextBoxColumn itemUnitCostColumn = new DataGridViewTextBoxColumn();
            itemUnitCostColumn.Name = "Unit Price";
            MngrOrderViewTable.Columns.Add(itemUnitCostColumn);

            DataGridViewTextBoxColumn itemCostColumn = new DataGridViewTextBoxColumn();
            itemCostColumn.Name = "Total Price";
            MngrOrderViewTable.Columns.Add(itemCostColumn);

        }

        private void CashierInitializeDataGridView()
        {
            //DataGridViewButtonColumn trashColumn = new DataGridViewButtonColumn();
            //trashColumn.Name = "Void";
            //trashColumn.Text = "T";
            //trashColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            //trashColumn.Width = 10;
            //MngrOrderViewTable.Columns.Add(trashColumn);

            DataGridViewTextBoxColumn itemNameColumn = new DataGridViewTextBoxColumn();
            itemNameColumn.Name = "Item Name";
            CashierOrderViewTable.Columns.Add(itemNameColumn);

            DataGridViewButtonColumn minusColumn = new DataGridViewButtonColumn();
            minusColumn.Name = "-";
            minusColumn.Text = "-";
            minusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            minusColumn.Width = 10;
            CashierOrderViewTable.Columns.Add(minusColumn);

            DataGridViewTextBoxColumn quantityColumn = new DataGridViewTextBoxColumn();
            quantityColumn.Name = "Qty";
            quantityColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            quantityColumn.Width = 15;
            CashierOrderViewTable.Columns.Add(quantityColumn);

            DataGridViewButtonColumn plusColumn = new DataGridViewButtonColumn();
            plusColumn.Name = "+";
            plusColumn.Text = "+";
            plusColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            plusColumn.Width = 10;
            CashierOrderViewTable.Columns.Add(plusColumn);
          
            DataGridViewTextBoxColumn itemUnitCostColumn = new DataGridViewTextBoxColumn();
            itemUnitCostColumn.Name = "Unit Price";
            CashierOrderViewTable.Columns.Add(itemUnitCostColumn);

            DataGridViewTextBoxColumn itemCostColumn = new DataGridViewTextBoxColumn();
            itemCostColumn.Name = "Total Price";
            CashierOrderViewTable.Columns.Add(itemCostColumn);
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
                        AccountListTable.Columns[0].Visible = false; 
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
                        FoodItemListTable.Columns[0].Visible = false; 
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

        public void CashierLoadBurgerItemMenu()
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
                        CashierItemBurgerView.Columns.Clear();

                        // Add the image column to the DataGridView
                        CashierItemBurgerView.Columns.Add(imageColumn);

                        CashierItemBurgerView.DataSource = dataTable;

                        CashierItemBurgerView.Columns[0].Visible = false;
                        CashierItemBurgerView.Columns[4].Visible = false;
                        CashierItemBurgerView.Columns[6].Visible = false;
                        CashierItemBurgerView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        public void CashierLoadSideItemMenu()
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
                        CashierItemSidesView.Columns.Clear();

                        // Add the image column to the DataGridView
                        CashierItemSidesView.Columns.Add(imageColumn);

                        CashierItemSidesView.DataSource = dataTable;

                        CashierItemSidesView.Columns[0].Visible = false;
                        CashierItemSidesView.Columns[4].Visible = false;
                        CashierItemSidesView.Columns[6].Visible = false;
                        CashierItemSidesView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        public void CashierLoadDrinksItemMenu()
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
                        CashierItemDrinksView.Columns.Clear();

                        // Add the image column to the DataGridView
                        CashierItemDrinksView.Columns.Add(imageColumn);

                        CashierItemDrinksView.DataSource = dataTable;

                        CashierItemDrinksView.Columns[0].Visible = false;
                        CashierItemDrinksView.Columns[4].Visible = false;
                        CashierItemDrinksView.Columns[6].Visible = false;
                        CashierItemDrinksView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        public void CashierLoadSetItemMenu()
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
                        CashierItemSetView.Columns.Clear();

                        // Add the image column to the DataGridView
                        CashierItemSetView.Columns.Add(imageColumn);

                        CashierItemSetView.DataSource = dataTable;

                        CashierItemSetView.Columns[0].Visible = false;
                        CashierItemSetView.Columns[4].Visible = false;
                        CashierItemSetView.Columns[6].Visible = false;
                        CashierItemSetView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

        private DataTable salesData = new DataTable(); // Declare a class-level variable

        public void MngrLoadSalesDB()
        {
            try
            {

                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();
                    string sql = "SELECT * FROM `sales`";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(salesData);

                        MngrSalesTable.DataSource = salesData;
                        MngrSalesTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
        }


        public void MngrLoadOrderHistoryDB()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();
                    string sql = "SELECT * FROM `orderhistory`";
                    MySqlCommand cmd = new MySqlCommand(sql, connection);
                    System.Data.DataTable dataTable = new System.Data.DataTable();

                    using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                    {
                        adapter.Fill(dataTable);

                        MngrOrderHistoryTable.DataSource = dataTable;

                        MngrOrderHistoryTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
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
        }


        private string GetBestSellingItemForToday()
        {
            string bestSellerName = "No best-selling item found for today";
            string currentDate = DateTime.Now.ToString("MM-dd-yyyy");

            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    string query = "SELECT ItemName, SUM(Qty) AS TotalQty " +
                                   "FROM orderhistory " +
                                   "WHERE DATE(Date) = @CurrentDate AND CheckedOut = 'Yes' AND Voided = 'No' " +
                                   "GROUP BY ItemName " +
                                   "ORDER BY TotalQty DESC " +
                                   "LIMIT 1;";

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@CurrentDate", currentDate);
                        MySqlDataReader reader = command.ExecuteReader();

                        if (reader.Read())
                        {
                            bestSellerName = reader["ItemName"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the database operation.
                // Example: MessageBox.Show("An error occurred: " + ex.Message);
            }

            return bestSellerName;
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
                RmbrAccCheckbox.Checked = true;
                MFpanelManager.MFShow(AdminPanel);
                AdminPanelManager.AdminFormShow(FoodItemPanel);
                AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                rememberAccount();
                logincredclear();
                AdminSalesStartDatePicker.Visible = false;
                this.Size = new System.Drawing.Size(1280, 720);
                return;
            }
            else if (ENREmplIDBox.Text == "Manager" && ENREmplPassBox.Text == "Manager123")
            {
                MessageBox.Show("Welcome back, Manager.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RmbrAccCheckbox.Checked = true;
                MngrNameBox.Text = "Test Manager";
                MFpanelManager.MFShow(ManagerPanel);
                MngrPanelManager.MngrFormShow(MngrNewOrderBtnPanel);
                MngrSessionNumRefresh();
                rememberAccount();
                logincredclear();
                this.Size = new System.Drawing.Size(1280, 720);

                return;
            }
            else if (ENREmplIDBox.Text == "Cashier" && ENREmplPassBox.Text == "Cashier123")
            {
                MessageBox.Show("Welcome back, Cashier.", "Login Verified", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RmbrAccCheckbox.Checked = true;
                CashierNameBox.Text = "Test Cashier";
                MFpanelManager.MFShow(CashierPanel);
                CashierPanelManager.CashierFormShow(CashierNewOrderBtnPanel);
                CashierSessionNumRefresh();
                rememberAccount();
                logincredclear();
                this.Size = new System.Drawing.Size(1280, 720);

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
                                        this.Size = new System.Drawing.Size(1280, 720);

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
                                        RmbrAccCheckbox.Checked = true;
                                        MFpanelManager.MFShow(ManagerPanel);
                                        MngrPanelManager.MngrFormShow(MngrNewOrderBtnPanel);
                                        MngrNameBox.Text = name;
                                        MngrSessionNumRefresh();
                                        rememberAccount();
                                        logincredclear();
                                        this.Size = new System.Drawing.Size(1280, 720);

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
                                        CashierPanelManager.CashierFormShow(CashierNewOrderBtnPanel);
                                        CashierNameBox.Text = name;
                                        CashierSessionNumRefresh();
                                        rememberAccount();
                                        logincredclear();
                                        this.Size = new System.Drawing.Size(1280, 720);
                                        return;


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

        private void MngrSessionNumRefresh()
        {
            MngrSessionNumBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            MngrSessionNumBox.Text = ID;
        }

        private void MngrOrderNumRefresh()
        {
            MngrOrderNumBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            MngrOrderNumBox.Text = ID;
        }

        private void CashierSessionNumRefresh()
        {
            CashierSessionNumBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            CashierSessionNumBox.Text = ID;
        }

        private void CashierOrderNumRefresh()
        {
            CashierOrderNumBox.Text = "";
            ID = RandomNumberGenerator.GenerateRandomNumber();
            CashierOrderNumBox.Text = ID;
        }


        private void logincredclear()
        {
            ENREmplIDBox.Text = "";
            ENREmplPassBox.Text = "";
            RmbrAccCheckbox.Checked = false;
            ENREmplShowPass.Checked = false;
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
                this.Size = new System.Drawing.Size(517, 545);

                MFpanelManager.MFShow(LoginPanel);

            }
        }

        private void ManagerSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Size = new System.Drawing.Size(517, 545);
                MFpanelManager.MFShow(LoginPanel);
                MngrOrderViewTable.Rows.Clear();
                MngrItemPanel.Enabled = true;

            }
            else
            {

            }
        }

        private void CashierSwitchBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Size = new System.Drawing.Size(517, 545);
                MFpanelManager.MFShow(LoginPanel);
                CashierOrderViewTable.Rows.Clear();
                CashierItemPanel.Enabled = true;

            }
            else
            {

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


        private void EditFoodItemBtn_Click(object sender, EventArgs e)
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

        private void EditEmplAccBtn_Click(object sender, EventArgs e)
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
                                    UpdateEmplPicBox.Image = System.Drawing.Image.FromStream(ms);
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

        private void AddEmplAccBtn_Click(object sender, EventArgs e)
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
                    System.Drawing.Image selectedImage = System.Drawing.Image.FromFile(openFileDialog.FileName);

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
                    EmplSelectedImage = System.Drawing.Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (EmplSelectedImage.Width == 128 && EmplSelectedImage.Height == 128)
                    {
                        UpdateEmplPicBox.Image = EmplSelectedImage;
                    }
                    else if (EmplSelectedImage.Width != 128 && EmplSelectedImage.Height != 128)
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


        private void UpdateEmplAccBtn_Click(object sender, EventArgs e)
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
                if (EmplSelectedImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        EmplSelectedImage.Save(ms, ImageFormat.Jpeg); // Replace with the correct image format
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
                            EmplSelectedImage = null;
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
                    System.Drawing.Image selectedImage = System.Drawing.Image.FromFile(openFileDialog.FileName);

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

        private void AddFoodItemBtn_Click(object sender, EventArgs e)
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
                    ItemSelectedImage = System.Drawing.Image.FromFile(openFileDialog.FileName);

                    // Check if the image dimensions are 64x64 pixels
                    if (ItemSelectedImage.Width == 128 && ItemSelectedImage.Height == 128)
                    {
                        UpdateItemPicBox.Image = ItemSelectedImage;
                        return;
                    }
                    else if (ItemSelectedImage.Width != 128 && ItemSelectedImage.Height != 128)
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

        private void UpdateFoodItemBtn_Click(object sender, EventArgs e)
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
                if (ItemSelectedImage != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ItemSelectedImage.Save(ms, ImageFormat.Jpeg); // Replace with the correct image format
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
                            ItemSelectedImage = null;
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
                                    UpdateItemPicBox.Image = System.Drawing.Image.FromStream(ms);
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
                MngrItemPanel.Enabled = true;
                MngrNetAmountBox.Text = "";
                MngrGrossAmountBox.Text = "";
                MngrVATBox.Text = "";
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
                    MngrItemPanel.Enabled = true;
                    MngrNetAmountBox.Text = "";
                    MngrGrossAmountBox.Text = "";
                    MngrVATBox.Text = "";

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
            DBRefresher();

        }

        private void MngrItemSideBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemSidesPanel);
            DBRefresher();

        }

        private void MngrItemDrinksBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemDrinksPanel);
            DBRefresher();

        }

        private void MngrItemSetBtn_Click(object sender, EventArgs e)
        {
            MngrItemPanelManager.MngrItemFormShow(MngrItemSetPanel);
            DBRefresher();

        }

        // Declare a field or property to hold the DataGridView


        // Constructor or initialization method


        private void MngrCheckoutBtn_Click(object sender, EventArgs e)
        {
            if (MngrOrderView != null && MngrOrderView.Rows.Count == 0)
            {
                MessageBox.Show("Select an item first to proceed to checkout.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                MngrOrderPanelManager.MngrOrderFormShow(MngrCheckoutViewPanel);
                MngrCalculateTotalPrice();
            }
        }



        private void MngrVoidBtn_Click(object sender, EventArgs e)
        {
            // Perform login verification before deleting the entire DataGridView

            if (MngrOrderView != null && MngrOrderView.Rows.Count == 0)
            {
                MessageBox.Show("Select an item first to void ordered items.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                DialogResult result = MessageBox.Show("Do you want to void the item(s) in the order?", "Item Void Order Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MngrItemPanel.Enabled = false;
                    MngrVoidOrderHistoryDB(MngrOrderView);
                    MngrOrderViewTable.Rows.Clear();
                    MessageBox.Show("Ordered items are voided.", "Item Void Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    MngrItemPanel.Enabled = true;
                }

            }

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
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                MngrDGVCellClick(dgv, selectedRow);
                MngrCalculateTotalPrice();
            }
        }

        private void MngrItemBurgerView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                MngrDGVCellClick(dgv, selectedRow);
                MngrCalculateTotalPrice();

            }
        }

        private void MngrItemDrinkView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                MngrDGVCellClick(dgv, selectedRow);
                MngrCalculateTotalPrice();

            }
        }

        private void MngrItemSetMealView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                MngrDGVCellClick(dgv, selectedRow);
                MngrCalculateTotalPrice();

            }
        }

        private void MngrDGVCellClick(DataGridView dgv, DataGridViewRow selectedRow)
        {
            string itemName = selectedRow.Cells[2].Value.ToString(); // Item Name

            bool itemExists = false;
            int existingRowIndex = -1;

            // Check if the item already exists in the order
            foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
            {
                if (row.Cells["Item Name"].Value != null && row.Cells["Item Name"].Value.ToString() == itemName)
                {
                    itemExists = true;
                    existingRowIndex = row.Index;
                    break;
                }
            }

            if (itemExists)
            {
                // The item already exists, increment quantity and update price
                string quantityString = MngrOrderViewTable.Rows[existingRowIndex].Cells["Qty"].Value?.ToString();
                if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                {
                    decimal itemCost = decimal.Parse(MngrOrderViewTable.Rows[existingRowIndex].Cells["Total Price"].Value?.ToString());

                    // Calculate the cost per item
                    decimal costPerItem = itemCost / quantity;

                    // Increase quantity
                    quantity++;

                    // Calculate updated item cost
                    decimal updatedCost = costPerItem * quantity;

                    // Update Qty and ItemCost in the DataGridView
                    MngrOrderViewTable.Rows[existingRowIndex].Cells["Qty"].Value = quantity.ToString();
                    MngrOrderViewTable.Rows[existingRowIndex].Cells["Total Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                    MngrCalculateTotalPrice();
                }
                else
                {
                    // Handle the case where quantityString is empty or not a valid integer
                    // For example, show an error message or set a default value
                }
            }
            else
            {
                // The item doesn't exist in the order, add it
                string itemPrice = selectedRow.Cells[5].Value.ToString(); // Item Price

                MngrOrderViewTable.Rows.Add(itemName, "-", "1", "+", itemPrice, itemPrice);
                MngrCalculateTotalPrice();

            }
        }
        private void MngrCalculateTotalPrice()
        {
            decimal total = 0;

            // Assuming the "Price" column is of decimal type
            int priceColumnIndex = MngrOrderViewTable.Columns["Total Price"].Index;

            foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
            {
                if (row.Cells[priceColumnIndex].Value != null)
                {
                    decimal price = decimal.Parse(row.Cells[priceColumnIndex].Value.ToString());
                    total += price;
                }
            }

            // Display the total price in the GrossAmountBox TextBox
            MngrCOGrossAmountBox.Text = total.ToString("F2"); // Format to two decimal places
            MngrGrossAmountBox.Text = total.ToString("F2"); // Format to two decimal places

            MngrCalculateVATAndNetAmount();
        }

        public void MngrCalculateVATAndNetAmount()
        {
            // Get the Gross Amount from the TextBox (MngrGrossAmountBox)
            if (decimal.TryParse(MngrCOGrossAmountBox.Text, out decimal grossAmount))
            {
                // Fixed VAT rate of 12%
                decimal rate = 12;

                // Calculate the VAT Amount
                decimal netAmount = grossAmount / ((rate / 100)+1);

                // Calculate the Net Amount
                decimal vatAmount = grossAmount - netAmount;

                // Display the calculated values in TextBoxes
                MngrCOVATBox.Text = vatAmount.ToString("0.00");
                MngrCONetAmountBox.Text = netAmount.ToString("0.00");
                MngrVATBox.Text = vatAmount.ToString("0.00");
                MngrNetAmountBox.Text = netAmount.ToString("0.00");
            }
            else
            {
                // Handle invalid Gross Amount input
                MessageBox.Show("Invalid Gross Amount. Please enter a valid number.");
            }
        }


        private void MngrOrderViewTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && MngrOrderViewTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                // Handle the Bin column
                if (MngrOrderViewTable.Columns[e.ColumnIndex].Name == "Void")
                {
                    // Remove the entire row
                    MngrOrderViewTable.Rows.RemoveAt(e.RowIndex);
                }
                else if (MngrOrderViewTable.Columns[e.ColumnIndex].Name == "-")
                {
                    string quantityString = MngrOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value?.ToString();
                    if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                    {
                        decimal itemCost = decimal.Parse(MngrOrderViewTable.Rows[e.RowIndex].Cells["Total Price"].Value?.ToString());

                        // Calculate the cost per item
                        decimal costPerItem = itemCost / quantity;

                        // Decrease quantity
                        if (quantity > 1)
                        {
                            quantity--;

                            // Calculate updated item cost (reset to original price)
                            decimal updatedCost = costPerItem * quantity;

                            // Update Qty and ItemCost in the DataGridView
                            MngrOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value = quantity.ToString();
                            MngrOrderViewTable.Rows[e.RowIndex].Cells["Total Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                        }
                    }
                    else
                    {
                        // Handle the case where quantityString is empty or not a valid integer
                        // For example, show an error message or set a default value
                    }
                }
                else if (MngrOrderViewTable.Columns[e.ColumnIndex].Name == "+")
                {
                    string quantityString = MngrOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value?.ToString();
                    if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                    {
                        decimal itemCost = decimal.Parse(MngrOrderViewTable.Rows[e.RowIndex].Cells["Total Price"].Value?.ToString());

                        // Calculate the cost per item
                        decimal costPerItem = itemCost / quantity;

                        // Increase quantity
                        quantity++;

                        // Calculate updated item cost
                        decimal updatedCost = costPerItem * quantity;

                        // Update Qty and ItemCost in the DataGridView
                        MngrOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value = quantity.ToString();
                        MngrOrderViewTable.Rows[e.RowIndex].Cells["Total Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                    }
                    else
                    {
                        // Handle the case where quantityString is empty or not a valid integer
                        // For example, show an error message or set a default value
                    }
                }
            }
        }



        private void MngrSalesBtn_Click(object sender, EventArgs e)
        {
            MngrPanelManager.MngrFormShow(MngrSalesPanel);
            DBRefresher();
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

        private void MngrSearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = MngrSearchBox.Text;
            SearchBurger(searchText);
            SearchDrinks(searchText);
            SearchSetMeals(searchText);
            SearchSides(searchText);
        }

        private void MngrSearchBoxBtn_Click(object sender, EventArgs e)
        {
            string searchText = MngrSearchBox.Text;
            SearchBurger(searchText);
            SearchDrinks(searchText);
            SearchSetMeals(searchText);
            SearchSides(searchText);
        }

        private void SearchFoodByFoodType(string searchText, string foodType)
        {
            connection.Open();
            // Modify your MySQL query to search in specific columns of the table for the specified food type
            string query = "SELECT * FROM `foodmenu` WHERE FoodType = @foodType AND " +
                           "(FoodName LIKE @searchText OR " +
                           "FoodCode LIKE @searchText OR " +
                           "FoodType LIKE @searchText OR " +
                           "FoodPrice LIKE @searchText OR " +
                           "FoodDateCreated LIKE @searchText)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@foodType", foodType);
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataGridView to the search results based on the food type
                    switch (foodType)
                    {
                        case "Burger":
                            MngrItemBurgerView.DataSource = dataTable;
                            break;
                        case "Sides":
                            MngrItemSidesView.DataSource = dataTable;
                            break;
                        case "Set Meals":
                            MngrItemSetMealView.DataSource = dataTable;
                            break;
                        case "Drinks":
                            MngrItemDrinkView.DataSource = dataTable;
                            break;
                        default:
                            // Handle an unknown food type or provide a default view
                            break;
                    }
                }
            }
            connection.Close();
        }

        // Separate methods for each food type
        private void SearchBurger(string searchText)
        {
            SearchFoodByFoodType(searchText, "Burger");
        }

        private void SearchSides(string searchText)
        {
            SearchFoodByFoodType(searchText, "Sides");
        }

        private void SearchSetMeals(string searchText)
        {
            SearchFoodByFoodType(searchText, "Set Meals");
        }

        private void SearchDrinks(string searchText)
        {
            SearchFoodByFoodType(searchText, "Drinks");
        }

        private void MngrDateTimePicker_ValueChanged(object sender, EventArgs e)
        {

        }

        private void DateTimePickerTimer_Tick(object sender, EventArgs e)
        {
            MngrDateTimePicker.Value = DateTime.Now;
            DateTime mngrcurrentDate = MngrDateTimePicker.Value;
            string mngrtoday = mngrcurrentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            
            CashierDateTimePicker.Value = DateTime.Now;
            DateTime cashierrcurrentDate = CashierDateTimePicker.Value;
            string Cashiertoday = cashierrcurrentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            CashierDateTimePickerBox.Text = Cashiertoday;


        }

        private void MngrDiscountSenior_CheckedChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(MngrCOGrossAmountBox.Text, out decimal grossAmount))
            {
                if (MngrDiscountSenior.Checked && !discountApplied)
                {
                    // Apply the 20% discount if the checkbox is checked and the discount hasn't been applied before
                    originalGrossAmount = grossAmount; // Store the original value
                    decimal discountPercentage = 20m;
                    decimal discountAmount = grossAmount * (discountPercentage / 100); // Calculate the discount amount
                    decimal discountedAmount = grossAmount - discountAmount; // Subtract the discount amount
                    MngrCOGrossAmountBox.Text = discountedAmount.ToString("0.00"); // Format to display as currency
                    discountApplied = true; // Set the flag to indicate that the discount has been applied
                    MngrDiscountBox.Text = discountAmount.ToString("0.00"); // Display the discount amount
                }
                else if (!MngrDiscountSenior.Checked && discountApplied)
                {
                    // Unchecked, set MngrGrossAmount to the original value if the discount has been applied before
                    MngrCOGrossAmountBox.Text = originalGrossAmount.ToString("0.00");
                    discountApplied = false; // Reset the flag
                    MngrDiscountBox.Text = "0.00"; // Reset the discount amount display
                }
                else
                {
                    // If the checkbox is checked but the discount has already been applied, update the discount amount display
                    decimal discountPercentage = 20m;
                    decimal discountAmount = originalGrossAmount * (discountPercentage / 100);
                    MngrDiscountBox.Text = discountAmount.ToString("0.00");
                }
            }
        }



        private void MngrDiscountPWD_CheckedChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(MngrCOGrossAmountBox.Text, out decimal grossAmount))
            {
                if (MngrDiscountSenior.Checked && !discountApplied)
                {
                    // Apply the 20% discount if the checkbox is checked and the discount hasn't been applied before
                    originalGrossAmount = grossAmount; // Store the original value
                    decimal discountPercentage = 20m;
                    decimal discountAmount = grossAmount * (discountPercentage / 100); // Calculate the discount amount
                    decimal discountedAmount = grossAmount - discountAmount; // Subtract the discount amount
                    MngrCOGrossAmountBox.Text = discountedAmount.ToString("0.00"); // Format to display as currency
                    discountApplied = true; // Set the flag to indicate that the discount has been applied
                    MngrDiscountBox.Text = discountAmount.ToString("0.00"); // Display the discount amount
                }
                else if (!MngrDiscountSenior.Checked && discountApplied)
                {
                    // Unchecked, set MngrGrossAmount to the original value if the discount has been applied before
                    MngrCOGrossAmountBox.Text = originalGrossAmount.ToString("0.00");
                    discountApplied = false; // Reset the flag
                    MngrDiscountBox.Text = "0.00"; // Reset the discount amount display
                }
                else
                {
                    // If the checkbox is checked but the discount has already been applied, update the discount amount display
                    decimal discountPercentage = 20m;
                    decimal discountAmount = originalGrossAmount * (discountPercentage / 100);
                    MngrDiscountBox.Text = discountAmount.ToString("0.00");
                }
            }
        }



        private void MngrGrossAmountBox_TextChanged(object sender, EventArgs e)
        {
            MngrCalculateVATAndNetAmount();
            if (decimal.TryParse(MngrCOGrossAmountBox.Text, out decimal grossAmount))
            {
                // Get the Cash Amount from the TextBox (MngrCashBox)
                if (decimal.TryParse(MngrCashBox.Text, out decimal cashAmount))
                {
                    // Calculate the Change
                    decimal change = cashAmount - grossAmount;

                    // Display the calculated change value in the MngrChangeBox
                    MngrChangeBox.Text = change.ToString("0.00");
                }
                else
                {
                    // Handle invalid input in MngrCashBox, e.g., display an error message
                    MngrChangeBox.Text = "Invalid Input";
                }
            }
            else
            {
                // Handle invalid input in MngrGrossAmountBox, e.g., display an error message
                MngrChangeBox.Text = "Invalid Input";
            }
        }

        private void MngrPaymentButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MngrCashBox.Text))
            {
                MessageBox.Show("Please add a valid amount of cash.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (decimal.TryParse(MngrChangeBox.Text, out decimal cash) && cash < 0)
            {
                MessageBox.Show("Please add a valid amount of cash.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                MngrGenerateReceipt();
                MngrPlaceOrderHistoryDB(MngrOrderViewTable);
                MngrPlaceOrderSalesDB();
            }
        }


        private void MngrPlaceOrderHistoryDB(DataGridView MngrOrderView)
        {
            // Assuming you have "MngrOrderNumBox" for OrderNumber and "MngrDateTimePicker" for Date
            string orderNum = MngrSessionNumBox.Text + "-" + MngrOrderNumBox.Text;
            DateTime currentDate = MngrDateTimePicker.Value;
            string today = currentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            string mngrName = MngrNameBox.Text;
            string yes = "Yes";
            string no = "No";

            if (MngrOrderViewTable.Rows.Count > 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        foreach (DataGridViewRow row in MngrOrderView.Rows)
                        {
                            if (row.Cells["Item Name"].Value != null)
                            {
                                string itemName = row.Cells["Item Name"].Value.ToString();
                                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                                decimal itemPrice = Convert.ToDecimal(row.Cells["Unit Price"].Value);
                                decimal itemTotalPrice = Convert.ToDecimal(row.Cells["Total Price"].Value);


                                string query = "INSERT INTO orderhistory (OrderNumber, Date, OrderedBy, ItemName, Qty, ItemPrice, ItemTotalPrice, CheckedOut, Voided) " +
                                               "VALUES (@OrderNumber, @Date, @OrderedBy, @ItemName, @Qty, @ItemPrice, @ItemTotalPrice, @Yes, @No)";

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@OrderNumber", orderNum);
                                    cmd.Parameters.AddWithValue("@Date", today);
                                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                                    cmd.Parameters.AddWithValue("@Qty", qty);
                                    cmd.Parameters.AddWithValue("@ItemPrice", itemPrice);
                                    cmd.Parameters.AddWithValue("@ItemTotalPrice", itemTotalPrice);
                                    cmd.Parameters.AddWithValue("@Yes", yes);
                                    cmd.Parameters.AddWithValue("@No", no);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("No items to insert into the database.");
            }
        }

        private void MngrPlaceOrderSalesDB()
        {
            DateTime currentDate = MngrDateTimePicker.Value;

            string orderNum = MngrSessionNumBox.Text + "-" + MngrOrderNumBox.Text;
            string today = currentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            string mngrName = MngrNameBox.Text;
            string netAmount = MngrCONetAmountBox.Text;
            string vat = MngrCOVATBox.Text;
            string grossAmount = MngrCOGrossAmountBox.Text;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Insert data into the accounts table, including the image (AccountPfp in the first position)
                    string insertQuery = "INSERT INTO sales (OrderNumber, Date, OrderedBy, NetAmount, VAT, GrossAmount)" +
                                        "VALUES (@OrderNum, @Date, @OrderedBy, @Net, @Vat, @Gross)";

                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);

                    cmd.Parameters.AddWithValue("@OrderNum", orderNum);
                    cmd.Parameters.AddWithValue("@Date", today);
                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                    cmd.Parameters.AddWithValue("@Net", netAmount);
                    cmd.Parameters.AddWithValue("@Vat", vat);
                    cmd.Parameters.AddWithValue("@Gross", grossAmount);


                    cmd.ExecuteNonQuery();
                }

                // Successful insertion
                MessageBox.Show("Order successfully placed.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //EmplIDRefresher();
                //AddNewAccBoxClear();
                //LoadEmployeeAcc();
                if (MngrCheckoutViewPanel.Visible)
                {

                    MngrCheckoutViewPanel.Visible = false;
                    MngrNewOrderBtnPanel.Visible = true;
                    MngrOrderViewPanel.Visible = true;
                    MngrOrderNumRefresh();
                    MngrOrderViewTable.Rows.Clear();

                }

                else
                {
                    MngrCheckoutViewPanel.Visible = true;
                    MngrOrderViewPanel.Visible = false;
                    MngrNewOrderBtnPanel.Visible = false;
                }
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


        private void MngrVoidOrderHistoryDB(DataGridView MngrOrderView)
        {
            // Assuming you have "MngrOrderNumBox" for OrderNumber and "MngrDateTimePicker" for Date
            string orderNum = MngrSessionNumBox.Text + "-" + MngrOrderNumBox.Text;
            DateTime currentDate = MngrDateTimePicker.Value;
            string today = currentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            string mngrName = MngrNameBox.Text;

            string yes = "Yes";
            string no = "No";

            if (MngrOrderViewTable.Rows.Count > 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        foreach (DataGridViewRow row in MngrOrderView.Rows)
                        {
                            if (row.Cells["Item Name"].Value != null)
                            {
                                string itemName = row.Cells["Item Name"].Value.ToString();
                                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                                decimal itemPrice = Convert.ToDecimal(row.Cells["Price"].Value);

                                string query = "INSERT INTO orderhistory (OrderNumber, Date, OrderedBy, ItemName, Qty, ItemPrice, CheckedOut, Voided) " +
                                               "VALUES (@OrderNumber, @Date, @OrderedBy, @ItemName, @Qty, @ItemPrice, @Yes, @No)";

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@OrderNumber", orderNum);
                                    cmd.Parameters.AddWithValue("@Date", today);
                                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                                    cmd.Parameters.AddWithValue("@Qty", qty);
                                    cmd.Parameters.AddWithValue("@ItemPrice", itemPrice);
                                    cmd.Parameters.AddWithValue("@Yes", no);
                                    cmd.Parameters.AddWithValue("@No", yes);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("No items to insert into the database.");
            }
        }


        private void MngrGenerateReceipt()
        {
            DateTime currentDate = MngrDateTimePicker.Value;
            string today = currentDate.ToString("MM-dd-yyyy hh:mm tt");
            string orderNumber = MngrOrderNumBox.Text;
            string staffName = MngrNameBox.Text;
            string legal = "Thank you for dining in Eat N' Run Burger Diner.\n" +
                "This receipt will serve as your proof of purchase of Eat N'Run food products, " +
                "in order to raise concerns about your purchase, please keep this receipt.";
            // Increment the file name

            // Generate a unique filename for the PDF
            string fileName = $"ENR_OrderReceipt_{orderNumber}.pdf";

            // Create a SaveFileDialog to choose the save location
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.FileName = fileName;
            BaseFont boldBaseFont = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font boldfont = new iTextSharp.text.Font(boldBaseFont, 6, iTextSharp.text.Font.BOLD);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                // Create a new document with custom page size (8.5"x4.25" in landscape mode)
                Document doc = new Document(new iTextSharp.text.Rectangle(Utilities.MillimetersToPoints(108f), Utilities.MillimetersToPoints(216f)));

                try
                {
                    // Create a PdfWriter instance
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

                    // Open the document for writing
                    doc.Open();

                    // Create fonts for the content
                    // Create fonts for the content
                    iTextSharp.text.Font headerFont = FontFactory.GetFont("Courier", 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font font = FontFactory.GetFont("Courier", 10, iTextSharp.text.Font.NORMAL);

                    // Create a centered alignment for text
                    Paragraph centerAligned = new Paragraph();
                    centerAligned.Alignment = Element.ALIGN_CENTER;

                    // Add centered content to the centerAligned Paragraph
                    centerAligned.Add(new Chunk("Eat N'Run Burger Diner", headerFont));
                    centerAligned.Add(new Chunk("\n123 Main St, Your City", font));
                    centerAligned.Add(new Chunk("\nPhone: (555) 555-5555", font));

                    // Add the centered content to the document
                    doc.Add(centerAligned);
                    doc.Add(new Chunk("\n")); // New line


                    doc.Add(new Paragraph($"Order Number: {orderNumber}", font));
                    doc.Add(new Paragraph($"Order Date: {today}", font));
                    doc.Add(new Paragraph($"Order Checked Out By: {staffName}", font));
                    doc.Add(new Chunk("\n")); // New line


                    // Iterate through the rows of your 
                    foreach (DataGridViewRow row in MngrOrderViewTable.Rows)
                    {
                        try
                        {
                            string itemName = row.Cells["Item Name"].Value?.ToString();
                            if (string.IsNullOrEmpty(itemName))
                            {
                                continue; // Skip empty rows
                            }

                            string qty = row.Cells["Qty"].Value?.ToString();
                            string itemcost = row.Cells["Price"].Value?.ToString();

                            doc.Add(new Paragraph($"{qty} | {itemName} | Php {itemcost}", font));
                            //MessageBox.Show($"{qty} | {itemName} | Php {itemcost}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        catch (Exception ex)
                        {
                            // Handle or log any exceptions that occur while processing DataGridView data
                            Console.WriteLine($"Error processing DataGridView row: {ex.Message}");
                        }
                    }


                    doc.Add(new Chunk("\n")); // New line

                    // Total from your textboxes as decimal
                    decimal netAmount = decimal.Parse(MngrCONetAmountBox.Text);
                    decimal vat = decimal.Parse(MngrCOVATBox.Text);
                    decimal grossAmount = decimal.Parse(MngrCOGrossAmountBox.Text);
                    decimal cash = decimal.Parse(MngrCashBox.Text);
                    decimal change = decimal.Parse(MngrChangeBox.Text);

                    doc.Add(new Paragraph($"Net Amount: Php{netAmount:F2}", font));
                    doc.Add(new Paragraph($"VAT (12%): Php{vat:F2}", font));
                    doc.Add(new Paragraph($"Gross Amount: Php{grossAmount:F2}", font));
                    doc.Add(new Paragraph($"Cash: Php{cash:F2}", font));
                    doc.Add(new Paragraph($"Change Due: Php{change:F2}", font));
                    Paragraph paragraph_footer = new Paragraph($"\n\n{legal}", boldfont);
                    paragraph_footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(paragraph_footer);
                }
                catch (DocumentException de)
                {
                    Console.Error.WriteLine(de.Message);
                }
                catch (IOException ioe)
                {
                    Console.Error.WriteLine(ioe.Message);
                }
                finally
                {
                    // Close the document
                    doc.Close();
                }

                MessageBox.Show($"Receipt saved as {filePath}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        private void MngrCashBox_TextChanged(object sender, EventArgs e)
        {
            // Get the Gross Amount from the TextBox (MngrGrossAmountBox)
            if (decimal.TryParse(MngrCOGrossAmountBox.Text, out decimal grossAmount))
            {
                // Get the Cash Amount from the TextBox (MngrCashBox)
                if (decimal.TryParse(MngrCashBox.Text, out decimal cashAmount))
                {
                    // Calculate the Change
                    decimal change = cashAmount - grossAmount;

                    // Display the calculated change value in the MngrChangeBox
                    MngrChangeBox.Text = change.ToString("0.00");
                }
                else
                {
                    // Handle invalid input in MngrCashBox, e.g., display an error message
                    MngrChangeBox.Text = "Invalid Input";
                }
            }
            else
            {
                // Handle invalid input in MngrGrossAmountBox, e.g., display an error message
                MngrChangeBox.Text = "Invalid Input";
            }
        }

        /*
         Cashier Start
        */

        private void CashierNewOrderBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to create a new order?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                CashierPanelManager.CashierFormShow(CashierOrderDashboardPanel);
                CashierItemPanelManager.CashierItemFormShow(CashierItemBurgerPanel);
                CashierOrderPanelManager.CashierOrderFormShow(CashierOrderViewPanel);
                CashierOrderNumRefresh();
                CashierItemPanel.Enabled = true;
            }
        }

        private void CashierItemBurgerBtn_Click(object sender, EventArgs e)
        {
            CashierItemPanelManager.CashierItemFormShow(CashierItemBurgerPanel);
            DBRefresher();
        }

        private void CashierItemSidesBtn_Click(object sender, EventArgs e)
        {
            CashierItemPanelManager.CashierItemFormShow(CashierItemSidesPanel);
            DBRefresher();
        }

        private void CashierItemDrinksBtn_Click(object sender, EventArgs e)
        {
            CashierItemPanelManager.CashierItemFormShow(CashierItemDrinksPanel);
            DBRefresher();
        }

        private void CashierItemSetBtn_Click(object sender, EventArgs e)
        {
            CashierItemPanelManager.CashierItemFormShow(CashierItemSetPanel);
            DBRefresher();
        }

        private void CashierOrderViewTable_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && CashierOrderViewTable.Columns[e.ColumnIndex] is DataGridViewButtonColumn)
            {
                // Handle the Bin column
                if (CashierOrderViewTable.Columns[e.ColumnIndex].Name == "Void")
                {
                    // Remove the entire row
                    CashierOrderViewTable.Rows.RemoveAt(e.RowIndex);
                }
                else if (CashierOrderViewTable.Columns[e.ColumnIndex].Name == "-")
                {
                    string quantityString = CashierOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value?.ToString();
                    if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                    {
                        decimal itemCost = decimal.Parse(CashierOrderViewTable.Rows[e.RowIndex].Cells["Price"].Value?.ToString());

                        // Calculate the cost per item
                        decimal costPerItem = itemCost / quantity;

                        // Decrease quantity
                        if (quantity > 1)
                        {
                            quantity--;

                            // Calculate updated item cost (reset to original price)
                            decimal updatedCost = costPerItem * quantity;

                            // Update Qty and ItemCost in the DataGridView
                            CashierOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value = quantity.ToString();
                            CashierOrderViewTable.Rows[e.RowIndex].Cells["Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                        }
                    }
                    else
                    {
                        // Handle the case where quantityString is empty or not a valid integer
                        // For example, show an error message or set a default value
                    }
                }
                else if (CashierOrderViewTable.Columns[e.ColumnIndex].Name == "+")
                {
                    string quantityString = CashierOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value?.ToString();
                    if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                    {
                        decimal itemCost = decimal.Parse(CashierOrderViewTable.Rows[e.RowIndex].Cells["Price"].Value?.ToString());

                        // Calculate the cost per item
                        decimal costPerItem = itemCost / quantity;

                        // Increase quantity
                        quantity++;

                        // Calculate updated item cost
                        decimal updatedCost = costPerItem * quantity;

                        // Update Qty and ItemCost in the DataGridView
                        CashierOrderViewTable.Rows[e.RowIndex].Cells["Qty"].Value = quantity.ToString();
                        CashierOrderViewTable.Rows[e.RowIndex].Cells["Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                    }
                    else
                    {
                        // Handle the case where quantityString is empty or not a valid integer
                        // For example, show an error message or set a default value
                    }
                }
            }
        }

        private void CashierItemSidesView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                CashierDGVCellClick(dgv, selectedRow);
            }
        }

        private void CashierItemBurgerView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                CashierDGVCellClick(dgv, selectedRow);
            }
        }

        private void CashierItemDrinksView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                CashierDGVCellClick(dgv, selectedRow);
            }
        }

        private void CashierItemSetView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridView dgv = (DataGridView)sender;
                DataGridViewRow selectedRow = dgv.Rows[e.RowIndex];

                // Call the method to handle the click event
                CashierDGVCellClick(dgv, selectedRow);
            }
        }

        private void CashierDGVCellClick(DataGridView dgv, DataGridViewRow selectedRow)
        {
            string itemName = selectedRow.Cells[2].Value.ToString(); // Item Name

            bool itemExists = false;
            int existingRowIndex = -1;

            // Check if the item already exists in the order
            foreach (DataGridViewRow row in CashierOrderViewTable.Rows)
            {
                if (row.Cells["Item Name"].Value != null && row.Cells["Item Name"].Value.ToString() == itemName)
                {
                    itemExists = true;
                    existingRowIndex = row.Index;
                    break;
                }
            }

            if (itemExists)
            {
                // The item already exists, increment quantity and update price
                string quantityString = CashierOrderViewTable.Rows[existingRowIndex].Cells["Qty"].Value?.ToString();
                if (!string.IsNullOrEmpty(quantityString) && int.TryParse(quantityString, out int quantity))
                {
                    decimal itemCost = decimal.Parse(CashierOrderViewTable.Rows[existingRowIndex].Cells["Total Price"].Value?.ToString());

                    // Calculate the cost per item
                    decimal costPerItem = itemCost / quantity;

                    // Increase quantity
                    quantity++;

                    // Calculate updated item cost
                    decimal updatedCost = costPerItem * quantity;

                    // Update Qty and ItemCost in the DataGridView
                    CashierOrderViewTable.Rows[existingRowIndex].Cells["Qty"].Value = quantity.ToString();
                    CashierOrderViewTable.Rows[existingRowIndex].Cells["Total Price"].Value = updatedCost.ToString("F2"); // Format to two decimal places
                    CashierCalculateTotalPrice();
                }
                else
                {
                    // Handle the case where quantityString is empty or not a valid integer
                    // For example, show an error message or set a default value
                }
            }
            else
            {
                // The item doesn't exist in the order, add it
                DialogResult result = MessageBox.Show("Do you want to add this in the order?", "Add Order", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    string itemPrice = selectedRow.Cells[5].Value.ToString(); // Item Price

                    CashierOrderViewTable.Rows.Add(itemName, "-", "1", "+", itemPrice, itemPrice);
                    CashierCalculateTotalPrice();
                }
            }
        }

        private void CashierCalculateTotalPrice()
        {
            decimal total = 0;

            // Assuming the "Price" column is of decimal type
            int priceColumnIndex = CashierOrderViewTable.Columns["Total Price"].Index;

            foreach (DataGridViewRow row in CashierOrderViewTable.Rows)
            {
                if (row.Cells[priceColumnIndex].Value != null)
                {
                    decimal price = decimal.Parse(row.Cells[priceColumnIndex].Value.ToString());
                    total += price;
                }
            }

            // Display the total price in the GrossAmountBox TextBox
            CashierGrossAmountBox.Text = total.ToString("F2"); // Format to two decimal places
            CashierCalculateVATAndNetAmount();
        }

        public void CashierCalculateVATAndNetAmount()
        {
            // Get the Gross Amount from the TextBox (MngrGrossAmountBox)
            if (decimal.TryParse(CashierGrossAmountBox.Text, out decimal grossAmount))
            {
                // Fixed VAT rate of 12%
                decimal rate = 12;

                // Calculate the VAT Amount
                decimal netAmount = grossAmount / ((rate / 100) + 1);

                // Calculate the Net Amount
                decimal vatAmount = grossAmount - netAmount;

                // Display the calculated values in TextBoxes
                CashierVATBox.Text = vatAmount.ToString("0.00");
                CashierNetAmountBox.Text = netAmount.ToString("0.00");
            }
            else
            {
                // Handle invalid Gross Amount input
                MessageBox.Show("Invalid Gross Amount. Please enter a valid number.");
            }
        }

        private void CashierCheckoutOrderBtn_Click(object sender, EventArgs e)
        {
            if (CashierOrderView != null && CashierOrderView.Rows.Count == 0)
            {
                MessageBox.Show("Select an item first to proceed to checkout.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                CashierOrderPanelManager.CashierOrderFormShow(CashierCheckoutViewPanel);
                CashierCalculateTotalPrice();
            }
        }

        private void CashierVoidBtn_Click(object sender, EventArgs e)
        {
            if (CashierOrderView != null && CashierOrderView.Rows.Count == 0)
            {
                MessageBox.Show("Select an item first to void ordered items.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                CashierOrderPanelManager.CashierOrderFormShow(CashierVoidViewPanel);
                CashierItemPanel.Enabled = false;
            }
        }

        private void CashierVoidExitBtn_Click(object sender, EventArgs e)
        {
            if (CashierVoidViewPanel.Visible)
            {

                CashierVoidViewPanel.Visible = false;
                CashierOrderViewPanel.Visible = true;
                CashierItemPanel.Enabled = true;
            }

            else
            {
                CashierVoidViewPanel.Visible = true;
                CashierOrderViewPanel.Visible = false;
            }
        }

        private void CashierSearchBox_TextChanged(object sender, EventArgs e)
        {
            string searchText = CashierSearchBox.Text;
            CashierSearchBurger(searchText);
            CashierSearchDrinks(searchText);
            CashierSearchSetMeals(searchText);
            CashierSearchSides(searchText);
        }

        private void CashierSearchBoxBtn_Click(object sender, EventArgs e)
        {

        }

        private void CashierSearchFoodByFoodType(string searchText, string foodType)
        {
            connection.Open();
            // Modify your MySQL query to search in specific columns of the table for the specified food type
            string query = "SELECT * FROM `foodmenu` WHERE FoodType = @foodType AND " +
                           "(FoodName LIKE @searchText OR " +
                           "FoodCode LIKE @searchText OR " +
                           "FoodType LIKE @searchText OR " +
                           "FoodPrice LIKE @searchText OR " +
                           "FoodDateCreated LIKE @searchText)";

            using (MySqlCommand cmd = new MySqlCommand(query, connection))
            {
                cmd.Parameters.AddWithValue("@foodType", foodType);
                cmd.Parameters.AddWithValue("@searchText", "%" + searchText + "%");

                using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
                {
                    DataTable dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    // Bind the DataGridView to the search results based on the food type
                    switch (foodType)
                    {
                        case "Burger":
                            CashierItemBurgerView.DataSource = dataTable;
                            break;
                        case "Sides":
                            CashierItemSidesView.DataSource = dataTable;
                            break;
                        case "Set Meals":
                            CashierItemSetView.DataSource = dataTable;
                            break;
                        case "Drinks":
                            CashierItemDrinksView.DataSource = dataTable;
                            break;
                        default:
                            // Handle an unknown food type or provide a default view
                            break;
                    }
                }
            }
            connection.Close();
        }

        // Separate methods for each food type
        private void CashierSearchBurger(string searchText)
        {
            CashierSearchFoodByFoodType(searchText, "Burger");
        }

        private void CashierSearchSides(string searchText)
        {
            CashierSearchFoodByFoodType(searchText, "Sides");
        }

        private void CashierSearchSetMeals(string searchText)
        {
            CashierSearchFoodByFoodType(searchText, "Set Meals");
        }

        private void CashierSearchDrinks(string searchText)
        {
            CashierSearchFoodByFoodType(searchText, "Drinks");
        }


        private void CashierVoidOrderBtn_Click(object sender, EventArgs e)
        {
            CashierOrderVoider();
        }

        private void CashierOrderVoider()
        {
            string emplPass = CashierVoidEmplPassBox.Text;
            string passchecker = HashHelper.HashString(emplPass);

            MySqlConnection connection = null;

            try
            {
                connection = new MySqlConnection(mysqlconn);
                connection.Open();

                // Query the database for any Manager with matching password
                string queryApproved = "SELECT EmployeeName FROM accounts WHERE EmployeePosition = 'Manager' AND HashedPass = @HashedPass";

                using (MySqlCommand cmdApproved = new MySqlCommand(queryApproved, connection))
                {
                    cmdApproved.Parameters.AddWithValue("@HashedPass", passchecker);

                    using (MySqlDataReader readerApproved = cmdApproved.ExecuteReader())
                    {
                        if (readerApproved.Read())
                        {
                            // Manager with matching password found
                            string name = readerApproved["EmployeeName"].ToString();

                            DialogResult result = MessageBox.Show("Do you want to void the item(s) in the order?", "Item Void Order Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                MessageBox.Show("Ordered items are voided.", "Item Void Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                CashierItemPanel.Enabled = true;
                                CashierVoidOrderHistoryDB(CashierOrderViewTable);
                                CashierOrderViewTable.Rows.Clear();
                                CashierOrderPanelManager.CashierOrderFormShow(CashierOrderViewPanel);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Incorrect Password or Account not found.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
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


        private void CashierVoidOrderHistoryDB(DataGridView MngrOrderView)
        {
            // Assuming you have "MngrOrderNumBox" for OrderNumber and "MngrDateTimePicker" for Date
            string orderNum = CashierSessionNumBox.Text + "-" + CashierOrderNumBox.Text;
            DateTime currentDate = CashierDateTimePicker.Value;
            string today = currentDate.ToString("MM-dd-yyyy dddd hh:mm tt");
            string mngrName = CashierNameBox.Text;

            string yes = "Yes";
            string no = "No";

            if (CashierOrderViewTable.Rows.Count > 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        foreach (DataGridViewRow row in CashierOrderView.Rows)
                        {
                            if (row.Cells["Item Name"].Value != null)
                            {
                                string itemName = row.Cells["Item Name"].Value.ToString();
                                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                                decimal itemPrice = Convert.ToDecimal(row.Cells["Price"].Value);

                                string query = "INSERT INTO orderhistory (OrderNumber, Date, OrderedBy, ItemName, Qty, ItemPrice, CheckedOut, Voided) " +
                                               "VALUES (@OrderNumber, @Date, @OrderedBy, @ItemName, @Qty, @ItemPrice, @Yes, @No)";

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@OrderNumber", orderNum);
                                    cmd.Parameters.AddWithValue("@Date", today);
                                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                                    cmd.Parameters.AddWithValue("@Qty", qty);
                                    cmd.Parameters.AddWithValue("@ItemPrice", itemPrice);
                                    cmd.Parameters.AddWithValue("@Yes", no);
                                    cmd.Parameters.AddWithValue("@No", yes);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("No items to insert into the database.");
            }
        }

        private void CashierDiscountSenior_CheckedChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(CashierGrossAmountBox.Text, out decimal grossAmount))
            {
                if (CashierDiscountSenior.Checked && !discountApplied)
                {
                    // Apply the 20% discount if the checkbox is checked and the discount hasn't been applied before
                    originalGrossAmount = grossAmount; // Store the original value
                    decimal discountedAmount = grossAmount * 0.8m; // 20% discount using decimal
                    CashierGrossAmountBox.Text = discountedAmount.ToString("0.00"); // Format to display as currency
                    discountApplied = true; // Set the flag to indicate that the discount has been applied
                }
                else if (!CashierDiscountSenior.Checked && discountApplied)
                {
                    // Unchecked, set MngrGrossAmount to the original value if the discount has been applied before
                    CashierGrossAmountBox.Text = originalGrossAmount.ToString("0.00");
                    discountApplied = false; // Reset the flag
                }
            }
        }

        private void CashierDiscountPWD_CheckedChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(CashierGrossAmountBox.Text, out decimal grossAmount))
            {
                if (CashierDiscountPWD.Checked && !discountApplied)
                {
                    // Apply the 20% discount if the checkbox is checked and the discount hasn't been applied before
                    originalGrossAmount = grossAmount; // Store the original value
                    decimal discountedAmount = grossAmount * 0.8m; // 20% discount using decimal
                    CashierGrossAmountBox.Text = discountedAmount.ToString("0.00"); // Format to display as currency
                    discountApplied = true; // Set the flag to indicate that the discount has been applied
                }
                else if (!CashierDiscountPWD.Checked && discountApplied)
                {
                    // Unchecked, set MngrGrossAmount to the original value if the discount has been applied before
                    CashierGrossAmountBox.Text = originalGrossAmount.ToString("0.00");
                    discountApplied = false; // Reset the flag
                }
            }
        }

        private void CashierGrossAmountBox_TextChanged(object sender, EventArgs e)
        {
            CashierCalculateVATAndNetAmount();
            if (decimal.TryParse(CashierGrossAmountBox.Text, out decimal grossAmount))
            {
                // Get the Cash Amount from the TextBox (MngrCashBox)
                if (decimal.TryParse(CashierCashBox.Text, out decimal cashAmount))
                {
                    // Calculate the Change
                    decimal change = cashAmount - grossAmount;

                    // Display the calculated change value in the MngrChangeBox
                    CashierChangeBox.Text = change.ToString("0.00");
                }
                else
                {
                    // Handle invalid input in MngrCashBox, e.g., display an error message
                    CashierChangeBox.Text = "Invalid Input";
                }
            }
            else
            {
                // Handle invalid input in MngrGrossAmountBox, e.g., display an error message
                CashierChangeBox.Text = "Invalid Input";
            }
        }

        private void CashierCashBox_TextChanged(object sender, EventArgs e)
        {
            if (decimal.TryParse(CashierGrossAmountBox.Text, out decimal grossAmount))
            {
                // Get the Cash Amount from the TextBox (MngrCashBox)
                if (decimal.TryParse(CashierCashBox.Text, out decimal cashAmount))
                {
                    // Calculate the Change
                    decimal change = cashAmount - grossAmount;

                    // Display the calculated change value in the MngrChangeBox
                    CashierChangeBox.Text = change.ToString("0.00");
                }
                else
                {
                    // Handle invalid input in MngrCashBox, e.g., display an error message
                    CashierChangeBox.Text = "Invalid Input";
                }
            }
            else
            {
                // Handle invalid input in MngrGrossAmountBox, e.g., display an error message
                CashierChangeBox.Text = "Invalid Input";
            }
        }

        private void CashierPlaceOrderBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(CashierCashBox.Text))
            {
                MessageBox.Show("Please add a valid amount of cash.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (decimal.TryParse(CashierChangeBox.Text, out decimal cash) && cash < 0)
            {
                MessageBox.Show("Please add a valid amount of cash.", "Ooooops!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                CashierGenerateReceipt();
                CashierPlaceOrderHistoryDB(MngrOrderViewTable);
                CashierPlaceOrderSalesDB();
            }
        }



        private void CashierGenerateReceipt()
        {
            DateTime currentDate = CashierDateTimePicker.Value;
            string today = currentDate.ToString("MM-dd-yyyy hh:mm tt");
            string orderNumber = CashierOrderNumBox.Text;
            string staffName = CashierNameBox.Text;
            string legal = "Thank you for dining in Eat N' Run Burger Diner.\n" +
                "This receipt will serve as your proof of purchase of Eat N'Run food products, " +
                "in order to raise concerns about your purchase, please keep this receipt.";
            // Increment the file name

            // Generate a unique filename for the PDF
            string fileName = $"ENR_OrderReceipt_{orderNumber}.pdf";

            // Create a SaveFileDialog to choose the save location
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF Files|*.pdf";
            saveFileDialog.FileName = fileName;
            BaseFont boldBaseFont = BaseFont.CreateFont(BaseFont.COURIER_BOLD, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
            iTextSharp.text.Font boldfont = new iTextSharp.text.Font(boldBaseFont, 6, iTextSharp.text.Font.BOLD);

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;

                // Create a new document with custom page size (8.5"x4.25" in landscape mode)
                Document doc = new Document(new iTextSharp.text.Rectangle(Utilities.MillimetersToPoints(108f), Utilities.MillimetersToPoints(216f)));

                try
                {
                    // Create a PdfWriter instance
                    PdfWriter writer = PdfWriter.GetInstance(doc, new FileStream(filePath, FileMode.Create));

                    // Open the document for writing
                    doc.Open();

                    // Create fonts for the content
                    // Create fonts for the content
                    iTextSharp.text.Font headerFont = FontFactory.GetFont("Courier", 16, iTextSharp.text.Font.BOLD);
                    iTextSharp.text.Font font = FontFactory.GetFont("Courier", 10, iTextSharp.text.Font.NORMAL);

                    // Create a centered alignment for text
                    Paragraph centerAligned = new Paragraph();
                    centerAligned.Alignment = Element.ALIGN_CENTER;

                    // Add centered content to the centerAligned Paragraph
                    centerAligned.Add(new Chunk("Eat N'Run Burger Diner", headerFont));
                    centerAligned.Add(new Chunk("\n123 Main St, Your City", font));
                    centerAligned.Add(new Chunk("\nPhone: (555) 555-5555", font));

                    // Add the centered content to the document
                    doc.Add(centerAligned);
                    doc.Add(new Chunk("\n")); // New line


                    doc.Add(new Paragraph($"Order Number: {orderNumber}", font));
                    doc.Add(new Paragraph($"Order Date: {today}", font));
                    doc.Add(new Paragraph($"Order Checked Out By: {staffName}", font));
                    doc.Add(new Chunk("\n")); // New line


                    // Iterate through the rows of your 
                    foreach (DataGridViewRow row in CashierOrderViewTable.Rows)
                    {
                        try
                        {
                            string itemName = row.Cells["Item Name"].Value?.ToString();
                            if (string.IsNullOrEmpty(itemName))
                            {
                                continue; // Skip empty rows
                            }

                            string qty = row.Cells["Qty"].Value?.ToString();
                            string itemcost = row.Cells["Price"].Value?.ToString();

                            doc.Add(new Paragraph($"{qty} | {itemName} | Php {itemcost}", font));
                            //MessageBox.Show($"{qty} | {itemName} | Php {itemcost}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        }
                        catch (Exception ex)
                        {
                            // Handle or log any exceptions that occur while processing DataGridView data
                            Console.WriteLine($"Error processing DataGridView row: {ex.Message}");
                        }
                    }


                    doc.Add(new Chunk("\n")); // New line

                    // Total from your textboxes as decimal
                    decimal netAmount = decimal.Parse(CashierNetAmountBox.Text);
                    decimal vat = decimal.Parse(CashierVATBox.Text);
                    decimal grossAmount = decimal.Parse(CashierGrossAmountBox.Text);
                    decimal cash = decimal.Parse(CashierCashBox.Text);
                    decimal change = decimal.Parse(CashierChangeBox.Text);

                    doc.Add(new Paragraph($"Net Amount: Php{netAmount:F2}", font));
                    doc.Add(new Paragraph($"VAT (12%): Php{vat:F2}", font));
                    doc.Add(new Paragraph($"Gross Amount: Php{grossAmount:F2}", font));
                    doc.Add(new Paragraph($"Cash: Php{cash:F2}", font));
                    doc.Add(new Paragraph($"Change Due: Php{change:F2}", font));
                    Paragraph paragraph_footer = new Paragraph($"\n\n{legal}", boldfont);
                    paragraph_footer.Alignment = Element.ALIGN_CENTER;
                    doc.Add(paragraph_footer);
                }
                catch (DocumentException de)
                {
                    Console.Error.WriteLine(de.Message);
                }
                catch (IOException ioe)
                {
                    Console.Error.WriteLine(ioe.Message);
                }
                finally
                {
                    // Close the document
                    doc.Close();
                }

                MessageBox.Show($"Receipt saved as {filePath}", "Receipt Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }




        private void CashierPlaceOrderHistoryDB(DataGridView MngrOrderView)
        {
            // Assuming you have "MngrOrderNumBox" for OrderNumber and "MngrDateTimePicker" for Date
            string orderNum = CashierSessionNumBox.Text + "-" + CashierOrderNumBox.Text;
            string today = CashierDateTimePickerBox.Text;
            string mngrName = CashierNameBox.Text;
            string yes = "Yes";
            string no = "No";

            if (CashierOrderViewTable.Rows.Count > 0)
            {
                try
                {
                    using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                    {
                        connection.Open();

                        foreach (DataGridViewRow row in CashierOrderViewTable.Rows)
                        {
                            if (row.Cells["Item Name"].Value != null)
                            {
                                string itemName = row.Cells["Item Name"].Value.ToString();
                                int qty = Convert.ToInt32(row.Cells["Qty"].Value);
                                decimal itemPrice = Convert.ToDecimal(row.Cells["Unit Price"].Value);
                                decimal itemTotalPrice = Convert.ToDecimal(row.Cells["Total Price"].Value);


                                string query = "INSERT INTO orderhistory (OrderNumber, Date, OrderedBy, ItemName, Qty, ItemPrice, ItemTotalPrice, CheckedOut, Voided) " +
                                               "VALUES (@OrderNumber, @Date, @OrderedBy, @ItemName, @Qty, @ItemPrice, @ItemTotalPrice, @Yes, @No)";

                                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                                {
                                    cmd.Parameters.AddWithValue("@OrderNumber", orderNum);
                                    cmd.Parameters.AddWithValue("@Date", today);
                                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                                    cmd.Parameters.AddWithValue("@ItemName", itemName);
                                    cmd.Parameters.AddWithValue("@Qty", qty);
                                    cmd.Parameters.AddWithValue("@ItemPrice", itemPrice);
                                    cmd.Parameters.AddWithValue("@ItemTotalPrice", itemTotalPrice);
                                    cmd.Parameters.AddWithValue("@Yes", yes);
                                    cmd.Parameters.AddWithValue("@No", no);

                                    cmd.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                MessageBox.Show("No items to insert into the database.");
            }
        }

        private void CashierPlaceOrderSalesDB()
        {
            string orderNum = CashierSessionNumBox.Text + "-" + CashierOrderNumBox.Text;
            string today = CashierDateTimePickerBox.Text;
            string mngrName = CashierNameBox.Text;
            string netAmount = CashierNetAmountBox.Text;
            string vat = CashierVATBox.Text;
            string grossAmount = CashierGrossAmountBox.Text;

            try
            {
                using (MySqlConnection connection = new MySqlConnection(mysqlconn))
                {
                    connection.Open();

                    // Insert data into the accounts table, including the image (AccountPfp in the first position)
                    string insertQuery = "INSERT INTO sales (OrderNumber, Date, OrderedBy, NetAmount, VAT, GrossAmount)" +
                                        "VALUES (@OrderNum, @Date, @OrderedBy, @Net, @Vat, @Gross)";

                    MySqlCommand cmd = new MySqlCommand(insertQuery, connection);

                    cmd.Parameters.AddWithValue("@OrderNum", orderNum);
                    cmd.Parameters.AddWithValue("@Date", today);
                    cmd.Parameters.AddWithValue("@OrderedBy", mngrName);
                    cmd.Parameters.AddWithValue("@Net", netAmount);
                    cmd.Parameters.AddWithValue("@Vat", vat);
                    cmd.Parameters.AddWithValue("@Gross", grossAmount);


                    cmd.ExecuteNonQuery();
                }

                // Successful insertion
                MessageBox.Show("Order successfully placed.", "Hooray!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //EmplIDRefresher();
                //AddNewAccBoxClear();
                //LoadEmployeeAcc();
                if (CashierCheckoutViewPanel.Visible)
                {

                    CashierCheckoutViewPanel.Visible = false;
                    CashierNewOrderBtnPanel.Visible = true;
                    CashierOrderViewPanel.Visible = true;
                    CashierOrderNumRefresh();
                    CashierOrderViewTable.Rows.Clear();

                }

                else
                {
                    CashierCheckoutViewPanel.Visible = true;
                    CashierOrderViewPanel.Visible = false;
                    CashierNewOrderBtnPanel.Visible = false;
                }
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

        private void CashierCheckoutExitBtn_Click(object sender, EventArgs e)
        {
            if (CashierCheckoutViewPanel.Visible)
            {

                CashierCheckoutViewPanel.Visible = false;
                CashierOrderViewPanel.Visible = true;

            }

            else
            {
                CashierCheckoutViewPanel.Visible = true;
                CashierOrderViewPanel.Visible = false;
            }
        }

        private void CashierOrderExitBtn_Click(object sender, EventArgs e)
        {
            if (CashierOrderDashboardPanel.Visible)
            {
                DialogResult result = MessageBox.Show("Do you want to cancel this order?", "Order Cancellation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CashierOrderDashboardPanel.Visible = false;
                    CashierNewOrderBtnPanel.Visible = true;
                    CashierOrderViewTable.Rows.Clear();
                    CashierItemPanel.Enabled = true;

                }

            }

            else
            {
                CashierOrderDashboardPanel.Visible = true;
                CashierNewOrderBtnPanel.Visible = false;
            }
        }

        private void MngrSalesStartDatePicker_ValueChanged(object sender, EventArgs e)
        {
            FilterAndSortDataGridView();
        }

        private void MngrSalesEndDatePicker_ValueChanged(object sender, EventArgs e)
        {
            FilterAndSortDataGridView();
        }

        private void FilterAndSortDataGridView()
        {
            DateTime startDate = MngrSalesStartDatePicker.Value.Date; // Get only the date part
            DateTime endDate = MngrSalesEndDatePicker.Value.Date;     // Get only the date part

            DataView dv = new DataView(salesData);

            if (startDate == endDate)
            {
                // If both date pickers have the same date, filter for that specific date
                dv.RowFilter = $"Date >= '{startDate:MM-dd-yyyy dddd hh:mm tt}' " +
                               $"AND Date <= '{startDate.AddDays(1):MM-dd-yyyy dddd hh:mm tt}'";
            }
            else
            {
                // If the dates are different, filter for the date range
                dv.RowFilter = $"Date >= '{startDate:MM-dd-yyyy dddd hh:mm tt}' " +
                               $"AND Date <= '{endDate.AddDays(1):MM-dd-yyyy dddd hh:mm tt}'";
            }

            // Sort the DataView by Date
            dv.Sort = "Date ASC";

            MngrSalesTable.DataSource = dv.ToTable();

            decimal totalSales = 0;
            foreach (DataRow row in dv.ToTable().Rows)
            {
                decimal grossAmount = Convert.ToDecimal(row["GrossAmount"]);
                totalSales += grossAmount;
            }

            MngrTotalSalesBox.Text = totalSales.ToString("0.00");
        }





    }
}
