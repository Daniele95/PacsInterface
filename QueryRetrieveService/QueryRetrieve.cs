using Dicom;
using Dicom.Network;
using System;
using System.Reflection;
using System.Windows;

namespace QueryRetrieveService
{
    public abstract class Publisher
    {
        public delegate void EventHandler(QueryObject s);
        public event EventHandler Event;

        public void RaiseEvent(QueryObject s)
        {
            Event(s);
        }
    }
    public class QueryRetrieve : Publisher
    {

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

            MethodInfo createQuery = typeof(DicomCFindRequest).GetMethod("Create"+level+"Query");


            PropertyInfo[] properties1 = query.GetType().GetProperties();

            object[] props = new object[properties1.Length];
            for (int i= 0; i < properties1.Length; i++)
            {
                props[i] = properties1[i].GetValue(query);
            }


            DicomCFindRequest cfind =(DicomCFindRequest) createQuery.Invoke(null, props);


            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) => {
                if (rp.HasDataset)
                {
                    var type = Type.GetType("QueryRetrieveService."+level+"ResponseQuery");
                    Console.WriteLine(type.ToString());
                    var response = (QueryObject)Activator.CreateInstance(type);

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
            client.Send("localhost", 11112, false, "USER", "MIOSERVER");

        }




    }
}
