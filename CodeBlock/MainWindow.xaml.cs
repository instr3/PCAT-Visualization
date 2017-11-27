using System;
using System.Collections.Generic;
using System.IO;
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
            if (File.Exists("test17-space.pcat"))
                InputTextBox.Text = File.ReadAllText("test17-space.pcat");
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Detail d = new Detail(InputTextBox.Text.Replace("\t", "    ").Replace("\r",""));
            d.ShowDialog();
        }

        private void InputTextBox_Drop(object sender, DragEventArgs e)
        {
            var fileNames = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (fileNames == null) return;
            var fileName = fileNames.FirstOrDefault();
            if (fileName == null) return;
            try
            {
                string result = "";
                using (StreamReader sr = new StreamReader(fileName))
                {
                    result = sr.ReadToEnd();
                }
                InputTextBox.Text = result;
            }
            catch
            {
                MessageBox.Show("Illegal file dropped");
            }
        }

        private void InputTextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}
