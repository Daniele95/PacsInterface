using System;
using System.Reflection;
using System.Windows;
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

        internal void showQueryResults(QueryObject response)
        {// image query 

/*          SeriesLevelQuery query = new SeriesLevelQuery(studyInstanceUID);

            QueryRetrieve q = new QueryRetrieve();
            q.Event += mainWindow.downloadPage.showQueryResults;
            mainWindow.frame.Navigate(mainWindow.downloadPage);
            mainWindow.downloadPage.listView.Items.Clear();
            q.find(query, "Series");
            */

            Dispatcher.BeginInvoke(new Action(() =>
            {

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
            }), DispatcherPriority.ContextIdle);
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
