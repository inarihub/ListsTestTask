using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using ListsTestTask.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace ListsTestTask.ViewModels
{
    class MainViewModel : BaseViewModel
    {
        private readonly List<OptionField> _availableOptions;
        public IRelayCommand ShowDialogViewCommand { get; set; }
        public IRelayCommand CloseMainWindowCommand { get; set; }

        private ObservableCollection<OptionField> _optionsToDisplay;
        public ObservableCollection<OptionField> OptionsToDisplay
        {
            get => _optionsToDisplay;
            set
            {
                _optionsToDisplay = value;
                OnPropertyChanged(nameof(OptionsToDisplay));
            }
        }
        public MainViewModel()
        {
            _availableOptions = new List<OptionField>();
            GetOptionsList("/OptionsTxtFile.txt");
            _optionsToDisplay = new ObservableCollection<OptionField>();
            ShowDialogViewCommand = new RelayCommand(ShowDialogView);
            CloseMainWindowCommand = new RelayCommand(CloseMainWindow);
        }
        private void GetOptionsList(string path)
        {
            using var stream = Application.GetContentStream(new Uri(path, UriKind.Relative)).Stream;
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) is not null)
            {
                if (!_availableOptions.Exists(x => x.Name == line))
                    _availableOptions.Add(new OptionField(line));
            }
            reader.Close();
        }
        private void ShowDialogView()
        {
            var listsDialogVM = new ListsDialogViewModel(_availableOptions, OptionsToDisplay);

            ListsDialogWindow listsDialogWin = new()
            {
                Owner = App.Current.MainWindow,
                DataContext = listsDialogVM
            };

            var isApplied = listsDialogWin.ShowDialog() ?? throw new NullReferenceException();

            if (isApplied)
            {
                UpdateSelected(listsDialogVM);
            }
        }
        private void UpdateSelected(ListsDialogViewModel listsDialogVM)
        {
            OptionsToDisplay.Clear();
            OptionsToDisplay = listsDialogVM.SelectedOptions;
        }
        private void CloseMainWindow()
        {
            App.Current.Shutdown();
        }
    }
}
