using System.Windows.Forms;


namespace EatNRunProject
{
    internal class MngrItemPanelCard
    {

        private Panel MngrBurger;
        private Panel MngrSides;
        private Panel MngrSet;
        private Panel MngrDrinks;

        public MngrItemPanelCard(Panel Burger, Panel Sides, Panel Set, Panel Drinks)
        {

            MngrBurger = Burger;
            MngrSides = Sides;
            MngrSet = Set;
            MngrDrinks = Drinks;

        }

        public void MngrItemFormShow(Panel panelToShow)
        {
            MngrBurger.Hide();
            MngrSides.Hide();
            MngrSet.Hide();
            MngrDrinks.Hide();
            panelToShow.Show();
        }
    }
}
