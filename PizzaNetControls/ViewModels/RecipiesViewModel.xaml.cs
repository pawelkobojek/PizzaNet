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

namespace PizzaNetControls.ViewModels
{
    /// <summary>
    /// Interaction logic for RecipiesViewModel.xaml
    /// </summary>
    public partial class RecipiesViewModel : UserControl, INotifyPropertyChanged
    {
        public RecipiesViewModel()
        {
            this.DataContext = this;
            InitializeComponent();
            this.Loaded += RecipiesViewModel_Loaded;
        }

        bool initialized = false;

        void RecipiesViewModel_Loaded(object sender, RoutedEventArgs e)
        {
            if (!initialized)
            {
                this.RecipiesView = new RecipiesView(Worker);
                initialized = true;
            }
        }

        private RecipiesView _vo;
        public RecipiesView RecipiesView
        {
            get { return _vo; }
            set { _vo = value; NotifyPropertyChanged("RecipiesView"); }
        }

        public IWorker Worker
        {
            get { return (IWorker)GetValue(WorkerProperty); }
            set { SetValue(WorkerProperty, value); }
        }

        public static readonly DependencyProperty WorkerProperty =
            DependencyProperty.Register("Worker", typeof(IWorker), typeof(RecipiesViewModel), new UIPropertyMetadata());


        private void RecipesContainer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.OriginalSource != RecipesContainer) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            _vo.ChangeRecipeSelection(RecipesContainer.SelectedIndex);
        }

        private void ButtonRemoveRecipe_Click(object sender, RoutedEventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            _vo.RemoveRecipe(RecipesContainer.SelectedIndex);
        }

        private void ButtonAddRecipe_Click(object sender, RoutedEventArgs e)
        {
            _vo.AddRecipe();
        }

        private void TextBoxRecipeName_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            if (RecipesContainer.SelectedIndex < 0) return;
            _vo.UpdateRecipe(RecipesContainer.SelectedIndex);
        }

        private void TextBoxRecipeName_KeyDown(object sender, KeyEventArgs e)
        {
            var txtb = sender as TextBox;
            if (txtb == null) return;
            if (RecipesContainer.SelectedIndex < 0) return;
            RecipeControl rc = (RecipeControl)RecipesContainer.SelectedItem;
            if (e.Key == Key.Return)
            {
                if (rc.Recipe.Name != txtb.Text)
                    txtb.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        internal void NotifyPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
