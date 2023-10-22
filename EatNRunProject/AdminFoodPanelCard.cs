using System.Windows.Forms;


namespace EatNRunProject
{
    internal class AdminFoodPanelCard
    {

        private Panel FoodNewItemForm;
        private Panel FoodUpdateItemForm;
        private Panel CreateFoodItemForm;

        public AdminFoodPanelCard(Panel FoodNewItemFormPanel, Panel FoodUpdateItemFormPanel, Panel CreateFoodItemFormPanel)
        {

            FoodNewItemForm = FoodNewItemFormPanel;
            FoodUpdateItemForm = FoodUpdateItemFormPanel;
            CreateFoodItemForm = CreateFoodItemFormPanel;
        }

        public void AdminFoodFormShow(Panel panelToShow)
        {
            FoodNewItemForm.Hide();
            FoodUpdateItemForm.Hide();
            CreateFoodItemForm.Hide();
            panelToShow.Show();
        }
    }
}
