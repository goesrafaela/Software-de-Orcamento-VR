using System.Windows;
using OrcaPro.Data;
using OrcaPro.Models;
using System.Windows.Input;
using System.Linq; //

namespace OrcaPro
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Cadastrar_Click(object sender, RoutedEventArgs e)
        {
            string nome = NomeBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string senha = SenhaBox.Password;

            // Reset erro
            ErroText.Visibility = Visibility.Collapsed;

            // Validação
            if (string.IsNullOrEmpty(nome) ||
                string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(senha))
            {
                MostrarErro("Preencha todos os campos.");
                return;
            }

            if (!email.Contains("@"))
            {
                MostrarErro("E-mail inválido.");
                return;
            }

            using (var db = new AppDbContext())
            {
                var existe = db.Usuarios.Any(u => u.Email == email);

                if (existe)
                {
                    MostrarErro("Este e-mail já está cadastrado.");
                    return;
                }

                var usuario = new Usuario
                {
                    Nome = nome,
                    Email = email,
                    Senha = senha
                };

                db.Usuarios.Add(usuario);
                db.SaveChanges();
            }

            MessageBox.Show("Cadastro realizado com sucesso!");

            this.Close();
        }

        private void MostrarErro(string mensagem)
        {
            ErroText.Text = mensagem;
            ErroText.Visibility = Visibility.Visible;
        }

        // 👇 AGORA DENTRO DA CLASSE (correto)
        private void VoltarLogin_Click(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }
    }
}