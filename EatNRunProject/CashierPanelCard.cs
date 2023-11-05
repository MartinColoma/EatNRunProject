using System.Windows.Forms;


namespace EatNRunProject
{
    internal class CashierPanelCard
    {

        private Panel CashierOrderBtn;
        private Panel CashierMngrOrderDashboard;

        public CashierPanelCard(Panel OrderBtn, Panel Dashboard)
        {

            CashierOrderBtn = OrderBtn;
            CashierMngrOrderDashboard = Dashboard;


        }

        public void CashierFormShow(Panel panelToShow)
        {
            CashierOrderBtn.Hide();
            CashierMngrOrderDashboard.Hide();

            panelToShow.Show();
        }
    }
}
