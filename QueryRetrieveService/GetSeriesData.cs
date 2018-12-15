using System.Drawing;
using System.Windows;

namespace QueryRetrieveService
{
    public class GetSeriesData
    {
        public static Bitmap getImage(SeriesResponseQuery series)
        {
            ImageLevelQuery query = new ImageLevelQuery(series);

            GetQueryResponsesList g = new GetQueryResponsesList();
            var  listImages = g.getResponsesList(query,"Image");
            MessageBox.Show(listImages.Count.ToString());

          //  var image = new DicomImage(@"test.dcm");
         //   image.RenderImage().AsBitmap().Save(@"test.jpg");

            return new Bitmap(@"C:\Users\daniele\Pictures\$01_image.jpg");
        }
    }
}
