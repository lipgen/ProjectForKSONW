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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ModernUINavigationApp1.Pages.Settings
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class connSetting : UserControl
    {
        procFuns pf = new procFuns();
        static string[] sCol = new string[6];
        public connSetting()
        {
            InitializeComponent();
            Main();
        }

        void Main()
        {
            pf.settingParser(sCol);
            textBox1.Text = sCol[0];
            textBox2.Text = sCol[1];
            textBox3.Text = sCol[2];
            textBox4.Text = sCol[3];
            textBox5.Text = sCol[4];
        }

        private void textBox1_TextChanged(object sender, TextChangedEventArgs e) { buttonAppl.IsEnabled = true; }
        private void textBox2_TextChanged(object sender, TextChangedEventArgs e) { buttonAppl.IsEnabled = true; }
        private void textBox3_TextChanged(object sender, TextChangedEventArgs e) { buttonAppl.IsEnabled = true; }
        private void textBox4_TextChanged(object sender, TextChangedEventArgs e) { buttonAppl.IsEnabled = true; }
        private void textBox5_TextChanged(object sender, TextChangedEventArgs e) { buttonAppl.IsEnabled = true; }

        private void buttonAppl_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
