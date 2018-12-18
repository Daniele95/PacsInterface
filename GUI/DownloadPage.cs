using GUI;
using QueryRetrieveService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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

    public static class downloadPage
    {

        //https://stackoverflow.com/questions/22499407/how-to-display-a-bitmap-in-a-wpf-image

        public static BitmapImage BitmapToImageSource(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {

                BitmapImage bitmapimage = new BitmapImage();
                try { 
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                } catch (Exception e) { MessageBox.Show("cannot read image.."); }

                return bitmapimage;
            }
        }


        public static void addMenuEntry(List<QueryObject> allResponses, List<BitmapImage> asd, DownloadPage downloadPage)
        {

            GridView gridView = new GridView();
            downloadPage.listView.View = gridView;

            List<dynamic> myItems = new List<dynamic>();
            dynamic myItem;
            IDictionary<string, object> myItemValues;


            // Populate the objects with dynamic columns
            for (var i = 0; i < allResponses.Count;i++)
            {
                var image = asd[i];
                myItem = new System.Dynamic.ExpandoObject();

                PropertyInfo[] p = allResponses[i].GetType().GetProperties();
                for (int j=0; j<p.Length; j++)
                {
                    myItemValues = (IDictionary<string, object>)myItem;
                    myItemValues[p[j].Name] = p[j].GetValue(allResponses[i]);
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
                    bool a = (column.Title == "StudyInstanceUID" || column.Title == "SeriesInstanceUID");
                    int testInt = a ? 0 : 1; 

                     var binding = new Binding(column.SourceField);
                    gridView.Columns.Add(new GridViewColumn {
                        Header = column.Title,
                        DisplayMemberBinding = binding,
                        Width = 100 * testInt
                    });
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
