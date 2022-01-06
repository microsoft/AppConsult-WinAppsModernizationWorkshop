using ContosoExpenses.Data.Models;
using ContosoExpenses.Data.Services;
using ContosoExpenses.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;

namespace ContosoExpenses.ViewModels
{
    public class AddNewExpenseViewModel : ObservableObject
    {
        private readonly IDatabaseService databaseService;
        private readonly IStorageService storageService;

        private string _address;
        public string Address
        {
            get { return _address; }
            set
            {
                SetProperty(ref _address, value);
                SaveExpenseCommand.NotifyCanExecuteChanged();
            }
        }

        private string _city;
        public string City
        {
            get { return _city; }
            set
            {
                SetProperty(ref _city, value);
                SaveExpenseCommand.NotifyCanExecuteChanged();
            }
        }

        private double _cost;
        public double Cost
        {
            get { return _cost; }
            set
            {
                SetProperty(ref _cost, value);
                SaveExpenseCommand.NotifyCanExecuteChanged();
            }
        }

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                SetProperty(ref _description, value);
                SaveExpenseCommand.NotifyCanExecuteChanged();
            }
        }

        private string _expenseType;
        public string ExpenseType
        {
            get { return _expenseType; }
            set
            {
                SetProperty(ref _expenseType, value);
                SaveExpenseCommand.NotifyCanExecuteChanged();
            }
        }

        private DateTimeOffset _date;
        public DateTimeOffset Date
        {
            get { return _date; }
            set { SetProperty(ref _date, value); }
        }


        public AddNewExpenseViewModel(IDatabaseService databaseService, IStorageService storageService)
        {
            this.databaseService = databaseService;
            this.storageService = storageService;

            Date = DateTime.Today;
        }

        private bool IsFormFilled
        {
            get
            {
                return !string.IsNullOrEmpty(Address) && !string.IsNullOrEmpty(City) && !string.IsNullOrEmpty(Description) && !string.IsNullOrEmpty(ExpenseType) && Cost != 0;
            }
        }

        private IRelayCommand _saveExpenseCommand;
        public IRelayCommand SaveExpenseCommand
        {
            get
            {
                if (_saveExpenseCommand == null)
                {
                    _saveExpenseCommand = new RelayCommand(() =>
                    {
                        Expense expense = new Expense
                        {
                            Address = Address,
                            City = City,
                            Cost = Cost,
                            Date = Date.DateTime,
                            Description = Description,
                            EmployeeId = storageService.SelectedEmployeeId,
                            Type = ExpenseType
                        };

                        databaseService.SaveExpense(expense);
                        WeakReferenceMessenger.Default.Send(new UpdateExpensesListMessage());
                        WeakReferenceMessenger.Default.Send(new CloseWindowMessage());
                    }, () => IsFormFilled
                    );
                }

                return _saveExpenseCommand;
            }
        }
    }
}
