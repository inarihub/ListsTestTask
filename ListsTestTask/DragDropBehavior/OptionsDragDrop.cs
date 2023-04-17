using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ListsTestTask.DragDropBehavior
{
    public class OptionsDragDrop
    {
        readonly DragDropService _dragDropHelper;
        readonly ListBox _source;

        public OptionsDragDrop(ListBox sourceList)
        {
            _source = sourceList;
            _dragDropHelper = DragDropService.GetDragDrop();
        }

        public void SetDragDrop()
        {
            _source.PreviewMouseLeftButtonDown += OptionsList_PreviewMouseLeftButtonDown;
            _source.PreviewMouseLeftButtonUp += OptionsList_PreviewMouseLeftButtonUp;
            _source.DragEnter += OptionsList_DragEnter;
            _source.DragLeave += OptionsList_DragLeave;
            _source.QueryContinueDrag += OptionsList_QueryContinueDrag;
            _source.MouseMove += OptionsList_MouseMove;
            _source.Drop += OptionsList_Drop;
        }

        public void UnsetDragDrop()
        {
            _source.PreviewMouseLeftButtonDown -= OptionsList_PreviewMouseLeftButtonDown;
            _source.PreviewMouseLeftButtonUp -= OptionsList_PreviewMouseLeftButtonUp;
            _source.DragEnter -= OptionsList_DragEnter;
            _source.DragLeave -= OptionsList_DragLeave;
            _source.QueryContinueDrag -= OptionsList_QueryContinueDrag;
            _source.MouseMove -= OptionsList_MouseMove;
            _source.Drop -= OptionsList_Drop;
        }

        private void OptionsList_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _dragDropHelper.SetDragReady((ListBox)sender);
        }

        private void OptionsList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragDropHelper.UnsetDragReady();
        }

        private void OptionsList_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragDropHelper.IsDragGesture((ListBox)sender))
            {
                e.Handled = true;
                _dragDropHelper.BeginDragDrop((ListBox)sender, (UIElement)e.OriginalSource);
            }
        }

        private void OptionsList_DragEnter(object sender, DragEventArgs e)
        {
            ListBox listTarget = (ListBox)sender;
            UIElement element = (UIElement)e.OriginalSource;

            if (!IsAllowedRegion(listTarget, element)) return;

            _dragDropHelper.OnEntered(listTarget, element, e.Data);
        }

        private void OptionsList_DragLeave(object sender, DragEventArgs e)
        {
            ListBox listTarget = (ListBox)sender;
            UIElement element = (UIElement)e.OriginalSource;

            if (IsAllowedRegion(listTarget, element)) return;
           
            _dragDropHelper.OnLeave();
        }

        private void OptionsList_Drop(object sender, DragEventArgs e)
        {
            ListBox listTarget = (ListBox)sender;
            UIElement element = (UIElement)e.OriginalSource;

            if (IsAllowedRegion(listTarget, element))
            e.Effects = DragDropEffects.Move;
        }

        private void OptionsList_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            if (e.EscapePressed || e.KeyStates.HasFlag(DragDropKeyStates.RightMouseButton))
            {
                e.Action = DragAction.Cancel;
            }
        }

        private static bool IsAllowedRegion(ListBox listTarget, UIElement element)
        {
            var container = listTarget.ContainerFromElement(element);
            return container is ListBoxItem || element is ScrollViewer;
        }
    }
}
