using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
//using System.Web;
//using System.Web.UI;
//using System.Web.UI.WebControls;

/// <summary>
/// Summary description for ExtensionClass
/// </summary>
public static class ExtensionClass
{
    public static DataTable ToDataTable<T>(this IEnumerable<T> pItems)
    {
        DataTable dataTable = new DataTable(typeof(T).Name);
        //Get all the properties by using reflection   
        PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo prop in Props)
        {
            //Setting column names as Property names  
            dataTable.Columns.Add(prop.Name);
        }
        if (pItems != null)
        {
            foreach (T item in pItems)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {

                    values[i] = Props[i].GetValue(item, null);
                }
                dataTable.Rows.Add(values);
            }

        }

        return dataTable;

    }

}