using Syncfusion.Windows.Forms.Interop;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
            LoginpanelManager = new LoginPanelCard(UserSelector, LoginFormPanel);
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
                LoginFormPanel.Visible = true;
                UserLbl.Text = "Welcome back, Admin.";
            }

            else
            {
                UserSelector.Visible = true;
                LoginFormPanel.Visible = false;
                UserLbl.Text = "Welcome back, Admin.";

            }
        }

        private void ManagerPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (UserSelector.Visible)
            {
                UserSelector.Visible = false;
                LoginFormPanel.Visible = true;
                UserLbl.Text = "Welcome back, Manager.";
            }

            else
            {
                UserSelector.Visible = true;
                LoginFormPanel.Visible = false;
                UserLbl.Text = "Welcome back, Manager.";

            }
        }

        private void CashierPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (UserSelector.Visible)
            {
                UserSelector.Visible = false;
                LoginFormPanel.Visible = true;
                UserLbl.Text = "Welcome back, Cashier.";
            }

            else
            {
                UserSelector.Visible = true;
                LoginFormPanel.Visible = false;
                UserLbl.Text = "Welcome back, Cashier.";

            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (LoginFormPanel.Visible)
            {
                LoginFormPanel.Visible = false;
                UserSelector.Visible = true;
                UserLbl.Text = "";
            }

            else
            {
                LoginFormPanel.Visible = true;
                UserSelector.Visible = false;
                UserLbl.Text = "";

            }
        }

        private void LoginBtn_Click(object sender, EventArgs e)
        {
            string usernameInput = EmpIDBox.Text;
            string passwordInput = EmpPassBox.Text;

            switch (usernameInput)
            {
                case "Admin":
                    if (passwordInput == "Admin123")
                    {
                        MessageBox.Show("Login Successful");
                        MFpanelManager.MFShow(AdminPanel);
                        AdminPanelManager.AdminFormShow(FoodItemPanel);
                        AdminFoodPanelManager.AdminFoodFormShow(CreateNewFoodBtnPanel);
                        EmpIDBox.Text = "";
                        EmpPassBox.Text = "";

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
                        EmpIDBox.Text = "";
                        EmpPassBox.Text = "";
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
                        EmpIDBox.Text = "";
                        EmpPassBox.Text = "";
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
            EmpPassBox.UseSystemPasswordChar = !EmpShowPass.Checked;
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
    }
}
