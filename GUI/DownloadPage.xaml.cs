using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
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

        internal void showQueryResults(List<QueryObject> allSeries)
        {
            // show data gained by image query
            List<BitmapImage> images = new List<BitmapImage>();
            List<QueryObject> seriesData = new List<QueryObject>();

            foreach (SeriesResponseQuery series in allSeries)
            {
                GetSeriesData getSeriesData = new GetSeriesData();
                images.Add(getSeriesData.getImage(series));
                seriesData.Add(getSeriesData.getSeriesData());
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                downloadPage.addMenuEntry(seriesData,images,this);
                
                
            }), DispatcherPriority.ContextIdle);

        }



        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;

            if (item != null && item.IsSelected)
            {
                var obj = (IDictionary<string, object>)(item.Content);
                MessageBox.Show(obj["SeriesInstanceUID"].ToString());
                //DOWNLOAD!!!!!

                QueryObject series = new SeriesResponseQuery(obj["StudyInstanceUID"].ToString(), obj["SeriesInstanceUID"].ToString());

                QueryRetrieve q = new QueryRetrieve();
                q.move("USER", series, "Series");


            }
        }

        internal void allSeriesArrived()
        {

        }
    }
}
