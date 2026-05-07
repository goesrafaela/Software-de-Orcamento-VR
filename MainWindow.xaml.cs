using System.Linq;
using System.Windows;
using System.Windows.Input;
using OrcaPro.Data;
using OrcaPro.Services;

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

            // 🔥 validação
            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(senha))
            {
                MostrarErro("Preencha todos os campos.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.Email == email &&
                    u.Senha == senha);

                // 🔥 LOGIN OK
                if (usuario != null)
                {
                    // 🔥 salva usuário logado
                    UsuarioSessao.NomeUsuario = usuario.Nome;

                    // 🔥 abre dashboard
                    var dashboard = new DashboardWindow();

                    // 🔥 define nova janela principal
                    Application.Current.MainWindow = dashboard;

                    dashboard.Show();

                    // 🔥 fecha login sem fechar app
                    this.Close();
                }
                else
                {
                    MostrarErro("Usuário ou senha inválidos.");
                }
            }
        }

        // 🔥 MOSTRAR ERRO
        private void MostrarErro(string mensagem)
        {
            ErroText.Text = mensagem;
            ErroText.Visibility = Visibility.Visible;
        }

        // 🔥 ABRIR CADASTRO
        private void AbrirCadastro(object sender, MouseButtonEventArgs e)
        {
            var tela = new RegisterWindow();

            tela.ShowDialog();
        }
    }
}