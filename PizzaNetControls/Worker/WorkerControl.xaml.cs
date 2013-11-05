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

namespace PizzaNetControls.Worker
{
    /// <summary>
    /// Interaction logic for WorkerControl.xaml
    /// </summary>
    public partial class WorkerControl : UserControl
    {
        private Queue<WorkerTask> tasks;
        private BackgroundWorker worker;

        public Control Lock { get; set; }

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
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (tasks.Count == 0)
            {
                this.spinner.Visibility = System.Windows.Visibility.Hidden;
                if (this.Lock != null)
                    this.Lock.IsEnabled = true;
            }
            else nextTask();
        }

        void nextTask()
        {
            this.spinner.Visibility = System.Windows.Visibility.Visible;
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
    }
}
