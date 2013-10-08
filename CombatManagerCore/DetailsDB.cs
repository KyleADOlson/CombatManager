using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScottsUtils;
using System.IO;

#if !MONO
#else
using Mono.Data.Sqlite;
#endif

namespace CombatManager
{
    public static class DetailsDB
    {
        #if !MONO
                private static SQL_Lite sqlDetailsDB;
        #else
		
                private static SqliteConnection detailsDB;
        #endif

        public static string LoadDetails(string ID, string table, string field)
        {
            return LoadDetails(ID, table, new List<string>() { field })[field];
        }


        public static Dictionary<string, string> LoadDetails(string ID, string table, List<string> fields)
        {

            var dict = new Dictionary<string, string>();

            string selectFields="";
            bool first = true;
            foreach (string s in fields)
            {
                if (!first)
                {
                    selectFields += ", ";
                }
                first = false;
                selectFields += s;
            }


            string commandText = "Select " + selectFields + " from " + table + " where ID=?";
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

                var r = cm.ExecuteReader();

                r.Read()
                foreach (string s in fields)
                {
                    dict[s] = r.GetString(r.GetOrdinal(s));
                }
                r.Close();   
                
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
                    return dict;
                }

                foreach (string s in fields)
                {
                    dict[s] = ret.Rows[0][s];
                }

#endif
   
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }


            return dict;
        
        }

        #if ANDROID
        static string _DBVersion;
        public static void PrepareDetailDB(String version)
        {
            _DBVersion = version;
            if (!Directory.Exists(DBFolder))
            {
                Directory.CreateDirectory(DBFolder);
            }
            //if !db exists
            if (!File.Exists(DBFullFilename))
            {
                //open stream
                using (Stream io = CoreContext.Context.Assets.Open("Details.db"))
                {
                    using (FileStream fs = File.Open(DBFullFilename, FileMode.Create))
                    {
                        //copy db
                        byte[] bytes = new byte[20480];
                        int read = io.Read(bytes, 0, bytes.Length);
                        while (read > 0)
                        {
                            fs.Write(bytes, 0, read);
                            read = io.Read(bytes, 0, bytes.Length);
                        }
                        fs.Close();
                    }
                    io.Close();
                }


                //delete old dbs
                List<string> files = new List<string>(Directory.EnumerateFiles(DBFolder));

                foreach (string s in from x in files where x != DBFullFilename select x)
                {
                    File.Delete(s);
                }
            }


        }

        private static string DBFolder
        {
            get
            {
                string dir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                dir = Path.Combine(dir, "Database");
                return dir;
            }
        }

        private static string DBFullFilename
        {
            get
            {
                string filename = DBFilename;
                return Path.Combine(DBFolder, filename);
            }
        }

        private static string DBFilename
        {
            get
            {
                return "detail" + _DBVersion + ".db";

            }
        }


        #endif
    }
}
