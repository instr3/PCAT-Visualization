using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CodeBlock
{
    /// <summary>
    /// Detail.xaml 的交互逻辑
    /// </summary>
    public partial class Detail : Window
    {
        List<StackPanel> smallstackpanels = new List<StackPanel>();
        List<List<TextBlock>> textblocks = new List<List<TextBlock>>();
        public Detail(List<List<string>> input)
        {
            InitializeComponent();
            foreach (var line in input)
            {
                StackPanel sp = new StackPanel();
                sp.Orientation = Orientation.Horizontal;
                List<TextBlock> ltb = new List<TextBlock>();
                foreach (var text in line)
                {
                    TextBlock tb = new TextBlock();
                    tb.Text = text;
                    tb.FontFamily = new FontFamily("Courier New");
                    /*
                    if (Regex.Match(text, @"\s").Success)
                        tb.Background = Brushes.Green;
                    else tb.Background = Brushes.Orange;
                    */
                    ltb.Add(tb);
                    sp.Children.Add(tb);
                }
                //sp.Background = Brushes.Azure;
                smallstackpanels.Add(sp);
                textblocks.Add(ltb);
                BigStackPanel.Children.Add(sp);
                sp.Height = 24;
                sp.HorizontalAlignment = HorizontalAlignment.Left;
            }
            colorlist = new List<Brush>();
            colorlist.Add(Brushes.Green);
            colorlist.Add(Brushes.Orange);
            colorlist.Add(Brushes.Pink);
            RandBlock(ref BigStackPanel, 0, smallstackpanels.Count, 0);
        }

        List<Brush> colorlist;
        void RandBlock(ref StackPanel fasp, int start, int end, int color)
        {
            //MessageBox.Show(start + " " + end + " " + color);
            fasp.Background = colorlist[color];
            for (int i = start; i < end; i++)
                smallstackpanels[i].Background = colorlist[color];
            color++;
            color %= colorlist.Count;
            fasp.Children.Clear();
            Random rnd = new Random();
            /*
            if (start == 0 && end == smallstackpanels.Count)
                for (int i = 0; i < smallstackpanels.Count; i++)
                    smallstackpanels[i].Background = Brushes.White;
            */
            if (start + 1 == end)
            {
                fasp.Children.Add(smallstackpanels[start]);
                smallstackpanels[start].Background = colorlist[color++];
                color %= colorlist.Count;
                (smallstackpanels[start].Children[rnd.Next() % smallstackpanels[start].Children.Count] as TextBlock).Background = colorlist[color];
                return;
            }
            int left, right;
            for (;;)
            {
                left = rnd.Next() % (end - start + 1) + start;
                right = rnd.Next() % (end - start + 1) + start;
                if (left < right && left != right) break;
            }
            for (int i = start; i < left; i++)
                fasp.Children.Add(smallstackpanels[i]);
            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Vertical;
            sp.HorizontalAlignment = HorizontalAlignment.Left;
            RandBlock(ref sp, left, right, color);
            fasp.Children.Add(sp);
            for (int i = right; i < end; i++)
                fasp.Children.Add(smallstackpanels[i]);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            /*
            Random r = new Random();
            foreach (var i in smallstackpanels)
                i.Background = l[r.Next() % 3];
            foreach (var line in textblocks)
                foreach (var i in line)
                    i.Background = l[r.Next() % 3];
            */
        }
    }
}
