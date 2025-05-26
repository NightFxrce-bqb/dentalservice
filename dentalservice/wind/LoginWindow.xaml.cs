using dentalservice.Wind;
using dentalservice.Data;
using dentalservice.Models;
using dentalservice;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace dentalservice.Wind
{
    public partial class LoginWindow : Window
    {
        private readonly DentalContext _dbContext;
        public User CurrentUser { get; private set; }

        public LoginWindow()
        {
            InitializeComponent();
            var optionsBuilder = new DbContextOptionsBuilder<DentalContext>();
            optionsBuilder.UseSqlite("Data Source=dental.db");
            _dbContext = new DentalContext(optionsBuilder.Options);
        }

        public DentalContext GetDbContext()
        {
            return _dbContext;
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var login = LoginTextBox.Text.Trim();
                var password = PasswordBox.Password;

                Console.WriteLine($"Попытка входа: Login={login}, Password={password}");

                if (string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password))
                {
                    Console.WriteLine("Ошибка: Логин или пароль пустые.");
                    MessageBox.Show("Введите логин и пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var user = _dbContext.Users
                    .FirstOrDefault(u => u.Login == login && u.Password == password);

                if (user == null)
                {
                    Console.WriteLine("Ошибка: Пользователь не найден.");
                    MessageBox.Show("Неверный логин или пароль.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Console.WriteLine($"Успешная авторизация: User={user.FullName}, Id={user.Id}");
                CurrentUser = user;
                // Принудительное открытие MainWindow
                var mainWindow = new MainWindow(_dbContext, CurrentUser);
                mainWindow.Show();
                Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение: {ex.Message}");
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var registerWindow = new RegisterWindow(_dbContext);
            if (registerWindow.ShowDialog() == true)
            {
                MessageBox.Show("Регистрация успешна! Теперь вы можете войти.", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}