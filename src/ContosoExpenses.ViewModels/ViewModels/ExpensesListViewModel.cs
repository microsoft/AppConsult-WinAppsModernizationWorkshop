using ContosoExpenses.Data.Models;
using ContosoExpenses.Data.Services;
using ContosoExpenses.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using System.Collections.Generic;
using System.Windows.Input;

namespace ContosoExpenses.ViewModels
{
    public class ExpensesListViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly IStorageService _storageService;

        private Employee _selectedEmployee;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set { SetProperty(ref _selectedEmployee, value); }
        }

        private string _fullName;
        public string FullName
        {
            get { return _fullName; }
            set { SetProperty(ref _fullName, value); }
        }

        private List<Expense> _expenses;
        public List<Expense> Expenses
        {
            get { return _expenses; }
            set { SetProperty(ref _expenses, value); }
        }

        private Expense _selectedExpense;

        public Expense SelectedExpense
        {
            get { return _selectedExpense; }
            set
            {
                if (value != null)
                {
                    _storageService.SelectedExpense = value.ExpenseId;
                    WeakReferenceMessenger.Default.Send(new SelectedExpenseMessage());
                    SetProperty(ref _selectedExpense, value);
                }
            }
        }

        private ICommand _addNewExpenseCommand;


        public ICommand AddNewExpenseCommand
        {
            get
            {
                if (_addNewExpenseCommand == null)
                {
                    _addNewExpenseCommand = new RelayCommand(() =>
                    {
                        WeakReferenceMessenger.Default.Send(new AddNewExpenseMessage());
                    });
                }

                return _addNewExpenseCommand;
            }
        }

        public ExpensesListViewModel(IDatabaseService databaseService, IStorageService storageService)
        {
            SelectedEmployee = databaseService.GetEmployee(storageService.SelectedEmployeeId);
            Expenses = databaseService.GetExpenses(storageService.SelectedEmployeeId);

            FullName = $"{SelectedEmployee.FirstName} {SelectedEmployee.LastName}";

            this._databaseService = databaseService;
            this._storageService = storageService;

            WeakReferenceMessenger.Default.Register<UpdateExpensesListMessage>(this, (_, message) =>
            {
                Expenses = this._databaseService.GetExpenses(this._storageService.SelectedEmployeeId);
            });
        }
    }
}
