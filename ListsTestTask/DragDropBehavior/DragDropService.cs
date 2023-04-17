using ListsTestTask.Models;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace ListsTestTask.DragDropBehavior
{
    public class DragDropService
    {
        private static DragDropService? _dragDropInstance;

        bool IsDragReady;
        bool IsInItemsControl;
        Point? StartDragPos;
        OptionField? PreviewOption;
        ObservableCollection<OptionField>? PreviewCollection;

        private DragDropService()
        {
            StartDragPos = null;
            IsDragReady = false;
            IsInItemsControl = true;
        }

        public static DragDropService GetDragDrop()
        {
            if (_dragDropInstance is null)
            {
                var instance = new DragDropService();
                _dragDropInstance = instance;
            }

            return _dragDropInstance;
        }

        internal void SetDragReady(ListBox listBox)
        {
            StartDragPos = Mouse.GetPosition(listBox);
            IsDragReady = true;
        }

        internal void UnsetDragReady()
        {
            StartDragPos = null;
            IsDragReady = false;
        }

        internal bool IsDragGesture(Selector listSource)
        {
            if (!IsDragReady)
                return false;

            Point oldPos = StartDragPos!.Value;
            Point newPos = Mouse.GetPosition(listSource);

            return
                Math.Abs(oldPos.X - newPos.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(oldPos.Y - newPos.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        internal void BeginDragDrop(ListBox source, UIElement element)
        {
            UnsetDragReady();

            if (!TryGetOption(source, element, out OptionField option))
                return;

            int selectedIndex = source.SelectedIndex;
            CreatePreview(source, option);
            DataObject data = new(typeof(OptionField), option);

            var result = DragDrop.DoDragDrop(source, data, DragDropEffects.Move | DragDropEffects.None);

            if (IsInItemsControl && result == DragDropEffects.None)
            {
                RemovePreview();
            }
            if ((!IsInItemsControl && result == DragDropEffects.Move) || result == DragDropEffects.None)
                ReturnElement(source, option, selectedIndex);
        }

        private static bool TryGetOption(ListBox source, UIElement element, out OptionField option)
        {
            if (source.ContainerFromElement(element) is ListBoxItem container &&
                container.Content is OptionField data)
            {
                option = data;
                return true;
            }

            option = null!;
            return false;
        }

        public void OnEntered(ListBox listTarget, UIElement targetElement, IDataObject data)
        {
            var content = (OptionField)data.GetData(typeof(OptionField));

            if (!IsInItemsControl)
            {
                InsertPreview(listTarget, content);
            }
            if (listTarget.ContainerFromElement(targetElement) is ListBoxItem item)
            {
                MovePreview(item, content);
            }
        }

        private void InsertPreview(ListBox listTarget, OptionField content)
        {
            CreatePreview(listTarget, content);
            PreviewCollection!.Add(content);
            IsInItemsControl = true;
            listTarget.SelectedItem = content;
        }

        private void CreatePreview(ListBox listTarget, OptionField content)
        {
            var collection = listTarget.ItemsSource as ObservableCollection<OptionField>;
            PreviewOption = content;
            PreviewCollection = collection;       
        }

        private void MovePreview(ListBoxItem item, OptionField content)
        {
            if (PreviewCollection is null || item.Content is not OptionField target || target == content) return;

            int newIndex = PreviewCollection.IndexOf(target);
            int oldIndex = PreviewCollection.IndexOf(content);

            PreviewCollection.Move(oldIndex, newIndex);
        }

        public void OnLeave()
        {
            RemovePreview();
            IsInItemsControl = false;
        }

        private void ReturnElement(ListBox listSource, OptionField option, int selectedIndex)
        {
            var collection = listSource.ItemsSource as ObservableCollection<OptionField>;
            if (collection is null) return;

            collection.Insert(selectedIndex, option);
            listSource.SelectedItem = option;
            IsInItemsControl = true;
        }

        internal void RemovePreview()
        {
            if (PreviewOption is null || PreviewCollection is null) return;
            PreviewCollection.Remove(PreviewOption);
            PreviewOption = null;
            PreviewCollection = null;
        }
    }
}
