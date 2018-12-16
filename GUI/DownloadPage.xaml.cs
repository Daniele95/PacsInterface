using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using QueryRetrieveService;

namespace GUI
{
    public partial class DownloadPage : Page
    {
        public DownloadPage()
        {
            InitializeComponent();
        }

        internal void showQueryResults(List<QueryObject> allResponses, List<Bitmap> images)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                foreach (QueryObject response in allResponses) { 
                    // Add columns
                    var gridView = new GridView();
                    listView.View = gridView;

                    PropertyInfo[] properties = response.GetType().GetProperties();

                    foreach (PropertyInfo property in properties)
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = property.Name,
                            DisplayMemberBinding = new Binding(property.Name)
                        });


                    listView.Items.Add(response);
                }
            }), DispatcherPriority.ContextIdle);

        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
        }

        internal void allSeriesArrived()
        {

        }
    }
}
