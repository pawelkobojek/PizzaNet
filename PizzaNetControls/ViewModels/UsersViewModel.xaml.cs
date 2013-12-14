using PizzaNetControls.Views;
using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using PizzaNetCommon.DTOs;
using System.Reflection;
using System.Globalization;

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for UsersViewModel.xaml
    /// </summary>
    public partial class UsersViewModel : UserControl, INotifyPropertyChanged
    {
        #region view model init and so on

        public UsersViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += UsersViewModel_Loaded;
        }

        bool initialized = false;

        void UsersViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.UsersView = new UsersView(Worker);
                initialized = true;
            }
        }

        private UsersView _vo;
        public UsersView UsersView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("UsersView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(UsersViewModel), new UIPropertyMetadata());

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        private void ButtonAddUser_Click(object sender, RoutedEventArgs e)
        {
            UsersView.AddUser();
        }

        private void ButtonRemoveUser_Click(object sender, RoutedEventArgs e)
        {
            if (listUsers.SelectedIndex < 0)
                return;

            UsersView.RemoveUser(listUsers.SelectedIndex);
        }

        private void ButtonSaveChanges_Click(object sender, RoutedEventArgs e)
        {
            UsersView.SaveChanges();
        }

        public void GotFocusAction()
        {
            UsersView.RefreshUsers();
        }

        public bool LostFocusAction()
        {
            return true;
        }

        private void TextBoxStockDetails_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (listUsers.SelectedIndex < 0) return;
            //MODIFIED _vo.UpdateStockItem(listStock.SelectedIndex);
            UsersView.Modified = true;
        }

        private void TextBoxStockDetails_KeyDown(object sender, KeyEventArgs e)
        {
            UsersView.Modified = true;
            var txtb = sender as TextBox;
            if (txtb == null) return;
            if (listUsers.SelectedIndex < 0) return;
            UserDTO rc = UsersView.UsersCollection[listUsers.SelectedIndex];
            if (e.Key == Key.Return)
            {
                BindingExpression exp = txtb.GetBindingExpression(TextBox.TextProperty);
                UserDTO user = exp.ResolvedSource as UserDTO;
                if (user == null) return;
                PropertyInfo pi = user.GetType().GetProperty(exp.ResolvedSourcePropertyName);
                string target = pi.GetValue(user).ToString();
                if (target != txtb.Text)
                    exp.UpdateSource();
                else exp.ValidateWithoutUpdate();
            }
        }

        private void CheckLastBinding()
        {
            if (listUsers.SelectedIndex < 0) return;
            UserDTO rc = UsersView.UsersCollection[listUsers.SelectedIndex];
            NumberFormatInfo nfi = new CultureInfo("en-US", true).NumberFormat;
            //TODO Nie wiem co
            //UsersView.Modified |=
            //    tbId.Text != rc.IngredientID.ToString() ||
            //    tbName.Text != rc.Name.ToString() ||
            //    tbNW.Text != rc.NormalWeight.ToString() ||
            //    tbEW.Text != rc.ExtraWeight.ToString() ||
            //    tbPU.Text != rc.PricePerUnit.ToString("0.########", nfi);
        }
    }
}
