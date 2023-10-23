using System.Windows.Forms;

namespace EatNRunProject
{
    internal class MainFormCard
    {
        private Panel LoginPanel;
        private Panel AdminPanel;
        private Panel ManagerPanel;
        private Panel CashierPanel;

        public MainFormCard(Panel loginPanel, Panel adminPanel, Panel managerPanel, Panel cashierpanel)
        {
            LoginPanel = loginPanel;
            AdminPanel = adminPanel;
            ManagerPanel = managerPanel;
            CashierPanel = cashierpanel;
        }

        public void MFShow(Panel panelToShow)
        {
            LoginPanel.Hide();
            AdminPanel.Hide();
            ManagerPanel.Hide();
            CashierPanel.Hide();

            panelToShow.Show();
        }

       
    }
}

