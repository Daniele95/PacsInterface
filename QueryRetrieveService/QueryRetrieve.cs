using Dicom;
using Dicom.Network;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;

namespace QueryRetrieveService
{
    public abstract class Publisher
    {
        public delegate void EventHandler(QueryObject s);
        public event EventHandler OnDatasetArrived;

        public void RaiseEvent(QueryObject s)
        {
            OnDatasetArrived(s);
        }

        public delegate void ConnectionClosed(List<QueryObject> l);
        public event ConnectionClosed OnConnectionClosed;

        public void RaiseConnectionClosed(List<QueryObject> l)
        {
            OnConnectionClosed(l);
        }

    }

    public class QueryRetrieve : Publisher
    {

        List<QueryObject> queryResponses = new List<QueryObject>();

        public void move ()
        {
            var cmove = new DicomCMoveRequest("USER", "1.3.6.1.4.1.5962.1.1.0.0.0.1196527414.5534.0.1");

            var client = new DicomClient();
            client.AddRequest(cmove);
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");
        }
        
        public void find(QueryObject query, string level)
        {
            if (level != "Study" && level != "Series" && level != "Image")
            { MessageBox.Show("incorrect level"); return; }

            DicomQueryRetrieveLevel queryLevel = (DicomQueryRetrieveLevel) Enum.Parse(typeof(DicomQueryRetrieveLevel), level);
            DicomCFindRequest cfind = new DicomCFindRequest(queryLevel);
            Type tipo = query.GetType();
            PropertyInfo[] properties1 = tipo.GetProperties();

            foreach (PropertyInfo property in properties1)
            {
                var tag = typeof(DicomTag).GetField(property.Name).GetValue(null);
                DicomTag theTag = (DicomTag.Parse(tag.ToString()));

                var variabile = property.GetValue(query);
                if (variabile.GetType().ToString() == "System.String")
                {
                    String a = (String)variabile;
                    cfind.Dataset.Add(theTag, a);
                }
                if (variabile.GetType().ToString() == "Dicom.DicomDateRange")
                {
                    DicomDateRange a = (DicomDateRange)variabile;
                    cfind.Dataset.Add(theTag, a);
                }
            }

            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) => {

                if (rp.HasDataset)
                {
                    var type = Type.GetType("QueryRetrieveService."+level+"ResponseQuery");
                    var response = (QueryObject)Activator.CreateInstance(type);
                    queryResponses.Add(response);

                    PropertyInfo[] properties = response.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        var tag = typeof(DicomTag).GetField(property.Name).GetValue(null);
                        property.SetValue(response, rp.Dataset.GetValues<string>(DicomTag.Parse(tag.ToString()))[0]);
                    }

                    RaiseEvent(response);
                }
            };


            var client = new DicomClient();
            client.AddRequest(cfind);
            client.AssociationReleased += (sender,e)=> {
                RaiseConnectionClosed(queryResponses);
            };
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");

        }
        
    }
}
