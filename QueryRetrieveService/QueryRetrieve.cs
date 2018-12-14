using Dicom;
using Dicom.Network;
using System;
using System.Reflection;
using System.Windows;

namespace QueryRetrieveService
{
    public abstract class Publisher
    {
        public delegate void EventHandler(StudyResponseQuery s);
        public event EventHandler Event;

        public void RaiseEvent(StudyResponseQuery s)
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

        public void doStudyQuery(StudyLevelQuery query)
        {
            MethodInfo createStudyQuery = typeof(DicomCFindRequest).GetMethod("CreateStudyQuery");

            PropertyInfo[] properties1 = query.GetType().GetProperties();

            object[] props = new object[properties1.Length] ;
            for (int i= 0; i < properties1.Length; i++) 
                props[i] = properties1[i].GetValue(query);


            /* without reflection would be like this:
            var cfind = DicomCFindRequest.CreateStudyQuery(
                patientName: query.PatientName,
                studyDateTime: new DicomDateRange(query.StudyDateMin, query.StudyDateMax),
                modalitiesInStudy: query.Modality
            );
            */

            DicomCFindRequest cfind =(DicomCFindRequest) createStudyQuery.Invoke(null, props);


            cfind.OnResponseReceived = (DicomCFindRequest rq, DicomCFindResponse rp) => {
                if (rp.HasDataset)
                {
                    var response = new StudyResponseQuery();

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
