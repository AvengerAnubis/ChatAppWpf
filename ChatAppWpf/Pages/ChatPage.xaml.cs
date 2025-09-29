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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ChatAppWpf.Pages
{
    /// <summary>
    /// Логика взаимодействия для ChatPage.xaml
    /// </summary>
    public partial class ChatPage : Page
    {
        /// <summary>
        /// Текст чата
        /// </summary>
        public ChatPage()
        {
            InitializeComponent();
            ChatClient.Instance.MessageReceived += OnMessageReceived;
        }

        /// <summary>
        /// Метод, вызываемый при получении сообщения
        /// </summary>
        /// <param name="msg">Сообщение, полученное от собеседника</param>
        private void OnMessageReceived(string msg) =>
            ApplyMessage(msg);

        /// <summary>
        /// Сохранить сообщение в буфер чата, вывести на экран
        /// </summary>
        /// <param name="msg">Сообщение</param>
        private void ApplyMessage(string msg)
        {
            chatStackPanel.Children.Add(new TextBlock() { Text = msg, TextWrapping = TextWrapping.Wrap });
        }
        /// <summary>
        /// Метод, вызываемый при клике на кнопку "Отправить" (>)
        /// Отправляет сообщение собеседнику, также сохраняет сообщение в буфер
        /// </summary>
        private void SendButtonClick(object sender, RoutedEventArgs e)
        {
            ChatClient.Instance.SendMessage(messageTextBox.Text);
            ApplyMessage(messageTextBox.Text);
        }
    }
}
