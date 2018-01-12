using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace CodeBlock.Context
{
    public class RenderLayer
    {
        public static string[] colorNames = { "#CCFFFF", "#CCFFEB", "#CCFFD6", "#E0FFCC", "#FFFFCC", "#FFF5CC", "#FFEBCC", "#FFCCCC", "#FFCCE0", "#FFCCF5", "#EBCCFF", "#D6CCFF", "#E6E6E6" };
        public static Color[] colorScheme = colorNames.Select(s => (Color) ColorConverter.ConvertFromString(s)).ToArray();

        public Thickness Margin { get; private set; }
        public Brush Color { get; private set; }
        public string Text { get; private set; }
        public RenderLayer(int depth, Color inputColor, string inputText)
        {
            Margin = new Thickness(depth * 15, 2, 5, 2);
            Color = new SolidColorBrush(inputColor);
            Text = inputText;
        }
        public RenderLayer(int depth, string inputText)
        {
            Margin = new Thickness(depth * 15, 2, 5, 2);
            Color = new SolidColorBrush(colorScheme[depth % colorScheme.Length]);
            Text = inputText;
        }
    }
}
