using System.Windows.Forms;


namespace EatNRunProject
{
    internal class CashierOrderPanelCard
    {

        private Panel CashierOrder;
        private Panel CashierCheckout;
        private Panel CashierVoid;

        public CashierOrderPanelCard(Panel Order, Panel Checkout, Panel Void)
        {

            CashierOrder = Order;
            CashierCheckout = Checkout;
            CashierVoid = Void;

        }

        public void CashierOrderFormShow(Panel panelToShow)
        {
            CashierOrder.Hide();
            CashierCheckout.Hide();
            CashierVoid.Hide();
            panelToShow.Show();
        }
    }
}
