using ChatAppWpf.Pages;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatAppWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow Instance { get; private set; }
        public Frame MainFrame {
            get
            {
                return mainFrame;
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            Instance = this;

            MainFrame.Navigate(new LoginPage());
        }
    }
}