using System;
using System.Drawing;
using System.Windows.Forms;

namespace CafeDeLunaSystem
{
    internal class TxtPlaceholder
    {
        public class PlaceholderHandler
        {
            private readonly string placeholderText;

            public readonly Color placeholderColor = Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(222)))), ((int)(((byte)(141)))));
            private readonly Color originalTextColor;

            public PlaceholderHandler(string placeholderText)
            {
                this.placeholderText = placeholderText;
            }

            // Focused
            public void Enter(object sender, EventArgs e)
            {
                TextBox textBox = sender as TextBox;
                if (textBox.Text.Equals(this.placeholderText))
                {
                    textBox.Text = string.Empty;
                }
                textBox.ForeColor = originalTextColor;
                // Disable the password character when focused
                textBox.UseSystemPasswordChar = false;
            }

            // Not focused
            public void Leave(object sender, EventArgs e)
            {
                TextBox textBox = sender as TextBox;

                if (string.IsNullOrWhiteSpace(textBox.Text))
                {
                    textBox.Text = this.placeholderText;
                    textBox.ForeColor = placeholderColor;
                    // Enable the password character when not focused and text is empty
                    textBox.UseSystemPasswordChar = true;
                }
            }
        }

        public static void SetPlaceholder(TextBox textBox, string placeholderText)
        {
            PlaceholderHandler handler = new PlaceholderHandler(placeholderText);
            textBox.Enter += handler.Enter;
            textBox.Leave += handler.Leave;
            textBox.ForeColor = handler.placeholderColor;
            textBox.Text = placeholderText;
            // Enable the password character when initializing if the text is not empty
            if (!string.IsNullOrWhiteSpace(textBox.Text) && textBox.Text != placeholderText)
            {
                textBox.UseSystemPasswordChar = true;
            }
        }
    }
}
