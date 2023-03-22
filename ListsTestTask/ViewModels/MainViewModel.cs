using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using ListsTestTask.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace ListsTestTask.ViewModels
{
    class MainViewModel
    {
        private readonly List<OptionField> _availableOptions;
        public ICommand ShowDialogViewCommand { get; set; }
        public ICommand CloseMainWindowCommand { get; set; }
        public ObservableCollection<OptionField> OptionsToDisplay { get; set; }
        public MainViewModel()
        {
            _availableOptions = new List<OptionField>();
            GetOptionsList("/OptionsTxtFile.txt");
            OptionsToDisplay = new ObservableCollection<OptionField>();
            ShowDialogViewCommand = new RelayCommand(ShowDialogView);
            CloseMainWindowCommand = new RelayCommand(CloseMainWindow);
        }
        private void GetOptionsList(string path)
        {
            using var stream = Application.GetContentStream(new Uri(path, UriKind.Relative)).Stream;
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                _availableOptions.Add(new OptionField(line, false));
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
            foreach (var item in listsDialogVM.SelectedOptions)
            {
                OptionsToDisplay.Add(item);
            }
        }
        private void CloseMainWindow()
        {
            App.Current.Shutdown();
        }
    }
}
