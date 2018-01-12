using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// InputTextBox.xaml 的交互逻辑
    /// </summary>
    public partial class InputTextBox : Window
    {
        public string Result { get; private set; }

        public InputTextBox()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Result = Input.Text;
            Close();
        }
    }
}
