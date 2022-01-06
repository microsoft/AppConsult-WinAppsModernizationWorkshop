using ContosoExpenses.Data.Services;
using Microsoft.Toolkit.Mvvm.ComponentModel;

namespace ContosoExpenses.ViewModels
{
    public class ExpensesDetailViewModel: ObservableObject
    {
        private string _expenseType;
        public string ExpenseType
        {
            get { return _expenseType; }
            set { SetProperty(ref _expenseType, value); }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }

        private string _location;
        public string Location
        {
            get { return _location; }
            set { SetProperty(ref _location, value); }
        }

        private double _amount;
        public double Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        public ExpensesDetailViewModel(IDatabaseService databaseService, IStorageService storageService)
        {
            var expense = databaseService.GetExpense(storageService.SelectedExpense);

            ExpenseType = expense.Type;
            Description = expense.Description;
            Location = expense.Address;
            Amount = expense.Cost;
        }
    }
}
