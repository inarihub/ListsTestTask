using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Collections.Specialized;

namespace ListsTestTask.ViewModels
{
    class ListsDialogViewModel
    {
        private ObservableCollection<OptionField> _availableOptions;
        private ObservableCollection<OptionField> _selectedOptions;
        public ObservableCollection<OptionField> AvailableOptions 
        {
            get => _availableOptions;
            set { _availableOptions= value; } 
        }
        public ObservableCollection<OptionField> SelectedOptions 
        {
            get => _selectedOptions;
            set { _selectedOptions = value; }
        }
        public IRelayCommand SelectCommand { get; set; }
        public IRelayCommand SelectAllCommand { get; set; }
        public IRelayCommand UnselectCommand { get; set; }
        public IRelayCommand UnselectAllCommand { get; set; }
        public IRelayCommand MoveSelectedUpCommand { get; set; }
        public IRelayCommand MoveSelectedDownCommand { get; set; }
        public IRelayCommand SubmitSelectedCommand { get; set; }
        public ListsDialogViewModel(List<OptionField> availableOptions, ObservableCollection<OptionField> selectedOptions)
        {
            _availableOptions = new(availableOptions.Except(selectedOptions));
            _selectedOptions = new(selectedOptions);
            AvailableOptions.CollectionChanged += OnAvailableCollectionChanged;
            SelectedOptions.CollectionChanged += OnSelectedCollectionChanged;
            SelectCommand = new RelayCommand(SelectOption, CanExecuteOnAvailable);
            SelectAllCommand = new RelayCommand(SelectAllOptions, CanExecuteOnAvailable);
            UnselectCommand = new RelayCommand(UnselectOption, CanExecuteOnSelected);
            UnselectAllCommand = new RelayCommand(UnselectAllOptions, CanExecuteOnSelected);
            MoveSelectedUpCommand = new RelayCommand(MoveOptionUp, CanExecuteOnSelected);
            MoveSelectedDownCommand = new RelayCommand(MoveOptionDown, CanExecuteOnSelected);
            SubmitSelectedCommand = new RelayCommand<Window>(SubmitSelected);
        }

        private void OnSelectedCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
                UnselectCommand.NotifyCanExecuteChanged();
                UnselectAllCommand.NotifyCanExecuteChanged();
                MoveSelectedDownCommand.NotifyCanExecuteChanged();
                MoveSelectedUpCommand.NotifyCanExecuteChanged();
        }
        private void OnAvailableCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
                SelectCommand.NotifyCanExecuteChanged();
                SelectAllCommand.NotifyCanExecuteChanged();
        }
        private void SelectOption()
        {
            TransferOption(AvailableOptions, SelectedOptions);
        }
        private void UnselectOption()
        {
            TransferOption(SelectedOptions, AvailableOptions);
        }
        private static void TransferOption(ObservableCollection<OptionField> oldCollection, ObservableCollection<OptionField> newCollection)
        {
            var selectedOption = oldCollection.SingleOrDefault(x => x.IsSelected);
            if (selectedOption is null) { return; }
            oldCollection.Remove(selectedOption);
            selectedOption.IsSelected = false;
            newCollection.Add(selectedOption);
        }
        private void SelectAllOptions()
        {
            TransferAllOptions(AvailableOptions, SelectedOptions);
        }
        private void UnselectAllOptions()
        {
            TransferAllOptions(SelectedOptions, AvailableOptions);
        }
        private static void TransferAllOptions(ObservableCollection<OptionField> oldCollection, ObservableCollection<OptionField> newCollection)
        {
            foreach (var option in oldCollection)
            {
                option.IsSelected = false;
                newCollection.Add(option);
            }
            oldCollection.Clear();
        }
        private void MoveOptionUp()
        {
            MoveOption(MoveDirection.Up);
        }
        private void MoveOptionDown()
        {
            MoveOption(MoveDirection.Down);
        }
        private void MoveOption(MoveDirection dir)
        {
            var selectedOption = _selectedOptions.SingleOrDefault(x => x.IsSelected);
            if (selectedOption is null) { return; }
            var selectedOptionIndex = _selectedOptions.IndexOf(selectedOption);

            if (dir == MoveDirection.Down)
            {
                if (selectedOptionIndex == _selectedOptions.Count - 1) { return; }
                _selectedOptions.Move(selectedOptionIndex, selectedOptionIndex + 1);
            }
            else if (dir == MoveDirection.Up)
            {
                if (selectedOptionIndex == 0) { return; }
                _selectedOptions.Move(selectedOptionIndex, selectedOptionIndex - 1);
            }
        }
        private void SubmitSelected(Window? window)
        {
            if (window is null) { return; }
            window.DialogResult = true;
        }
        private bool CanExecuteOnAvailable()
        {
            return AvailableOptions.Any();
        }
        private bool CanExecuteOnSelected()
        {
            return SelectedOptions.Any();
        }
    }
    enum MoveDirection
    {
        Up,
        Down
    }
}
