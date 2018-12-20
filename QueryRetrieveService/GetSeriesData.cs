using Dicom;
using Dicom.Imaging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
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
        GUILogic guiLogic;

        public GetSeriesData(GUILogic guiLogic)
        {
            this.guiLogic = guiLogic;
        }

        static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        bool isImage = true;

        public BitmapImage downloadImage(SeriesResponseQuery series)
        {
            clearFolder(Constants.imageThumbsFolder);

            ImageLevelQuery query = new ImageLevelQuery(series);

            GetQueryResponsesList g = new GetQueryResponsesList();
            // IMAGE LEVEL QUERY
            List<QueryObject> listImages = g.getResponsesList(query, "Image");

            if (listImages.Count > 0)
            {
                int numImageToDownload = (int)(listImages.Count / 2.0f);
                ImageResponseQuery image = (ImageResponseQuery)listImages[numImageToDownload];

                watch(Constants.imageThumbsFolder);

                QueryRetrieve q = new QueryRetrieve();
                String path = Path.Combine(Constants.imageThumbsFolder, "singleImage.txt");
                File.WriteAllText(path, "gotImage");

                //MOVE IMAGE
                q.move(GUILogic.readFromFile("thisMachineAE"), image, "Image", guiLogic);
                bool a = manualResetEvent.WaitOne(500);
                // now wait

                watcher.Dispose();

                if (isImage) return loadImageSource();
                else return null;
            }
            else return new BitmapImage();
        }
        public QueryObject getSeriesData()
        {
            return seriesData;
        }
        public BitmapImage loadImageSource()
        {
            using (MemoryStream memory = new MemoryStream())
            {
                BitmapImage myBitmapImage = new BitmapImage();
                try
                {
                    seriesThumb.Save(memory, ImageFormat.Png);
                    myBitmapImage.BeginInit();
                    //   myBitmapImage.UriSource = new Uri(Path.GetFullPath(imageJpgPath));
                    //       myBitmapImage.DecodePixelWidth = 200;

                    myBitmapImage.StreamSource = memory;
                    myBitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    myBitmapImage.EndInit();
                }
                catch (Exception) { }
                return myBitmapImage;
            }
        }

        private void onDicomArrived(object sender, FileSystemEventArgs e)
        {
            try
            {
                var dicomImage = new DicomImage(Path.Combine(watcher.Path, Constants.tempFileName));
                extractData(dicomImage);
                try
                {
                    seriesThumb = dicomImage.RenderImage().AsClonedBitmap();
                    manualResetEvent.Set();
                }
                catch (Exception exc)
                {
                    isImage = false;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine("got this exception: " + exc.Message);
            }
        }

        public void extractData(DicomImage dicomImage)
        {
            // read data from downloaded image
            PropertyInfo[] p = typeof(SeriesData).GetProperties();
            seriesData = new SeriesData();
            foreach (PropertyInfo prop in p)
            {
                var tag = typeof(DicomTag).GetField(prop.Name).GetValue(null);

                string value = "";
                dicomImage.Dataset.TryGetSingleValue(DicomTag.Parse(tag.ToString()), out value);
                prop.SetValue(seriesData, value);
            }

            //    string patientName = dicomImage.Dataset.GetValues<string>(DicomTag.PatientName)[0].Replace(' ', '_');
            //    string studyDescription = dicomImage.Dataset.GetValues<string>(DicomTag.StudyDescription)[0].Replace(' ', '_').Replace('/', '-');
            //   string seriesDescription = dicomImage.Dataset.GetValues<string>(DicomTag.SeriesDescription)[0].Replace(' ', '_').Replace('/', '-');

            // save image under studies/thisSeries.thumb
            //   string thumbFolder = Path.Combine(GUILogic.readFromFile("databaseFolder"), patientName, studyDescription);
            //    Directory.CreateDirectory(thumbFolder);
            // manualResetEvent.Set();
        }

        public void clearFolder(string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.GetFiles())
            {
                while (IsFileLocked(file)) { }
                file.Delete();
            }
        }

        /// <summary>
        /// Code by ChrisW -> http://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-file-is-in-use
        /// </summary>
        protected virtual bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        void watch(string folder)
        {
            watcher = new FileSystemWatcher();
            watcher.Path = folder;
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Filter = "*.*";

            watcher.Created += new FileSystemEventHandler(onDicomArrived);
            watcher.Changed += new FileSystemEventHandler(onDicomArrived);

            watcher.EnableRaisingEvents = true;
        }
    }
}
