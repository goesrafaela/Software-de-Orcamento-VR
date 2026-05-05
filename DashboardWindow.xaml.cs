using System.Windows;

namespace OrcaPro
{
    public partial class DashboardWindow : Window
    {
        public DashboardWindow()
        {
            InitializeComponent();
        }

        private void Sair_Click(object sender, RoutedEventArgs e)
        {
            var login = new MainWindow();
            login.Show();
            this.Close();
        }

        private void AbrirClientes_Click(object sender, RoutedEventArgs e)
        {
            var tela = new ClientesWindow();
            tela.ShowDialog();
        }
    }
}