using Dicom.Imaging;
using Dicom.Network;
using Listener;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;

namespace QueryRetrieveService
{
    public class GetSeriesData
    {

       

        public static Bitmap getImage(SeriesResponseQuery series)
        {
            ImageLevelQuery query = new ImageLevelQuery(series);

            GetQueryResponsesList g = new GetQueryResponsesList();
            List<QueryObject> listImages = g.getResponsesList(query,"Image");

            int numImageToDownload = (int)(listImages.Count / 2.0f);
            ImageResponseQuery image = (ImageResponseQuery)listImages[numImageToDownload];

            
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = @"C:\Users\daniele\Documents\Visual Studio 2017\Projects\PacsInterface\GUI\bin\Debug\images";
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;


            QueryRetrieve q = new QueryRetrieve();
            q.move("IMAGEUSER",image, "Image");


            var dicomImage = new DicomImage(Path.Combine(watcher.Path,"file.dcm"));
            Bitmap asd = dicomImage.RenderImage().AsClonedBitmap();

            return asd;
        }

        private static void OnChanged(object sender, FileSystemEventArgs e)
        {
            MessageBox.Show("file rated");
        }
    }
}
