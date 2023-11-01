using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MngrOrderPanelCard
    {

        private Panel MngrOrder;
        private Panel MngrCheckout;
        private Panel MngrVoid;

        public MngrOrderPanelCard(Panel Order, Panel Checkout, Panel Void)
        {

            MngrOrder = Order;
            MngrCheckout = Checkout;
            MngrVoid = Void;

        }

        public void MngrOrderFormShow(Panel panelToShow)
        {
            MngrOrder.Hide();
            MngrCheckout.Hide();
            MngrVoid.Hide();
            panelToShow.Show();
        }
    }
}
