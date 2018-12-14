using System;
using System.Windows;
using System.Windows.Controls;
using QueryRetrieveService;

namespace GUI
{
    public partial class DownloadPage : Page
    {
        public DownloadPage()
        {
            InitializeComponent();
        }

        internal void showQueryResults(QueryObject s)
        {
            MessageBox.Show("arrivato|!");
        }
    }
}
