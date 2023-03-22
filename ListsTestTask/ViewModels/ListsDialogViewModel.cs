using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

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
        public ICommand SelectCommand { get; set; }
        public ICommand SelectAllCommand { get; set; }
        public ICommand UnselectCommand { get; set; }
        public ICommand UnselectAllCommand { get; set; }
        public ICommand MoveSelectedUpCommand { get; set; }
        public ICommand MoveSelectedDownCommand { get; set; }
        public ICommand SubmitSelectedCommand { get; set; }
        public ListsDialogViewModel(List<OptionField> availableOptions, ObservableCollection<OptionField> selectedOptions)
        {
            _availableOptions = new(availableOptions.Except(selectedOptions));
            _selectedOptions = new(selectedOptions);
            SelectCommand = new RelayCommand(SelectOption);
            SelectAllCommand = new RelayCommand(SelectAllOptions);
            UnselectCommand = new RelayCommand(UnselectOption);
            UnselectAllCommand = new RelayCommand(UnselectAllOptions);
            MoveSelectedUpCommand = new RelayCommand(MoveOptionUp);
            MoveSelectedDownCommand = new RelayCommand(MoveOptionDown);
            SubmitSelectedCommand = new RelayCommand<Window>(SubmitSelected);
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
    }
    enum MoveDirection
    {
        Up,
        Down
    }
}
