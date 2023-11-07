using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MngrOrderPanelCard
    {

        private Panel MngrOrder;
        private Panel MngrCheckout;

        public MngrOrderPanelCard(Panel Order, Panel Checkout)
        {

            MngrOrder = Order;
            MngrCheckout = Checkout;

        }

        public void MngrOrderFormShow(Panel panelToShow)
        {
            MngrOrder.Hide();
            MngrCheckout.Hide();
            panelToShow.Show();
        }
    }
}
