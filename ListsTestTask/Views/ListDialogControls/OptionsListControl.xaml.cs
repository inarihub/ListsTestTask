using CommunityToolkit.Mvvm.Input;
using ListsTestTask.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ListsTestTask.Views.ListDialogControls
{
    /// <summary>
    /// Interaction logic for OptionsListControl.xaml
    /// </summary>
    public partial class OptionsListControl : UserControl
    {
        public OptionsListControl()
        {
            InitializeComponent();
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(OptionsListControl),
                new PropertyMetadata(-1));

        public ObservableCollection<OptionField> Collection
        {
            get { return (ObservableCollection<OptionField>)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(ObservableCollection<OptionField>), typeof(OptionsListControl),
                new PropertyMetadata(null));

        public IRelayCommand DoubleClickCommand
        {
            get { return (IRelayCommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(IRelayCommand), typeof(OptionsListControl),
                new PropertyMetadata(null));
    }
}
