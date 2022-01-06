using ContosoExpenses.Data.Services;
using ContosoExpenses.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ContosoExpenses
{
    public class ViewModelLocator
    {
        private IServiceProvider _container;

        public ViewModelLocator()
        {
            var services = new ServiceCollection();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<ExpensesListViewModel>();
            services.AddTransient<ExpensesDetailViewModel>();
            services.AddTransient<AddNewExpenseViewModel>();
            services.AddTransient<IDatabaseService, DatabaseService>();

            services.AddSingleton<IStorageService, StorageService>();
            _container = services.BuildServiceProvider();
        }

        public MainWindowViewModel MainWindowViewModel => _container.GetRequiredService<MainWindowViewModel>();

        public ExpensesListViewModel ExpensesListViewModel => _container.GetRequiredService<ExpensesListViewModel>();

        public ExpensesDetailViewModel ExpensesDetailViewModel => _container.GetRequiredService<ExpensesDetailViewModel>();

        public AddNewExpenseViewModel AddNewExpenseViewModel => _container.GetRequiredService<AddNewExpenseViewModel>();
    }
}
