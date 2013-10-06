using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScottsUtils;

#if !MONO
#else
using Mono.Data.Sqlite;
#endif

namespace CombatManager
{
    static class DetailsDB
    {
        #if !MONO
                private static SQL_Lite sqlDetailsDB;
        #else
		
                private static SqliteConnection detailsDB;
        #endif

        public static string LoadDetails(string ID, string table, string field)
        {
            string details = "";
            string commandText = "Select " + field + " from " + table +" where ID=?";
            try
            {
#if MONO
                if (detailsDB == null)
                {
#if ANDROID
                    detailsDB = new SqliteConnection("DbLinqProvider=Sqlite;Data Source=" + DBFullFilename);
#else
                    
                    detailsDB = new SqliteConnection("DbLinqProvider=Sqlite;Data Source=Details.db");
#endif
                    detailsDB.Open();

                }

                var cm = detailsDB.CreateCommand();
                cm.CommandText = commandText;
                var p = cm.CreateParameter();
                p.Value = ID;
                cm.Parameters.Add(p);

                details = (string)cm.ExecuteScalar();
#else
                if (sqlDetailsDB == null)
                {
                    sqlDetailsDB = new SQL_Lite();
                    sqlDetailsDB.SkipHeaderRow = true;
                    sqlDetailsDB.Open("details.db");
                }

                RowsRet ret = null;

                ret = sqlDetailsDB.ExecuteCommand(commandText, new object[] { ID });

                if (ret == null || ret.Count() == 0)
                {
                    return "";
                }

                details = ret.Rows[0][field];

#endif
   
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            return details;
        
        }
    }
}
