using System.Windows.Forms;


namespace EatNRunProject
{
    internal class AdminAccPanelCard
    {

        private Panel AccNewItemForm;
        private Panel AccUpdateItemForm;
        private Panel CreateAccItem1Form;
        private Panel CreateAccItem2Form;

        public AdminAccPanelCard(Panel AccNewItemFormPanel, Panel AccUpdateItemFormPanel, Panel AccItem1FormPanel, Panel AccItem2FormPanel)
        {

            AccNewItemForm = AccNewItemFormPanel;
            AccUpdateItemForm = AccUpdateItemFormPanel;
            CreateAccItem1Form = AccItem1FormPanel;
            CreateAccItem2Form = AccItem2FormPanel;

        }

        public void AdminAccFormShow(Panel panelToShow)
        {
            AccNewItemForm.Hide();
            AccUpdateItemForm.Hide();
            CreateAccItem1Form.Hide();
            CreateAccItem2Form.Hide();
            panelToShow.Show();
        }
    }
}
