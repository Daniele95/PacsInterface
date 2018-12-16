using GUI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace GUI
{
    public class Column
    {
        public string Title { get; set; }
        public string SourceField { get; set; }
    }

    public static class handleImages
    {

        //https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();

                return bitmapimage;
            }
        }


        public static void addImage(Bitmap asd, DownloadPage downloadPage)
        {

            GridView gridView = new GridView();
            downloadPage.listView.View = gridView;

            List<dynamic> myItems = new List<dynamic>();
            dynamic myItem;
            IDictionary<string, object> myItemValues;

            var image = BitmapToImageSource(asd);

            // Populate the objects with dynamic columns
            for (var i = 0; i < 100; i++)
            {
                myItem = new System.Dynamic.ExpandoObject();

                foreach (string column in new string[] { "Id", "Name", "Something" })
                {
                    myItemValues = (IDictionary<string, object>)myItem;
                    myItemValues[column] = "My value for " + column + " - " + i;
                }

                myItem.Icon = image;

                myItems.Add(myItem);
            }

            // Assuming that all objects have same columns - using first item to determine the columns
            List<Column> columns = new List<Column>();

            myItemValues = (IDictionary<string, object>)myItems[0];

            // Key is the column, value is the value
            foreach (var pair in myItemValues)
            {
                Column column = new Column();

                column.Title = pair.Key;
                column.SourceField = pair.Key;

                columns.Add(column);
            }

            // Add the column definitions to the list view
            gridView.Columns.Clear();

            foreach (var column in columns)
            {

                if (column.SourceField == "Icon")
                {
                    gridView.Columns.Add(new GridViewColumn
                    {
                        Header = column.Title,
                        CellTemplate = downloadPage.FindResource("iconTemplate") as DataTemplate
                    });
                }
                else
                {
                    var binding = new Binding(column.SourceField);
                    gridView.Columns.Add(new GridViewColumn { Header = column.Title, DisplayMemberBinding = binding });
                }


            }

            // Add all items to the list
            foreach (dynamic item in myItems)
            {
                downloadPage.listView.Items.Add(item);
            }


        }

    }
}
