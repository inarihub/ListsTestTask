using ListsTestTask.Models;
using ListsTestTask.Views.ListDialogControls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ListsTestTask.DragDropBehavior;

public class DragDropOptionsList
{
    static Point _startDragPos;
    static bool _isReadyToDrug;
    static bool _isInList;
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
                control.optionsList.QueryContinueDrag += OptionsList_QueryContinueDrag;
                control.optionsList.GiveFeedback += OptionsList_GiveFeedback;
                control.optionsList.MouseMove += OptionsList_MouseMove;
                control.optionsList.Drop += OptionsList_Drop;
            }
            else if ((bool)e.OldValue)
            {
                control.optionsList.PreviewMouseLeftButtonDown -= OptionsList_PreviewMouseLeftButtonDown;
                control.optionsList.PreviewMouseLeftButtonUp -= OptionsList_PreviewMouseLeftButtonUp;
                control.optionsList.DragEnter -= OptionsList_DragEnter;
                control.optionsList.QueryContinueDrag -= OptionsList_QueryContinueDrag;
                control.optionsList.GiveFeedback -= OptionsList_GiveFeedback;
                control.optionsList.MouseMove -= OptionsList_MouseMove;
                control.optionsList.Drop -= OptionsList_Drop;
            }
        }
    }

    private static void OptionsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
        if (e.EscapePressed || e.KeyStates.HasFlag(DragDropKeyStates.RightMouseButton)) 
        {
            e.Action = DragAction.Cancel;
            RemovePreview();
            e.Handled = true;
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
        _isInList = true;
        int selectedIndex = listSource.SelectedIndex;
        _targetIndex = selectedIndex;
        OptionField sourceObj = (OptionField)listSource.SelectedItem;
        DataObject _dataObj = new(typeof(OptionField), sourceObj);

        _previewItem = sourceObj;
        _previewCollection = listSource.ItemsSource as ObservableCollection<OptionField>;

        var results = DragDrop.DoDragDrop(listSource, _dataObj, DragDropEffects.Move);

        if (!_isInList)
        {
            ReturnElement(listSource, sourceObj, selectedIndex);
        }
    }

    private static void ReturnElement(ListBox listSource, OptionField sourceObj, int selectedIndex)
    {
        var collection = listSource.ItemsSource as ObservableCollection<OptionField>;
        collection?.Insert(selectedIndex, sourceObj);
        listSource.SelectedIndex = selectedIndex;
        _isInList = true;
    }

    private static void OptionsList_DragEnter(object sender, DragEventArgs e)
    {
        ListBox listTarget = (ListBox)sender;

        if (!e.Data.GetDataPresent(typeof(OptionField)) || e.Effects != DragDropEffects.Move) 
            return;

        if (!_isInList && IsContentElement(listTarget, e.OriginalSource))
        {
            InsertPreview(listTarget, e.OriginalSource, e.Data);
        }
        else if (IsContentElement(listTarget, e.OriginalSource))
        {
            MovePreview(listTarget, e.OriginalSource, e.Data, e);
        }
        else
        {
            e.Effects = DragDropEffects.None;
        }
    }

    private static void OptionsList_GiveFeedback(object sender, GiveFeedbackEventArgs e)
    {
        if (e.Effects != DragDropEffects.Move)
        {
            Mouse.SetCursor(Cursors.No);

            if (_previewItem is not null && _isInList)
            {
                RemovePreview();
            }
        }
    }

    private static void RemovePreview()
    {
        _previewCollection?.Remove(_previewItem!);
        _previewItem = null;
        _previewCollection = null;
        _targetIndex = 0;
        _isInList = false;
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

    private static void InsertPreview(ListBox listTarget, object obj, IDataObject data)
    {   
        var dataObj = (OptionField)data.GetData(typeof(OptionField));
        _previewItem = GetItemContainer(listTarget, obj)?.Content as OptionField;
        SetIndexFromTarget(_previewItem, listTarget);
        SetInsert(dataObj, listTarget);
        _isInList = true;
    }

    private static void MovePreview(ListBox listTarget, object obj, IDataObject data, DragEventArgs e)
    {
        var target = GetItemContainer(listTarget, obj)?.Content as OptionField;
        var dataObj = (OptionField)data.GetData(typeof(OptionField));
        if (target is not null && target == dataObj) return;
        int oldIndex = _targetIndex;
        SetIndexFromTarget(target, listTarget);

        var collection = listTarget.ItemsSource as ObservableCollection<OptionField>;
        collection?.Move(oldIndex, _targetIndex);
        listTarget.SelectedItem = collection?[_targetIndex];
    }

    private static void SetInsert(OptionField data, ListBox listTarget)
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

    private static bool IsContentElement(ListBox owner, object element)
    {
        ListBox listTarget = owner;

        return (listTarget.ContainerFromElement(element as UIElement) is ListBoxItem ||
            element is ScrollViewer || element is Border);
    }

    private static ListBoxItem GetItemContainer(ListBox listBox, object obj)
    {
        return (ListBoxItem)listBox.ContainerFromElement(obj as UIElement);
    }
}
