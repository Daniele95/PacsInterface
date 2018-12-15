using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;

namespace QueryRetrieveService
{
    public class GetQueryResponsesList : Publisher
    {

        static ManualResetEvent manualResetEvent = new ManualResetEvent(false);
        List<QueryObject> queryResponses = new List<QueryObject>();

        public List<QueryObject> getResponsesList(QueryObject query, string level)
        {
            QueryRetrieve retrieveSeries = new QueryRetrieve();

            retrieveSeries.OnDatasetArrived += gotASeries;
            retrieveSeries.OnConnectionClosed += (seriesList) => { gotAllSeries(); };

            retrieveSeries.find(query, level);

            manualResetEvent.Reset();
            bool a = manualResetEvent.WaitOne(1000);
            if (a == false) MessageBox.Show("timeout");

            return queryResponses;
        }

        private void gotASeries(QueryObject s)
        {
            queryResponses.Add(s);
        }
        private void gotAllSeries()
        {
            Console.WriteLine("se non aspetto qui va in timeout!!");
            manualResetEvent.Set();
        }
    }
}
