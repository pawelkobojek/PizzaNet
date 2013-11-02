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
using System.Windows.Shapes;

namespace PizzaNetControls.Worker
{
    /// <summary>
    /// Interaction logic for WorkerWindow.xaml
    /// </summary>
    public partial class WorkerWindow : Window
    {
        public delegate object RunnableTask(object[] args);
        public delegate void WorkFinishedHandler(object sender, WorkFinishedEventArgs e);

        public event WorkFinishedHandler WorkFinished;

        private RunnableTask task;
        private object[] args;
        public object Result { get; set; }

        private BackgroundWorker worker;

        private WorkerWindow(RunnableTask task, params object[] args)
        {
            InitializeComponent();
            this.task = task;
            this.args = args;

            this.worker = new BackgroundWorker();
            this.worker.RunWorkerCompleted += worker_RunWorkerCompleted;
            this.WorkFinished += WorkerWindow_WorkFinished;
        }

        public WorkerWindow(RunnableTask task, WorkFinishedHandler workFinishedHandler, params object[] args) : this(task, args)
        {
            if (workFinishedHandler!=null)
                this.WorkFinished += workFinishedHandler;
        }

        public WorkerWindow(Window owner, RunnableTask task, WorkFinishedHandler workFinishedHandler, params object[] args)
            : this(task, workFinishedHandler, args)
        {
            if (owner != this)
                this.Owner = owner;
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (WorkFinished != null)
                WorkFinished(sender, new WorkFinishedEventArgs(this.Result, args));
        }

        private void WorkerWindow_WorkFinished(object sender, WorkFinishedEventArgs e)
        {
            this.Close();
        }

        protected void Window_Loaded(object sender, RoutedEventArgs e)
        {   
            if (task != null)
            {
                worker.DoWork += (obj, a) =>
                {
                    this.Result = this.task(args);
                };
                worker.RunWorkerAsync();
            }
            else this.Close();
        }

        public void Finish(object result)
        {
            if (WorkFinished!=null)
                WorkFinished(this, new WorkFinishedEventArgs(result, args));
        }
    }
}
