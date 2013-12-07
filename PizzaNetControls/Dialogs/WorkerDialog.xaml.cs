using PizzaNetControls.Workers;
using System;
using System.Collections.Generic;
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

namespace PizzaNetControls.Dialogs
{
    /// <summary>
    /// Interaction logic for WorkerDialog.xaml
    /// </summary>
    public partial class WorkerDialog : ModalDialog, IWorker
    {
        public WorkerDialog()
        {
            InitializeComponent();
        }

        public void EnqueueTask(WorkerTask task)
        {
            this.Show();
            worker.EnqueueTask(task);
            worker.AllWorkDone += worker_AllWorkDone;
        }

        void worker_AllWorkDone(object sender, EventArgs e)
        {
            if (this.AllWorkDone != null)
                AllWorkDone(this, e);
            this.Hide();
        }

        public event EventHandler<EventArgs> AllWorkDone;
    }
}
