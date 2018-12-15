using Dicom;
using System;
using System.Windows;

namespace QueryRetrieveService
{

    public class ImageResponseQuery : QueryObject
    {
        public string SOPInstanceUID { get; set; } = "";
    }
    public class ImageLevelQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string SeriesInstanceUID { get; set; } = "";
        public string SOPInstanceUID { get; set; } = "";
        public string InstanceNumber { get; set; } = "1";

        public ImageLevelQuery(SeriesResponseQuery response)
        {
            SeriesInstanceUID = response.SeriesInstanceUID;
            StudyInstanceUID = response.StudyInstanceUID;
        }
    }
    public class SeriesResponseQuery : QueryObject
    {
        public string SeriesInstanceUID { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
    }
    public class SeriesLevelQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string Modality { get; set; } = "";
        public string SeriesInstanceUID { get; set; } = "";

        public SeriesLevelQuery(StudyResponseQuery response)
        {
            StudyInstanceUID = response.StudyInstanceUID;
        }
    }
    public class StudyResponseQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string PatientID { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string StudyDate { get; set; } = "";
      //  public string Modality { get; set; } = "";
    }
    public class StudyLevelQuery : QueryObject
    {
        public string PatientID { get; set; } = "";
        public string PatientName { get; set; } = "";
        public DicomDateRange StudyDate { get; set; } = new DicomDateRange();
        public string AccessionNumber { get; set; } = "";
        public string StudyID { get; set; } = "";
        public string Modality { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
    }

    public abstract class QueryObject
    {
    }




}
