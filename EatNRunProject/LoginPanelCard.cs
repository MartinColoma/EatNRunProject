using System.Windows.Forms;


namespace EatNRunProject
{
    internal class LoginPanelCard
    {

        private Panel EmployeeLoginForm;


        private Panel UserSelect;

        public LoginPanelCard(Panel EmployeeLoginFormPanel, Panel UserSelector)
        {

            EmployeeLoginForm = EmployeeLoginFormPanel;
            UserSelect = UserSelector;
        }

        public void LoginFormShow(Panel panelToShow)
        {
            EmployeeLoginForm.Hide();
            UserSelect.Hide();
            panelToShow.Show();
        }
    }
}
