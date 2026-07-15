using System.Data;
using System.Reflection;

namespace WhatsappComercial.Modelos
{
    public class Convertir
    {
        public static DataTable LINQToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();

            // column names 
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;

            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others   will follow 
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }

                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }

                DataRow dr = dtReturn.NewRow();

                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }

                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        public static List<T> ConvertirDataTableALista<T>(DataTable table) where T : new()
        {
            var lista = new List<T>();

            foreach (DataRow row in table.Rows)
            {
                T obj = new T();

                foreach (DataColumn col in table.Columns)
                {
                    PropertyInfo prop = typeof(T).GetProperty(col.ColumnName);

                    if (prop != null && prop.CanWrite)
                    {
                        object valor = row[col.ColumnName];

                        if (valor != DBNull.Value)
                        {
                            prop.SetValue(obj, Convert.ChangeType(valor, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                        }
                    }
                }

                lista.Add(obj);
            }

            return lista;
        }


        public static T ConvertirDataTableAObjeto<T>(DataTable table) where T : new()
        {
            T obj = new T();
            DataRow dataRow = table.Rows[0];

            foreach (DataColumn col in table.Columns)
            {
                PropertyInfo prop = typeof(T).GetProperty(col.ColumnName);

                if (prop != null && prop.CanWrite)
                {
                    object valor = dataRow[col.ColumnName];

                    if (valor != DBNull.Value)
                    {
                        prop.SetValue(obj, Convert.ChangeType(valor, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType));
                    }
                }
            }

            return obj;
        }
    }
}
