using Dicom;
using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;

namespace QueryRetrieveService
{
    public class GetSeriesData
    {
        public SeriesData seriesData;
        FileSystemWatcher watcher;
        Bitmap seriesThumb;
        static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        string imagePath = "";

        public BitmapImage getImage(SeriesResponseQuery series)
        {
            ImageLevelQuery query = new ImageLevelQuery(series);

            GetQueryResponsesList g = new GetQueryResponsesList();
            List<QueryObject> listImages = g.getResponsesList(query,"Image");

            int numImageToDownload = (int)(listImages.Count / 2.0f);
            ImageResponseQuery image = (ImageResponseQuery)listImages[numImageToDownload];

            
            watcher = new FileSystemWatcher();
            watcher.Path = Constants.imageThumbsFolder;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";
            watcher.Created += new FileSystemEventHandler(OnChanged);
            watcher.Changed += new FileSystemEventHandler(OnChanged);
            watcher.EnableRaisingEvents = true;


            QueryRetrieve q = new QueryRetrieve();
            q.move("IMAGEUSER",image, "Image");
            //WAIT LISTENER
            
            bool a = manualResetEvent.WaitOne(4000);
            if (a == false) MessageBox.Show("timeout");

            watcher.Dispose();

            return loadImageSource();
        }
        public QueryObject getSeriesData()
        {
            return seriesData;
        }
        public BitmapImage loadImageSource()
        {
            BitmapImage myBitmapImage = new BitmapImage();

            myBitmapImage.BeginInit();
            myBitmapImage.UriSource = new Uri(imagePath);

            myBitmapImage.DecodePixelWidth = 200;
            myBitmapImage.EndInit();
            return myBitmapImage;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {
            var dicomImage = new DicomImage(Path.Combine(watcher.Path, Constants.tempFileName));
            seriesThumb = dicomImage.RenderImage().AsClonedBitmap();
            File.Delete(Path.Combine(watcher.Path, Constants.tempFileName));

            // read data from downloaded image
            PropertyInfo[] p = typeof(SeriesData).GetProperties();
            seriesData = new SeriesData();
            foreach (PropertyInfo prop in p)
            {
                var tag = typeof(DicomTag).GetField(prop.Name).GetValue(null);
                string value = dicomImage.Dataset.GetValues<string>(DicomTag.Parse(tag.ToString()))[0];
                prop.SetValue(seriesData, value);
            }

            string patientName = dicomImage.Dataset.GetValues<string>(DicomTag.PatientName)[0].Replace(' ','_');
            string studyDescription = dicomImage.Dataset.GetValues<string>(DicomTag.StudyDescription)[0].Replace(' ', '_').Replace('/','-');
            string seriesDescription = dicomImage.Dataset.GetValues<string>(DicomTag.SeriesDescription)[0].Replace(' ', '_').Replace('/', '-');

            // save image under studies/thisSeries.thumb
            string thumbFolder = Path.Combine(GUILogic.readFromFile("databaseFolder"), patientName, studyDescription);
            Directory.CreateDirectory(thumbFolder);
            imagePath = Path.Combine(thumbFolder, seriesDescription) + ".jpg";
            seriesThumb.Save(imagePath);
            manualResetEvent.Set();
        }
    }
}
