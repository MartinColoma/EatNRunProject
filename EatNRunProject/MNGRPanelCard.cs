using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MngrPanelCard
    {

        private Panel MngrOrderBtn;
        private Panel MngrOrderDashboard;

        public MngrPanelCard(Panel OrderBtn, Panel Dashboard)
        {

            MngrOrderBtn = OrderBtn;
            MngrOrderDashboard = Dashboard;

        }

        public void MngrFormShow(Panel panelToShow)
        {
            MngrOrderBtn.Hide();
            MngrOrderDashboard.Hide();
            panelToShow.Show();
        }
    }
}
