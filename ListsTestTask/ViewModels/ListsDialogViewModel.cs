using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Collections.Specialized;

namespace ListsTestTask.ViewModels;

enum MoveDirection
{
    Up,
    Down
}

enum TransferType
{
    ToSelected,
    ToAvailable
}

class ListsDialogViewModel : BaseViewModel
{
    private int _selectedIndex;
    private int _availableIndex;
    public int SelectedIndex
    {
        get { return _selectedIndex; }
        set
        {
            _selectedIndex = value;
            UnselectCommand.NotifyCanExecuteChanged();
            MoveSelectedCommand.NotifyCanExecuteChanged();
        }
    }
    public int AvailableIndex
    {
        get { return _availableIndex; }
        set
        {
            _availableIndex = value;
            SelectCommand.NotifyCanExecuteChanged();
        }
    }

    public ObservableCollection<OptionField> AvailableOptions { get; set; }
    public ObservableCollection<OptionField> SelectedOptions { get; set; }

    public IRelayCommand SelectCommand { get; set; }
    public IRelayCommand UnselectCommand { get; set; }
    public IRelayCommand SelectAllCommand { get; set; }
    public IRelayCommand UnselectAllCommand { get; set; }
    public IRelayCommand MoveSelectedCommand { get; set; }
    public IRelayCommand SubmitSelectedCommand { get; set; }

    public ListsDialogViewModel(List<OptionField> availableOptions, ObservableCollection<OptionField> selectedOptions)
    {
        AvailableOptions = new(availableOptions.Except(selectedOptions));
        SelectedOptions = new(selectedOptions);
        AvailableOptions.CollectionChanged += OnCollectionChanged;
        SelectedOptions.CollectionChanged += OnCollectionChanged;
        SubmitSelectedCommand = new RelayCommand<Window>(SubmitSelected);
        MoveSelectedCommand = new RelayCommand<MoveDirection>(MoveOption, CanExecuteOnMove);
        SelectCommand = new RelayCommand(SelectOption, CanExecuteOnSelect);
        UnselectCommand = new RelayCommand(UnselectOption, CanExecuteOnUnselect);
        SelectAllCommand = new RelayCommand(SelectAllOptions, CanExecuteOnAllSelect);
        UnselectAllCommand = new RelayCommand(UnselectAllOptions, CanExecuteOnAllUnselect);
    }

    private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        SelectCommand.NotifyCanExecuteChanged();
        UnselectCommand.NotifyCanExecuteChanged();
        SelectAllCommand.NotifyCanExecuteChanged();
        UnselectAllCommand.NotifyCanExecuteChanged();
        MoveSelectedCommand.NotifyCanExecuteChanged();
    }

    private void SelectOption()
    {
        TransferOption(AvailableOptions, SelectedOptions, AvailableIndex);
    }

    private void UnselectOption()
    {
        TransferOption(SelectedOptions, AvailableOptions, SelectedIndex);
    }

    private static void TransferOption(ObservableCollection<OptionField> oldCollection, ObservableCollection<OptionField> newCollection, int index)
    {
        if (index < 0) { return; }
        var selectedOption = oldCollection[index];
        oldCollection.Remove(selectedOption);
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

    private void MoveOption(MoveDirection direction)
    {
        if (SelectedIndex < 0) { return; }
        var selectedOption = SelectedOptions[SelectedIndex];

        if (direction == MoveDirection.Down)
        {
            if (SelectedIndex == SelectedOptions.Count - 1) { return; }
            SelectedOptions.Move(SelectedIndex, SelectedIndex + 1);
        }
        else if (direction == MoveDirection.Up)
        {
            if (SelectedIndex == 0) { return; }
            SelectedOptions.Move(SelectedIndex, SelectedIndex - 1);
        }
    }

    private void SubmitSelected(Window? window)
    {
        if (window is null) { return; }
        window.DialogResult = true;
    }

    private bool CanExecuteOnSelect()
    {
        return AvailableIndex != -1;
    }

    private bool CanExecuteOnUnselect()
    {
        return SelectedIndex != -1;
    }

    private bool CanExecuteOnAllSelect()
    {
        return AvailableOptions.Any();
    }

    private bool CanExecuteOnAllUnselect()
    {
        return SelectedOptions.Any();
    }

    private bool CanExecuteOnMove(MoveDirection direction)
    {
        if (direction == MoveDirection.Down)
            return SelectedIndex < SelectedOptions.Count - 1 && SelectedIndex != -1;

        return SelectedIndex > 0;
    }
}
