using System.Windows.Forms;


namespace EatNRunProject
{
    internal class LoginPanelCard
    {

        private Panel LoginForm;
        private Panel UserSelect;

        public LoginPanelCard(Panel LoginFormPanel, Panel UserSelector)
        {

            LoginForm = LoginFormPanel;
            UserSelect = UserSelector;
        }

        public void LoginShow(Panel panelToShow)
        {
            LoginForm.Hide();
            UserSelect.Hide();
            panelToShow.Show();
        }
    }
}
