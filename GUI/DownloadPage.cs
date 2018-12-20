using QueryRetrieveService;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
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
        public static void addMenuEntry(QueryObject response, BitmapImage image, DownloadPage downloadPageIstance)
        {
            GridView gridView = new GridView();
            downloadPageIstance.listView.View = gridView;

            List<dynamic> myItems = new List<dynamic>();
            dynamic myItem;
            IDictionary<string, object> myItemValues;


            // Populate the objects with dynamic columns
            //  for (var i = 0; i < allResponses.Count;i++)
            //   {
            myItem = new System.Dynamic.ExpandoObject();

            if (response != null)
            {
                PropertyInfo[] p = response.GetType().GetProperties();
                for (int j = 0; j < p.Length; j++)
                {
                    myItemValues = (IDictionary<string, object>)myItem;
                    myItemValues[p[j].Name] = p[j].GetValue(response);
                    //MessageBox.Show(p[j].GetValue(response).ToString());
                }
            }
            myItem.Icon = image;

            myItems.Add(myItem);

            //  }

            if (myItems.Count > 0)
            {

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
                            CellTemplate = downloadPageIstance.FindResource("iconTemplate") as DataTemplate
                        });
                    }
                    else
                    {
                        bool a = (column.Title == "StudyInstanceUID" || column.Title == "SeriesInstanceUID");
                        int testInt = a ? 0 : 1;

                        var binding = new Binding(column.SourceField);
                        gridView.Columns.Add(new GridViewColumn
                        {
                            Header = column.Title,
                            DisplayMemberBinding = binding,
                            Width = 100 * testInt
                        });
                    }


                }

                // Add all items to the list
                foreach (dynamic item in myItems)
                {
                    downloadPageIstance.listView.Items.Add(item);
                }
            }

        }

    }
}
