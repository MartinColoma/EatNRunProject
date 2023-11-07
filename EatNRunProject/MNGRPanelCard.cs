using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MngrPanelCard
    {

        private Panel MngrOrderBtn;
        private Panel MngrOrderDashboard;
        private Panel MngrSales;

        public MngrPanelCard(Panel OrderBtn, Panel Dashboard, Panel Sales)
        {

            MngrOrderBtn = OrderBtn;
            MngrOrderDashboard = Dashboard;
            MngrSales = Sales;

        }

        public void MngrFormShow(Panel panelToShow)
        {
            MngrOrderBtn.Hide();
            MngrOrderDashboard.Hide();
            MngrSales.Hide();
            panelToShow.Show();
        }
    }
}
