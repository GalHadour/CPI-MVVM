using System.Windows.Media;


namespace CPI.ViewModels
{
    public static class ColorsPalette
    {

        public static SolidColorBrush Red { get; } = new BrushConverter().ConvertFromString("#CCE53935") as SolidColorBrush;
        public static SolidColorBrush Green { get; } = new BrushConverter().ConvertFromString("#CC43A047") as SolidColorBrush;
        public static SolidColorBrush Gray { get; } = new BrushConverter().ConvertFromString("#CC888888") as SolidColorBrush;
        public static SolidColorBrush Blue { get; } = new BrushConverter().ConvertFromString("#CC1E88E5") as SolidColorBrush;
        public static SolidColorBrush Orange { get; } = new BrushConverter().ConvertFromString("#CCFF9800") as SolidColorBrush;
        public static SolidColorBrush Yellow { get; } = new BrushConverter().ConvertFromString("#CCFFB74D") as SolidColorBrush;


    }
}
