using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QueryRetrieveService
{

    public class StudyResponseQuery : QueryObject
    {
        //  public string QueryRetrieveLevel { get; set; } = "STUDY";
        public string PatientName { get; set; } = "";
        public string StudyDate { get; set; } = "";
        public string Modality { get; set; } = "";
    }
        public class StudyLevelQuery : QueryObject
    {
      //  public string QueryRetrieveLevel { get; set; } = "STUDY";
        public string PatientName { get; set; } = "";
        public string Modality { get; set; } = "";
        public DateTime StudyDateMin { get; set; } = DateTime.Today.AddYears(-100);
        public DateTime StudyDateMax { get; set; } = DateTime.Today;
    //  public string PatientBirthDate { get; set; } = "";

    //    public string PatientID { get; set; } = "";
   //     public string StudyInstanceUID { get; set; } = "";
   //     public string AccessionNumber { get; set; } = "";
     //   public string StudyDescription { get; set; } = "";
    }

    public abstract class QueryObject
    {/*
        public List<string> getKeys()
        {
            Dictionary<string, string> queryData = new Dictionary<string, string>();

            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                queryData.Add(property.Name, property.GetValue(this).ToString());
            }
            return new List<string>(queryData.Keys);
        }

        public string GetField(string tag)
        {
            //  try { ret = queryData[tag]; }
            // catch (Exception exc) { MessageBox.Show("the key "+ tag + " was not present in the database \n" + exc.StackTrace); }
            return GetType().GetProperty(tag).GetValue(this).ToString();
        }

        public void SetField(string tag, string value)
        {
            PropertyInfo prop = this.GetType().GetProperty(tag, BindingFlags.Public | BindingFlags.Instance);

            if (null != prop && prop.CanWrite)
                prop.SetValue(this, value);
        }
        */
    }
}
