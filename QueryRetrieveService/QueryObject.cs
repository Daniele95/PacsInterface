using Dicom;
using System;
using System.Windows;

namespace QueryRetrieveService
{
    public class SeriesData : QueryObject
    {
        public string SeriesDescription { get; set; } = "";
        public string StudyDate { get; set; } = "";
        public string Modality { get; set; } = "";
        public string SeriesInstanceUID { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
    }


    public class ImageResponseQuery : QueryObject
    {
        public string SOPInstanceUID { get; set; } = "";
        public string SeriesInstanceUID { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
    }
    public class ImageLevelQuery : QueryObject
    {
        public string StudyInstanceUID { get; set; } = "";
        public string SeriesInstanceUID { get; set; } = "";
        public string SOPInstanceUID { get; set; } = "";
        public string InstanceNumber { get; set; } = "";

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

        public SeriesResponseQuery()
        { }
        public SeriesResponseQuery(string studyInstanceUID,string seriesInstanceUID)
        {
            SeriesInstanceUID = seriesInstanceUID;
            StudyInstanceUID = studyInstanceUID;
        }
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
        public string ModalitiesInStudy { get; set; } = "";
        public string PatientBirthDate { get; set; } = "";
        public string StudyDescription { get; set; } = "";
    }
    public class StudyLevelQuery : QueryObject
    {
        public string PatientID { get; set; } = "";
        public string PatientName { get; set; } = "";
        public DicomDateRange StudyDate { get; set; } = new DicomDateRange();
        public string AccessionNumber { get; set; } = "";
        public string StudyID { get; set; } = "";
        public string ModalitiesInStudy { get; set; } = "";
        public string StudyInstanceUID { get; set; } = "";
        public string PatientBirthDate { get; set; } = "";
        public string StudyDescription { get; set; } = "";
    }

    public abstract class QueryObject
    {
    }




}
