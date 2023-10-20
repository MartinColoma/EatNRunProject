using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Forms;

namespace EatNRunProject
{
    public partial class EatnRun : Form
    {
        public EatnRun()
        {
            InitializeComponent();
        }

        private void EatnRun_Load(object sender, EventArgs e)
        {

        }

        private void AdminPB_Click(object sender, EventArgs e)
        {
            if (LoginPanel.Visible)
            {
                LoginPanel.Visible = false;
                UserPickerPanel.Visible = true;
                //WCPanel.Visible = false;
                //RegiPanel.Visible = true;
                //SNComboBox.Text = "";
                //PassBox.Text = "";
                UserLbl.Text = "Welcome back, Admin.";
            }

            else
            {
                LoginPanel.Visible = true;
                UserPickerPanel.Visible = false;
                UserLbl.Text = "Welcome back, Admin.";

            }
        }

        private void ManagerPB_Click(object sender, EventArgs e)
        {
            if (LoginPanel.Visible)
            {
                LoginPanel.Visible = false;
                UserPickerPanel.Visible = true;
                //WCPanel.Visible = false;
                //RegiPanel.Visible = true;
                //SNComboBox.Text = "";
                //PassBox.Text = "";
                UserLbl.Text = "Welcome back, Manager.";
            }

            else
            {
                LoginPanel.Visible = true;
                UserPickerPanel.Visible = false;
                UserLbl.Text = "Welcome back, Manager.";

            }
        }

        private void CashierPB_Click(object sender, EventArgs e)
        {
            if (LoginPanel.Visible)
            {
                LoginPanel.Visible = false;
                UserPickerPanel.Visible = true;
                //WCPanel.Visible = false;
                //RegiPanel.Visible = true;
                //SNComboBox.Text = "";
                //PassBox.Text = "";
                UserLbl.Text = "Welcome back, Cashier.";
            }

            else
            {
                LoginPanel.Visible = true;
                UserPickerPanel.Visible = false;
                UserLbl.Text = "Welcome back, Cashier.";

            }
        }

        private void exitBtn_Click(object sender, EventArgs e)
        {
            if (UserPickerPanel.Visible)
            {
                LoginPanel.Visible = true;
                UserPickerPanel.Visible = false;
                //WCPanel.Visible = false;
                //RegiPanel.Visible = true;
                //SNComboBox.Text = "";
                //PassBox.Text = "";
            }

            else
            {
                LoginPanel.Visible = false;
                UserPickerPanel.Visible = true;
            }
        }
    }
}
