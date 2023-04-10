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

        public ObservableCollection<OptionField> Index
        {
            get { return (ObservableCollection<OptionField>)GetValue(IndexProperty); }
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
    }
}
