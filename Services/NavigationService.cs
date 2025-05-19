using System;
using System.Windows.Forms;
using FoodOrderApp.Helpers;
using FoodOrderApp.Forms;

namespace FoodOrderApp.Services
{
    public class NavigationService
    {
        private Form _currentForm;     // Текущая открытая форма
        private Form _previousForm;    // Форма, которая была открыта до текущей

        // Конструктор принимает стартовую форму (например, main или admin)
        public NavigationService(Form startForm)
        {
            _currentForm = startForm;
        }

        public void NavigateToMenu()
        {
            OpenForm(new MenuForm());
        }

        public void NavigateToOrders()
        {
            if (Session.CurrentUser.Role == Models.UserRole.admin)
                OpenForm(new AdminOrdersForm());
            else
                OpenForm(new UserOrdersForm());
        }

        public void NavigateToPayment()
        {
            OpenForm(new PaymentForm());
        }

        public void NavigateToStatistics()
        {
            if (Session.CurrentUser.Role == Models.UserRole.admin)
                OpenForm(new StatisticsForm());
            else
                MessageBox.Show("Доступ запрещён", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        // Общий метод открытия формы
        private void OpenForm(Form form)
        {
            // Сохраняем текущую форму в previous, т.к. она будет скрыта
            _previousForm = _currentForm;

            // Обновляем currentForm на новую форму
            _currentForm = form;

            // Скрываем предыдущую форму
            _previousForm.Hide();

            // Показываем новую форму
            _currentForm.Show();

            // Подписываемся на событие закрытия новой формы
            _currentForm.FormClosed += (s, e) =>
            {
                // Когда новая форма закрылась, показываем предыдущую,
                // если она существует и не была удалена
                if (_previousForm != null && !_previousForm.IsDisposed)
                {
                    _previousForm.Show();

                    // Восстанавливаем currentForm на предыдущую
                    _currentForm = _previousForm;
                }
            };
        }
    }
}
