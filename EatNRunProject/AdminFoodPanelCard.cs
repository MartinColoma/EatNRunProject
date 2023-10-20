using System.Windows.Forms;


namespace EatNRunProject
{
    internal class AdminFoodPanelCard
    {

        private Panel FoodNewItemForm;
        private Panel FoodUpdateItemForm;

        public AdminFoodPanelCard(Panel FoodNewItemFormPanel, Panel FoodUpdateItemFormPanel)
        {

            FoodNewItemForm = FoodNewItemFormPanel;
            FoodUpdateItemForm = FoodUpdateItemFormPanel;

        }

        public void AdminFoodFormShow(Panel panelToShow)
        {
            FoodNewItemForm.Hide();
            FoodUpdateItemForm.Hide();
            panelToShow.Show();
        }
    }
}
