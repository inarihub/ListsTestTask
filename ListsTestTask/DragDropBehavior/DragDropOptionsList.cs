using ListsTestTask.Models;
using ListsTestTask.Views.ListDialogControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ListsTestTask.DragDropBehavior
{
    public class DragDropOptionsList
    {
        static Point _startDragPos;
        static bool _isReadyToDrug;
        static DataObject _dragObject;

        public static bool GetDragEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(DragEnabledProperty);
        }

        public static void SetDragEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(DragEnabledProperty, value);
        }

        public static readonly DependencyProperty DragEnabledProperty =
            DependencyProperty.RegisterAttached("DragEnabled", typeof(bool), typeof(OptionsListControl), new PropertyMetadata(OnDragEnabledChanged));

        private static void OnDragEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && obj is OptionsListControl control)
            {
                OptionsListControl optionList = control;
                optionList.optionsList.PreviewMouseLeftButtonDown += OptionsList_PreviewMouseLeftButtonDown;
                optionList.optionsList.PreviewMouseLeftButtonUp += OptionsList_PreviewMouseLeftButtonUp;
                optionList.optionsList.MouseMove += OptionsList_MouseMove;
                optionList.optionsList.Drop += OptionsList_Drop;
                optionList.AllowDrop = true;
            }
        }

        private static void OptionsList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isReadyToDrug = false;
            ListBox listSource = (ListBox)sender;
        }

        private static void OptionsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ListBox listSource = (ListBox)sender;
            if (listSource.Items.Count == 0) { return; }
            _startDragPos = e.GetPosition(listSource);
            _isReadyToDrug = true;
            
        }

        private static void OptionsList_Drop(object sender, DragEventArgs e)
        {
            ListBox lb = (ListBox)sender;

            var result = VisualTreeHelper.HitTest(lb, e.GetPosition(lb)).VisualHit;
            var item = (result as FrameworkElement)?.DataContext;
            var itemIndex = 0;
            if (item is not null && item is OptionField && lb.Items.IndexOf(item) > 0)
            {
                itemIndex = lb.Items.IndexOf(item); ;
            }
            else
            {
                itemIndex = lb.Items.Count;
            }

            if (!e.Data.GetDataPresent(typeof(OptionField))) { e.Handled = true; return; }
            OptionField field = (OptionField)e.Data.GetData(typeof(OptionField));

            (lb.ItemsSource as ObservableCollection<OptionField>)?.Insert(itemIndex, field);
            e.Effects = DragDropEffects.Move;
            e.Handled = true;
        }
        
        private static void OptionsList_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox listSource = (ListBox)sender;
            Point mousePos = e.GetPosition(listSource);

            if (_isReadyToDrug && listSource.SelectedIndex >= 0 &&
                (Math.Abs(_startDragPos.X - mousePos.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(_startDragPos.Y - mousePos.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                e.Handled = true;
                _isReadyToDrug = false;
                int selectedIndex = listSource.SelectedIndex;
                OptionField sourceObj = (OptionField)listSource.Items.GetItemAt(selectedIndex);
                DataObject dObj = new(typeof(OptionField), sourceObj);
                (listSource.ItemsSource as ObservableCollection<OptionField>)?.Remove(sourceObj);
                
                var results = DragDrop.DoDragDrop(listSource, dObj, DragDropEffects.Move);
                if (!results.HasFlag(DragDropEffects.Move)) 
                { 
                    (listSource.ItemsSource as ObservableCollection<OptionField>)?.Insert(selectedIndex, sourceObj);
                    listSource.SelectedIndex = selectedIndex;
                }
                Trace.WriteLine(sourceObj.Name);
            }
        }
    }
}
