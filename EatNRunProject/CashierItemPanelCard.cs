using System.Windows.Forms;


namespace EatNRunProject
{
    internal class CashierItemPanelCard
    {

        private Panel CashierBurger;
        private Panel CashierSides;
        private Panel CashierSet;
        private Panel CashierDrinks;

        public CashierItemPanelCard(Panel Burger, Panel Sides, Panel Set, Panel Drinks)
        {

            CashierBurger = Burger;
            CashierSides = Sides;
            CashierSet = Set;
            CashierDrinks = Drinks;

        }

        public void CashierItemFormShow(Panel panelToShow)
        {
            CashierBurger.Hide();
            CashierSides.Hide();
            CashierSet.Hide();
            CashierDrinks.Hide();
            panelToShow.Show();
        }
    }
}
