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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CodeBlock
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<List<string>> NaiveTokenMaker(string input)
        {
            List<List<string>> res = new List<List<string>>();
            foreach (var line in Regex.Split(input, "\r\n"))
            {
                List<string> l = new List<string>();
                foreach (Match m in Regex.Matches(line, @"\s+|\S+"))
                    l.Add(m.Value);
                res.Add(l);
            }
            return res;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Detail d = new Detail(NaiveTokenMaker(InputTextBox.Text));
            d.ShowDialog();
        }
    }
}
