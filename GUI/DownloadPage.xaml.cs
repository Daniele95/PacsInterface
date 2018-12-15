using System;
using System.Collections.Generic;
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
        List<ImageResponseQuery> images;

        internal void showQueryResults(QueryObject response)
        {

            ImageLevelQuery query = new ImageLevelQuery((SeriesResponseQuery)response);

            QueryRetrieve q = new QueryRetrieve();
            q.Event += showQueryResults2;
            q.OnConnectionClosed += imagesArrived;
            images = new List<ImageResponseQuery>();
            q.find(query, "Image");


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

        private void imagesArrived()
        {
            int imageToDownload = (int)(images.Count / 2.0f);
        }

        void showQueryResults2(QueryObject response)
        {
            images.Add((ImageResponseQuery)response);
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
        }
    }
}
