using System.Windows.Forms;


namespace EatNRunProject
{
    internal class LoginPanelCard
    {

        private Panel AdminLoginForm;
        private Panel ManagerLoginForm;
        private Panel CashierLoginForm;

        private Panel UserSelect;

        public LoginPanelCard(Panel AdminLoginFormPanel, Panel ManagerLoginFormPanel, Panel CashierLoginFormPanel, Panel UserSelector)
        {

            AdminLoginForm = AdminLoginFormPanel;
            ManagerLoginForm = ManagerLoginFormPanel;
            CashierLoginForm = CashierLoginFormPanel;
            UserSelect = UserSelector;
        }

        public void LoginShow(Panel panelToShow)
        {
            AdminLoginForm.Hide();
            ManagerLoginForm.Hide();
            CashierLoginForm.Hide();
            UserSelect.Hide();
            panelToShow.Show();
        }
    }
}
