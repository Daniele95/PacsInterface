using System;
using System.Collections.Generic;
using System.IO;
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
        public MainWindow mainWindow;
        public DownloadPage(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
            InitializeComponent();
        }

        internal void showQueryResults(List<QueryObject> allSeries)
        {
            // show data gained by image query
            List<BitmapImage> images = new List<BitmapImage>();
            List<QueryObject> seriesData = new List<QueryObject>();
            bool thereIsImage = false;

            //   foreach (SeriesResponseQuery series in allSeries)
            //    {
            SeriesResponseQuery series = (SeriesResponseQuery)allSeries[0];
            GetSeriesData getSeriesData = new GetSeriesData(mainWindow.guiLogic);
            BitmapImage imgSource = getSeriesData.downloadImage(series);

            if (imgSource != null)
            {
                var retrievedSeriesData = getSeriesData.getSeriesData();

                Dispatcher.BeginInvoke(new Action(() =>
                {
                    if (thereIsImage) downloadPage.addMenuEntry(retrievedSeriesData, imgSource, this);

                }), DispatcherPriority.ContextIdle);
                thereIsImage = true;
            }

            //   }


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

                GUILogic.clearImageThumbs();


                MessageBox.Show("now download: ");

                q.move(GUILogic.readFromFile("thisMachineAE"), series, "Series",mainWindow.guiLogic);


            }
        }

        internal void allSeriesArrived()
        {

        }
    }
}
