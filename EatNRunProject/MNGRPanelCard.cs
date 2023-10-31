using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MNGRPanelCard
    {

        private Panel MngrOrderBtn;
        private Panel MngrOrderDashboard;

        public MNGRPanelCard(Panel OrderBtn, Panel Dashboard)
        {

            MngrOrderBtn = OrderBtn;
            MngrOrderDashboard = Dashboard;

        }

        public void AdminFormShow(Panel panelToShow)
        {
            MngrOrderBtn.Hide();
            MngrOrderDashboard.Hide();
            panelToShow.Show();
        }
    }
}
