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

namespace PizzaNetControls.Workers
{
    /// <summary>
    /// Interaction logic for WorkerControl.xaml
    /// </summary>
    public partial class WorkerControl : UserControl, IWorker
    {
        private Queue<WorkerTask> tasks;
        private BackgroundWorker worker;

        public FrameworkElement Lock { get; set; }

        public event EventHandler<EventArgs> RefreshButtonClicked;

        public bool IsRefreshButtonEnabled
        {
            get { return (bool)GetValue(IsRefreshButtonEnabledProperty); }
            set { SetValue(IsRefreshButtonEnabledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsRefreshButtonEnabled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsRefreshButtonEnabledProperty =
            DependencyProperty.Register("IsRefreshButtonEnabled", typeof(bool), typeof(WorkerControl),
            new PropertyMetadata(true, new PropertyChangedCallback(IsRefreshButtonEnabledPropertyChanged)));

        private static void IsRefreshButtonEnabledPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = obj as WorkerControl;
            if (!control.worker.IsBusy)
            {
                if ((e.NewValue as bool?).Value)
                {
                    control.button.Visibility = Visibility.Visible;
                    control.button.IsEnabled = true;
                }
                else
                {
                    control.button.Visibility = Visibility.Hidden;
                    control.button.IsEnabled = false;
                }
            }
        }

        public WorkerControl()
        {
            InitializeComponent();
            this.tasks = new Queue<WorkerTask>();

            this.worker = new BackgroundWorker();
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted;
        }

        public void EnqueueTask(WorkerTask task)
        {
            tasks.Enqueue(task);
            if (!worker.IsBusy)
            {
                nextTask();
            }
            else
            {
                if (AllWorkDone != null)
                    AllWorkDone(this, new EventArgs());
            }
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (tasks.Count == 0)
            {
                this.spinner.Visibility = System.Windows.Visibility.Hidden;
                if (IsRefreshButtonEnabled)
                {
                    this.button.Visibility = System.Windows.Visibility.Visible;
                    this.button.IsEnabled = true;
                }
                if (this.Lock != null)
                    this.Lock.IsEnabled = true;
                if (AllWorkDone != null)
                    AllWorkDone(this, new EventArgs());
            }
            else nextTask();
        }

        void nextTask()
        {
            this.spinner.Visibility = System.Windows.Visibility.Visible;
            this.button.Visibility = System.Windows.Visibility.Hidden;
            this.button.IsEnabled = false;
            if (this.Lock != null)
                this.Lock.IsEnabled = false;
            WorkerTask t = tasks.Dequeue();

            DoWorkEventHandler doWorkHandler = (s, e) =>
            {
                t.Result = t.Task(t.Arguments);
            };
            RunWorkerCompletedEventHandler workCompletedHandler = (s, e) =>
            {
                worker.DoWork -= doWorkHandler;
                t.Finish(this, new WorkFinishedEventArgs(t.Result, t.Arguments));
            };

            worker.DoWork += doWorkHandler;
            worker.RunWorkerCompleted += workCompletedHandler;
            worker.RunWorkerCompleted += (s, e) => { worker.RunWorkerCompleted -= workCompletedHandler; };
            worker.RunWorkerAsync();
        }

        public event EventHandler<EventArgs> AllWorkDone;

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (RefreshButtonClicked != null)
                RefreshButtonClicked(this, new EventArgs());
        }
    }
}
