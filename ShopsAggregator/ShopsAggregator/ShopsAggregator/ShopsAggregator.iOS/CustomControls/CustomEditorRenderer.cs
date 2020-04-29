using System.ComponentModel;
using CoreGraphics;
using Foundation;
using ShopsAggregator.CustomControls;
using ShopsAggregator.iOS.CustomControls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomEditor), typeof(CustomEditorRenderer))]
namespace ShopsAggregator.iOS.CustomControls
{
    public class CustomEditorRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if (e.NewElement != null)
            {
                var view = (CustomEditor)Element;
                Control.Layer.BorderColor = view.BorderColor.ToCGColor();
                Control.Layer.BorderWidth = 2;
                Control.Layer.CornerRadius = 5;
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);
            var view = (CustomEditor)Element; 
            Control.Layer.BorderColor = view.BorderColor.ToCGColor();
        }
    }
}