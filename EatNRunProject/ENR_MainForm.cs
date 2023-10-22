using Syncfusion.Windows.Forms.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EatNRunProject
{
    public partial class ENR_MainForm : Form
    {
        //classes
        private MainFormCard MFpanelManager;
        private LoginPanelCard LoginpanelManager;
        private AdminPanelCard AdminPanelManager;
        private AdminFoodPanelCard AdminFoodPanelManager;
        private AdminAccPanelCard AdminAccPanelManager;

        public ENR_MainForm()
        {
            InitializeComponent();
            
            MFpanelManager = new MainFormCard(LoginPanel, AdminPanel, ManagerPanel, CashierPanel);
            LoginpanelManager = new LoginPanelCard(UserSelector, AdminLoginFormPanel, MngrLoginFormPanel, CashierLoginFormPanel);
            AdminPanelManager = new AdminPanelCard(FoodItemPanel, SalesPanel, AccountsPanel);
            AdminFoodPanelManager = new AdminFoodPanelCard(NewItemPanel, UpdateItemPanel, CreateNewFoodBtnPanel);
            AdminAccPanelManager = new AdminAccPanelCard(NewAccPanel, UpdateAccPanel, CreateAccBtnPanel);

            MFpanelManager.MFShow(LoginPanel);
            LoginpanelManager.LoginShow(UserSelector);


            this.FormClosing += new FormClosingEventHandler(MainForm_FormClosing);

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

        private void AdminPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (UserSelector.Visible)
            {
                UserSelector.Visible = false;
                AdminLoginFormPanel.Visible = true;
                AdminUserLbl.Text = "Welcome back, Admin.";
            }

            else
            {
                UserSelector.Visible = true;
                AdminLoginFormPanel.Visible = false;
                AdminUserLbl.Text = "Welcome back, Admin.";

            }
        }

        private void ManagerPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (UserSelector.Visible)
            {
                UserSelector.Visible = false;
                AdminLoginFormPanel.Visible = true;
                AdminUserLbl.Text = "Welcome back, Manager.";
            }

            else
            {
                UserSelector.Visible = true;
                AdminLoginFormPanel.Visible = false;
                AdminUserLbl.Text = "Welcome back, Manager.";

            }
        }

        private void CashierPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (UserSelector.Visible)
            {
                UserSelector.Visible = false;
                AdminLoginFormPanel.Visible = true;
                AdminUserLbl.Text = "Welcome back, Cashier.";
            }

            else
            {
                UserSelector.Visible = true;
                AdminLoginFormPanel.Visible = false;
                AdminUserLbl.Text = "Welcome back, Cashier.";

            }
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

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string usernameInput = AdminEmpIDBox.Text;
            string passwordInput = AdminEmpPassBox.Text;

            switch (usernameInput)
            {
                case "Admin":
                    if (passwordInput == "Admin123")
                    {
                        MessageBox.Show("Login Successful");
                        MFpanelManager.MFShow(AdminPanel);
                        AdminPanelManager.AdminFormShow(FoodItemPanel);
                        AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                        AdminEmpIDBox.Text = "";
                        AdminEmpPassBox.Text = "";

                    }
                    else
                    {
                        MessageBox.Show("Access Denied");
                    }
                    break;

                case "Manager":
                    if (passwordInput == "Manager123")
                    {
                        MessageBox.Show("Login Successful");
                        MFpanelManager.MFShow(ManagerPanel);
                        AdminEmpIDBox.Text = "";
                        AdminEmpPassBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Access Denied");
                    }
                    break;
                case "Cashier":
                    if (passwordInput == "Cashier123")
                    {
                        MessageBox.Show("Login Successful");
                        MFpanelManager.MFShow(CashierPanel);
                        AdminEmpIDBox.Text = "";
                        AdminEmpPassBox.Text = "";
                    }
                    else
                    {
                        MessageBox.Show("Access Denied");
                    }
                    break;
                default:
                    MessageBox.Show("Login Error");
                    break;
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
                LoginpanelManager.LoginShow(UserSelector);
            }
        }

        private void ManagerSwitchBtn_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Do you want to switch user?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                MFpanelManager.MFShow(LoginPanel);
                LoginpanelManager.LoginShow(UserSelector);
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
                LoginpanelManager.LoginShow(UserSelector);
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
            }
        }

        private void UpdateAccBtn_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to edit this account?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                AdminAccPanelManager.AdminAccFormShow(UpdateAccPanel);
            }
        }

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
            }
            else
            {
                CreateAccBtnPanel.Visible = false;
                NewAccPanel.Visible = true;
            }
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

        private void MngrEmplShowPass_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void MngrLoginBtn_Click(object sender, EventArgs e)
        {

        }

        private void MngrExitButton_Click(object sender, EventArgs e)
        {

        }
    }
}
