using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EatNRunProject
{
    public class CardLayoutPanel : Panel
    {
        private List<Panel> cardPanels = new List<Panel>();
        private Dictionary<string, Panel> cardPanelMap = new Dictionary<string, Panel>(); // Added dictionary for mapping panel names
        private int currentCardIndex = 0;

        public void AddCard(string panelName, Panel panel) // Updated AddCard method to include panel name
        {
            cardPanels.Add(panel);
            panel.Visible = false;
            this.Controls.Add(panel);

            cardPanelMap[panelName] = panel; // Store the panel in the dictionary with its name
        }

        public void ShowCard(int index)
        {
            if (index >= 0 && index < cardPanels.Count)
            {
                cardPanels[currentCardIndex].Visible = false;
                currentCardIndex = index;
                cardPanels[currentCardIndex].Visible = true;
            }
        }

        public void ShowCardByName(string panelName) // Added method to show a panel by name
        {
            if (cardPanelMap.ContainsKey(panelName))
            {
                foreach (Panel panel in cardPanels)
                {
                    panel.Visible = false;
                }
                cardPanelMap[panelName].Visible = true;
            }
        }

        public void ShowNextCard()
        {
            ShowCard((currentCardIndex + 1) % cardPanels.Count);
        }

        public void ShowPreviousCard()
        {
            int newIndex = currentCardIndex - 1;
            if (newIndex < 0)
                newIndex = cardPanels.Count - 1;
            ShowCard(newIndex);
        }
    }
}
