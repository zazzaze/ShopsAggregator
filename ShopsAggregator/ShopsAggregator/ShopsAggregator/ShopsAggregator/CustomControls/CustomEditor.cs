using System;
using Xamarin.Forms;
namespace ShopsAggregator.CustomControls
{
    public class CustomEditor : Entry
    {
        private Color borderColor;
        public Color BorderColor
        {
            get => borderColor;
            set
            {
                borderColor = value;
            }
        }

        public CustomEditor() : base()
        {
        }
    }
}