﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using WMM.Data;
using WMM.WPF.Helpers;
using WMM.WPF.MVVM;
using WMM.WPF.Resources;

namespace WMM.WPF.Transactions
{
    public class TransactionListViewModelBase : ObservableObject
    {
        private ObservableCollection<Transaction> _transactions;
        private AsyncRelayCommand<Transaction> _deleteTransactionCommand;
        private RelayCommand<Transaction> _editTransactionCommand;
        protected readonly IRepository Repository;
        protected readonly IWindowService WindowService;
        private RelayCommand<Transaction> _useAsTemplateCommand;

        public TransactionListViewModelBase(IRepository repository, IWindowService windowService, bool showDate)
        {
            Repository = repository;
            WindowService = windowService;
            ShowDate = showDate;
            Transactions = new ObservableCollection<Transaction>();
        }

        public bool ShowDate { get; }

        public ObservableCollection<Transaction> Transactions
        {
            get => _transactions;
            set => SetValue(ref _transactions, value);
        }

        public AsyncRelayCommand<Transaction> DeleteTransactionCommand => _deleteTransactionCommand ?? (_deleteTransactionCommand = new AsyncRelayCommand<Transaction>(DeleteTransaction));
        private async Task DeleteTransaction(Transaction transaction)
        {
            if(!WindowService.AskConfirmation(Captions.ConfirmDeleteTransaction))
                return;

            await Repository.DeleteTransaction(transaction);
            Transactions.Remove(transaction);
        }

        public RelayCommand<Transaction> EditTransactionCommand =>
            _editTransactionCommand ?? (_editTransactionCommand = new RelayCommand<Transaction>(EditTransaction));

        private void EditTransaction(Transaction transaction)
        {
            var editTransactionViewModel = new EditTransactionViewModel(transaction, Repository, ShowDate);
            editTransactionViewModel.TransactionChanged += (sender, args) =>
            {
                var index = Transactions.IndexOf(args.OldTransaction);
                Transactions.Remove(args.OldTransaction);
                Transactions.Insert(index, args.NewTransaction);
            };
            WindowService.OpenDialogWindow(editTransactionViewModel);
        }

        public RelayCommand<Transaction> UseAsTemplateCommand =>
            _useAsTemplateCommand ?? (_useAsTemplateCommand = new RelayCommand<Transaction>(UseTransactionAsTemplate));

        private void UseTransactionAsTemplate(Transaction transaction)
        {
            RaiseUseAsTemplateRequested(transaction);
        }
        
        public event TransactionEventHandler UseAsTemplateRequested;

        private void RaiseUseAsTemplateRequested(Transaction transaction)
        {
            UseAsTemplateRequested?.Invoke(this, new TransactionEventArgs(transaction));
        }
    }
}
