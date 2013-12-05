using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PizzaNetControls.Dialogs
{
    public class ModalDialog : UserControl
    {
        public ModalDialog()
        {
            Visibility = Visibility.Hidden;
        }

        private bool _parentWasEnabled = true;

        public bool IsShown
        {
            get { return (bool)GetValue(IsShownProperty); }
            set { SetValue(IsShownProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsShown.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsShownProperty =
            DependencyProperty.Register("IsShown", typeof(bool), typeof(ModalDialog), new UIPropertyMetadata(false, IsShownChangedCallback));

        public static void IsShownChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == true)
            {
                ModalDialog dlg = (ModalDialog)d;
                dlg.Show();
            }
            else
            {
                ModalDialog dlg = (ModalDialog)d;
                dlg.Hide();
            }
        }

        #region OverlayOn

        public UIElement OverlayOn
        {
            get { return (UIElement)GetValue(OverlayOnProperty); }
            set { SetValue(OverlayOnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Parent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OverlayOnProperty =
            DependencyProperty.Register("OverlayOn", typeof(UIElement), typeof(ModalDialog), new UIPropertyMetadata(null));

        #endregion

        public void Show()
        {

            // Force recalculate binding since Show can be called before binding are calculated            
            BindingExpression expressionOverlayParent = this.GetBindingExpression(OverlayOnProperty);
            if (expressionOverlayParent != null)
            {
                expressionOverlayParent.UpdateTarget();
            }

            if (OverlayOn == null)
            {
                throw new InvalidOperationException("Required properties are not bound to the model.");
            }

            Visibility = System.Windows.Visibility.Visible;

            _parentWasEnabled = OverlayOn.IsEnabled;
            OverlayOn.IsEnabled = false;

            if (ModalDialogShown != null)
                ModalDialogShown(this, new EventArgs());
        }

        protected void Hide()
        {
            Visibility = Visibility.Hidden;
            OverlayOn.IsEnabled = _parentWasEnabled;

            if (ModalDialogHidden != null)
                ModalDialogHidden(this, new ModalDialogEventArgs() { DialogResult = this.DialogResult });
        }

        public bool DialogResult { get; set; }

        public event EventHandler<EventArgs> ModalDialogShown;
        public event EventHandler<ModalDialogEventArgs> ModalDialogHidden;

        public class ModalDialogEventArgs : EventArgs
        {
            public bool DialogResult { get; set; }
        }
    }
}
