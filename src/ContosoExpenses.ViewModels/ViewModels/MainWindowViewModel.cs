using ContosoExpenses.Data.Models;
using ContosoExpenses.Data.Services;
using ContosoExpenses.Messages;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;

namespace ContosoExpenses.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        private List<Employee> _employees;
        public List<Employee> Employees
        {
            get { return _employees; }
            set { SetProperty(ref _employees, value); }
        }

        private Employee _selectedEmployee;
        private readonly IStorageService _storageService;

        public Employee SelectedEmployee
        {
            get { return _selectedEmployee; }
            set
            {
                if (value != null)
                {
                    _storageService.SelectedEmployeeId = value.EmployeeId;
                    WeakReferenceMessenger.Default.Send(new SelectedEmployeeMessage());
                    SetProperty(ref _selectedEmployee, value);
                }
            }
        }

        public MainWindowViewModel(IDatabaseService databaseService, IStorageService storageService)
        {
            databaseService.InitializeDatabase();
            Employees = databaseService.GetEmployees();
            this._storageService = storageService;
        }
    }
}
