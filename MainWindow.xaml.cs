using System.Windows;
using System.Linq;        // 👈 AQUI
using OrcaPro.Data;
using System.Windows.Input;

namespace OrcaPro
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            

            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }
        }

       private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailBox.Text.Trim();
            string senha = SenhaBox.Password;

            ErroText.Visibility = Visibility.Collapsed;

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
            {
                MostrarErro("Preencha todos os campos.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.Email == email &&
                    u.Senha == senha);

                if (usuario != null)
                {
                    var dashboard = new DashboardWindow();
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    MostrarErro("Usuário ou senha inválidos.");
                }
            }
}

private void MostrarErro(string mensagem)
{
    ErroText.Text = mensagem;
    ErroText.Visibility = Visibility.Visible;
}
        private void AbrirCadastro(object sender, MouseButtonEventArgs e)
        {
            var tela = new RegisterWindow();
            tela.ShowDialog();
        }
    }
}