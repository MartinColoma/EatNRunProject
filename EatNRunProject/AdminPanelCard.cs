using System.Windows.Forms;


namespace EatNRunProject
{
    internal class AdminPanelCard
    {

        private Panel FoodItemForm;
        private Panel AccountForm;
        private Panel SalesForm;

        public AdminPanelCard(Panel FoodItemFormPanel, Panel AccountItemFormPanel, Panel SalesFormPanel)
        {

            FoodItemForm = FoodItemFormPanel;
            AccountForm = AccountItemFormPanel;
            SalesForm = SalesFormPanel;

        }

        public void AdminFormShow(Panel panelToShow)
        {
            FoodItemForm.Hide();
            AccountForm.Hide();
            SalesForm.Hide();
            panelToShow.Show();
        }
    }
}
