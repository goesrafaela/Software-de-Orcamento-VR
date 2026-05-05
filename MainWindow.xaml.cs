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
            using (var db = new AppDbContext())
            {
                var usuario = db.Usuarios.FirstOrDefault(u =>
                    u.Email == EmailBox.Text &&
                    u.Senha == SenhaBox.Password);

                if (usuario != null)
                {
                    MessageBox.Show("Login realizado!");
                }
                else
                {
                    MessageBox.Show("Usuário ou senha inválidos!");
                }
            }
        }
        private void AbrirCadastro(object sender, MouseButtonEventArgs e)
        {
            var tela = new RegisterWindow();
            tela.ShowDialog();
        }
    }
}