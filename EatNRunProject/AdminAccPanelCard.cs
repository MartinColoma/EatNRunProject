using System.Windows.Forms;


namespace EatNRunProject
{
    internal class AdminAccPanelCard
    {

        private Panel AccNewItemForm;
        private Panel AccUpdateItemForm;
        private Panel CreateAccItemForm;

        public AdminAccPanelCard(Panel AccNewItemFormPanel, Panel AccUpdateItemFormPanel, Panel AccFoodItemFormPanel)
        {

            AccNewItemForm = AccNewItemFormPanel;
            AccUpdateItemForm = AccUpdateItemFormPanel;
            CreateAccItemForm = AccFoodItemFormPanel;
        }

        public void AdminAccFormShow(Panel panelToShow)
        {
            AccNewItemForm.Hide();
            AccUpdateItemForm.Hide();
            CreateAccItemForm.Hide();
            panelToShow.Show();
        }
    }
}
