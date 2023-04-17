using CommunityToolkit.Mvvm.Input;
using ListsTestTask.DragDropBehavior;
using ListsTestTask.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ListsTestTask.Views.ListDialogControls
{
    public partial class OptionsListControl : UserControl
    {
        readonly OptionsDragDrop _optionsDragDrop;

        public OptionsListControl()
        {
            InitializeComponent();
            _optionsDragDrop = new(optionsList);
        }

        public int Index
        {
            get { return (int)GetValue(IndexProperty); }
            set { SetValue(IndexProperty, value); }
        }

        public ObservableCollection<OptionField> Collection
        {
            get { return (ObservableCollection<OptionField>)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        public bool DragDrop
        {
            get { return (bool)GetValue(DragDropProperty); }
            set { SetValue(DragDropProperty, value); }
        }

        public IRelayCommand DoubleClickCommand
        {
            get { return (IRelayCommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty IndexProperty =
            DependencyProperty.Register("Index", typeof(int), typeof(OptionsListControl),
                new PropertyMetadata(-1));

        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(ObservableCollection<OptionField>), typeof(OptionsListControl),
                new PropertyMetadata(null));

        public static readonly DependencyProperty DragDropProperty =
            DependencyProperty.Register("DragDrop", typeof(bool), typeof(OptionsListControl), new PropertyMetadata(false, DragDropChangedCallback));

        private static void DragDropChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == (bool)e.OldValue) return;
            if ((bool)e.NewValue) 
            {
                (d as OptionsListControl)?._optionsDragDrop.SetDragDrop();
            }
            else
            {
                (d as OptionsListControl)?._optionsDragDrop.UnsetDragDrop();
            }
        }

        public static readonly DependencyProperty DoubleClickCommandProperty =
            DependencyProperty.Register("DoubleClickCommand", typeof(IRelayCommand), typeof(OptionsListControl),
                new PropertyMetadata(null));

        private void ListBoxItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DoubleClickCommand?.Execute(null);
                e.Handled = true;
            }
        }
    }
}
