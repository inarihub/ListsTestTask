using ListsTestTask.Models;
using ListsTestTask.Views.ListDialogControls;
using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace ListsTestTask.DragDropBehavior;

enum ElementVerticalHalf
{
    Upper,
    Lower
}

public class DragDropOptionsList
{
    static Point _startDragPos;
    static bool _isReadyToDrug;
    static bool _isItemVisible;
    static int _targetIndex;
    static OptionField? _previewItem;
    static ObservableCollection<OptionField>? _previewCollection;

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
        Trace.WriteLine(obj.ToString());
        if (obj is OptionsListControl control)
        {
            if ((bool)e.NewValue)
            {
                control.optionsList.PreviewMouseLeftButtonDown += OptionsList_PreviewMouseLeftButtonDown;
                control.optionsList.PreviewMouseLeftButtonUp += OptionsList_PreviewMouseLeftButtonUp;
                control.optionsList.PreviewDragEnter += OptionsList_DragEnter;
                control.optionsList.PreviewDragLeave += OptionsList_DragLeave;
                control.optionsList.GiveFeedback += OptionsList_GiveFeedback;
                control.optionsList.MouseMove += OptionsList_MouseMove;
                control.optionsList.Drop += OptionsList_Drop;
            }
            else if ((bool)e.OldValue)
            {
                control.optionsList.PreviewMouseLeftButtonDown -= OptionsList_PreviewMouseLeftButtonDown;
                control.optionsList.PreviewMouseLeftButtonUp -= OptionsList_PreviewMouseLeftButtonUp;
                control.optionsList.DragEnter -= OptionsList_DragEnter;
                control.optionsList.DragLeave -= OptionsList_DragLeave;
                control.optionsList.GiveFeedback -= OptionsList_GiveFeedback;
                control.optionsList.MouseMove -= OptionsList_MouseMove;
                control.optionsList.Drop -= OptionsList_Drop;
            }
        }
    }

    private static void OptionsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        ListBox listSource = (ListBox)sender;
        if (listSource.Items.Count == 0)
            return;

        _startDragPos = e.GetPosition(listSource);
        _isReadyToDrug = true;
    }

    private static void OptionsList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _isReadyToDrug = false;
    }

    private static void OptionsList_MouseMove(object sender, MouseEventArgs e)
    {
        ListBox listSource = (ListBox)sender;
        Point mousePos = e.GetPosition(listSource);

        if (_isReadyToDrug && listSource.Items.CurrentPosition >= 0 && IsContentElement(listSource, e.OriginalSource) &&
            (Math.Abs(_startDragPos.X - mousePos.X) > SystemParameters.MinimumHorizontalDragDistance ||
            Math.Abs(_startDragPos.Y - mousePos.Y) > SystemParameters.MinimumVerticalDragDistance))
        {
            BeginDragDrop(listSource);
        }
    }

    private static void BeginDragDrop(ListBox listSource)
    {
        _isReadyToDrug = false;
        _isItemVisible = true;
        int selectedIndex = listSource.SelectedIndex;
        OptionField sourceObj = (OptionField)listSource.SelectedItem;
        DataObject _dataObj = new(typeof(OptionField), sourceObj);

        _previewItem = sourceObj;
        _previewCollection = listSource.ItemsSource as ObservableCollection<OptionField>;

        var results = DragDrop.DoDragDrop(listSource, _dataObj, DragDropEffects.Move);

        if (results == DragDropEffects.None && _previewItem is null)
        {
            var collection = listSource.ItemsSource as ObservableCollection<OptionField>;
            collection?.Insert(selectedIndex, sourceObj);
            listSource.SelectedIndex = selectedIndex;
        }
    }

    private static void OptionsList_DragEnter(object sender, DragEventArgs e)
    {
        ListBox listTarget = (ListBox)sender;

        if (!e.Data.GetDataPresent(typeof(OptionField)) || e.Effects != DragDropEffects.Move) 
            return;

        if (!_isItemVisible && IsContentElement(listTarget, e.OriginalSource))
        {
            InsertPreview(listTarget, e.OriginalSource, e.Data);
            Trace.WriteLine(e.OriginalSource.ToString());
        }
        else if (!IsContentElement(listTarget, e.OriginalSource))
        {
            RemovePreview(listTarget, e.Data);
            e.Effects = DragDropEffects.None;
        }
        else
        {
            MovePreview(listTarget, e.OriginalSource, e.Data, e);
        }
    }

    private static void OptionsList_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        if (e.Effects != DragDropEffects.Move)
        {
            Mouse.SetCursor(Cursors.No);

            if (_previewItem is not null && _isItemVisible)
            {
                _previewCollection?.Remove(_previewItem);
                _previewItem = null;
                _previewCollection = null;
                _targetIndex = 0;
                _isItemVisible = false;
            }
        }
    }

    private static void OptionsList_DragLeave(object sender, DragEventArgs e)
    {
        //e.Handled = true;
        //if (e.Data.GetDataPresent(typeof(OptionField)))
        //    Trace.WriteLine(e.OriginalSource.ToString());
        //ListBox listTarget = (ListBox)sender;

        //if (_isItemVisible)
        //{
        //    var result = VisualTreeHelper.HitTest(sender as ListBox, e.GetPosition(sender as UIElement));
        //    if (result is not null || IsContentElement(listTarget, e.OriginalSource))
        //    { return; }
        //    _isItemVisible = false;
        //    var dataObj = (OptionField)e.Data.GetData(typeof(OptionField));
        //    var collection = listTarget.ItemsSource as ObservableCollection<OptionField>;
        //    collection?.Remove(dataObj);
        //    _targetIndex = 0;
        //}
    }

    private static void OptionsList_Drop(object sender, DragEventArgs e)
    {
        if (e.AllowedEffects != DragDropEffects.Move && !IsContentElement((ListBox)sender, e.OriginalSource))
        {
            e.Effects = DragDropEffects.None;
            return;
        }

        e.Effects = DragDropEffects.Move;
    }

    private static ListBoxItem GetItemContainer(ListBox listBox, object obj)
    {
        return (ListBoxItem)listBox.ContainerFromElement(obj as UIElement);
    }

    private static void PreviewInsert(OptionField data, ListBox listTarget)
    {
        _previewCollection = listTarget.ItemsSource as ObservableCollection<OptionField>;
        _previewItem = data;
        _previewCollection?.Insert(_targetIndex, _previewItem);
        listTarget.SelectedItem = _previewCollection?[_targetIndex];
    }

    private static void SetIndexFromTarget(OptionField? target, ListBox listTarget)
    {
        if (target is not null && _targetIndex >= 0)
        {
            _targetIndex = listTarget.Items.IndexOf(target);
        }
    }

    private static void InsertPreview(ListBox listTarget, object obj, IDataObject data)
    {
        _isItemVisible = true;
        var dataObj = (OptionField)data.GetData(typeof(OptionField));
        _previewItem = GetItemContainer(listTarget, obj)?.Content as OptionField;
        SetIndexFromTarget(_previewItem, listTarget);
        PreviewInsert(dataObj, listTarget);
    }

    private static void RemovePreview(ListBox listTarget, IDataObject data)
    {
        //var collection = listTarget.ItemsSource as ObservableCollection<OptionField>;
        //var dataObj = (OptionField)data.GetData(typeof(OptionField));
        //collection?.Remove(dataObj);
        //_targetIndex = 0;
        //_isItemVisible = false;
    }

    private static void MovePreview(ListBox listTarget, object obj, IDataObject data, DragEventArgs e)
    {
        var target = GetItemContainer(listTarget, obj)?.Content as OptionField;
        var dataObj = (OptionField)data.GetData(typeof(OptionField));
        if (target is not null && target == dataObj) return;
        int oldIndex = _targetIndex;
       
        SetIndexFromTarget(target, listTarget);

        //if (GetAllocatonHalf(listTarget, e) == ElementVerticalHalf.Lower)
        //    _targetIndex++;

        var collection = listTarget.ItemsSource as ObservableCollection<OptionField>;
        collection?.Move(oldIndex, _targetIndex);
        listTarget.SelectedItem = collection?[_targetIndex];
    }

    private static ElementVerticalHalf? GetAllocatonHalf(ListBox listTarget, DragEventArgs e)
    {
        if (GetItemContainer(listTarget, e.OriginalSource) is ListBoxItem listItem)
        {
            var pos = e.GetPosition(listItem);
            var size = listItem.RenderSize;
            if (pos.Y > size.Height / 2)
                return ElementVerticalHalf.Lower;
            else
                return ElementVerticalHalf.Upper;
        }
        else
        {
            _targetIndex = listTarget.Items.Count;
            return null;
        }
    }

    private static bool IsContentElement(ListBox owner, object element)
    {
        ListBox listTarget = owner;

        return (listTarget.ContainerFromElement(element as UIElement) is ListBoxItem ||
            element is ScrollViewer || element is Border);
    }
}
