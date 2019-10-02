﻿using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using WMM.Data;
using WMM.WPF.Goals;
using WMM.WPF.Helpers;
using WMM.WPF.MVVM;
using WMM.WPF.Recurring;
using WMM.WPF.Transactions;

namespace WMM.WPF.Balances
{
    public class MonthBalanceViewModel: ObservableObject
    {
        private readonly IRepository _repository;
        private readonly IWindowService _windowService;
        private Balance _totalBalance;
        private RelayCommand _showRecurringTransactionsCommand;
        private RelayCommand<string> _showDetailTransactionsCommand;
        private bool _isExpanded;

        public DateTime Month { get; }

        public Balance TotalBalance
        {
            get => _totalBalance;
            private set => SetValue(ref _totalBalance, value);
        }

        public string Name => Month.ToString("Y");

        public ObservableCollection<AreaBalanceViewModel> AreaBalances { get; }

        public ObservableCollection<GoalMonthViewModel> Goals { get; }

        public RecurringTransactionsViewModel RecurringTransactionsViewModel { get; }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetValue(ref _isExpanded, value, SaveExpandedState);
        }

        private void SaveExpandedState()
        {
            if(IsExpanded)
                SettingsHelper.SaveExpandedMonth(Month);
            else
                SettingsHelper.SaveCollapsedMonth(Month);
        }

        public MonthBalanceViewModel(DateTime date, IRepository repository, IWindowService windowService)
        {
            _repository = repository;
            _windowService = windowService;
            Month = date.FirstDayOfMonth();
            AreaBalances = new ObservableCollection<AreaBalanceViewModel>();
            Goals = new ObservableCollection<GoalMonthViewModel>();
            RecurringTransactionsViewModel = new RecurringTransactionsViewModel(_repository, _windowService, Month);
            _isExpanded = DateTime.Now.Date.FirstDayOfMonth() == Month || SettingsHelper.IsMonthExpanded(Month);
        }

        public async Task Initialize()
        {
            await LoadAllBalances();
            await LoadGoals();
            await RecurringTransactionsViewModel.Initialize();
        }

        private async Task LoadAllBalances()
        {
            TotalBalance = await _repository.GetBalance(Month.FirstDayOfMonth(), Month.LastDayOfMonth());
            await LoadAreaBalances();
        }

        private async Task LoadAreaBalances()
        {
            AreaBalances.Clear();
            var balanceDictionary =
                await _repository.GetAreaBalances(Month.FirstDayOfMonth(), Month.LastDayOfMonth());
            foreach (var item in balanceDictionary)
            {
                var areaBalance = new AreaBalanceViewModel(item.Key, item.Value);
                AreaBalances.Add(areaBalance);
                await LoadCategoryBalances(areaBalance);
            }
        }

        private async Task LoadCategoryBalances(AreaBalanceViewModel areaBalanceViewModel)
        {
            areaBalanceViewModel.CategoryBalances.Clear();

            var balanceDictionary =
                await _repository.GetCategoryBalances(Month.FirstDayOfMonth(), Month.LastDayOfMonth(), areaBalanceViewModel.Area);

            foreach (var item in balanceDictionary.OrderBy(x => x.Key))
            {
                areaBalanceViewModel.CategoryBalances.Add(new CategoryBalanceViewModel(item.Key, item.Value));
            }
        }

        public async Task RecalculateBalances()
        {
            await LoadAllBalances();
        }

        public async Task RecalculateBalances(Category category)
        {
            TotalBalance = await _repository.GetBalance(Month.FirstDayOfMonth(), Month.LastDayOfMonth());

            var area = category.Area;
            var areaBalance = await _repository.GetBalanceForArea(Month.FirstDayOfMonth(), Month.LastDayOfMonth(), area);
            var categoryBalance = await _repository.GetBalanceForCategory(Month.FirstDayOfMonth(),
                Month.LastDayOfMonth(), category);

            var areaBalanceViewModel = AreaBalances.FirstOrDefault(x => x.Area == area);
            if (areaBalanceViewModel == null)
            {
                areaBalanceViewModel = new AreaBalanceViewModel(area, areaBalance);
                AreaBalances.Add(areaBalanceViewModel);
                await LoadCategoryBalances(areaBalanceViewModel);
            }
            else
            {
                areaBalanceViewModel.Balance = areaBalance;
                var categoryBalanceViewModel =
                    areaBalanceViewModel.CategoryBalances.FirstOrDefault(x => x.Name == category.Name);
                if (categoryBalanceViewModel == null)
                {
                    areaBalanceViewModel.CategoryBalances.Add(new CategoryBalanceViewModel(category.Name, categoryBalance));
                }
                else
                {
                    categoryBalanceViewModel.Balance = categoryBalance;
                }
            }
        }

        private async Task LoadGoals()
        {
            var goals = await _repository.GetGoals();
            foreach (var goal in goals)
            {
                var vm = new GoalMonthViewModel(goal, Month, _repository);
                await vm.Initialize();
                Goals.Add(vm);
            }
        }

        public RelayCommand ShowRecurringTransactionsCommand =>
            _showRecurringTransactionsCommand ?? (_showRecurringTransactionsCommand = new RelayCommand(ShowRecurringTransactions));
        private void ShowRecurringTransactions()
        {
            _windowService.OpenDialogWindow(RecurringTransactionsViewModel);
        }

        public RelayCommand<string> ShowDetailTransactionsCommand =>
            _showDetailTransactionsCommand ?? (_showDetailTransactionsCommand = new RelayCommand<string>(ShowDetailTransactions));
        private void ShowDetailTransactions(string category)
        {
            RaiseDetailTransactionsRequested(Month.FirstDayOfMonth(), Month.LastDayOfMonth(), category);
        }

        public event DetailTransactionsRequestEventHandler DetailTransactionsRequested;

        private void RaiseDetailTransactionsRequested(DateTime dateFrom, DateTime dateTo, string category)
        {
            DetailTransactionsRequested?.Invoke(this, new DetailTransactionsRequestEventArgs(dateFrom, dateTo, category));
        }



    }
}
