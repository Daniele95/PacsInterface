using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
                downloadPage.addMenuEntries(seriesData,images,this);       
                
            }), DispatcherPriority.ContextIdle);

        }



        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;

            if (item != null && item.IsSelected)
            {
                var obj = (IDictionary<string, object>)(item.Content);
                MessageBox.Show(obj["SeriesInstanceUID"].ToString());

                //now download

                QueryObject series = new SeriesResponseQuery(obj["StudyInstanceUID"].ToString(), obj["SeriesInstanceUID"].ToString());

                QueryRetrieve q = new QueryRetrieve();
                q.move(GUILogic.readFromFile("thisMachineMainAE"), series, "Series");


            }
        }

        internal void allSeriesArrived()
        {

        }
    }
}
