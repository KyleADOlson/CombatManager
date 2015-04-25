using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScottsUtils;
using System.IO;
using System.Reflection;

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

        public static void  OpenDB()
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
#else
            if (sqlDetailsDB == null)
            {
                sqlDetailsDB = new SQL_Lite();
                sqlDetailsDB.SkipHeaderRow = true;
                sqlDetailsDB.Open(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "details.db"));
            }
#endif

         
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
                OpenDB();
#if MONO



                var cm = detailsDB.CreateCommand();
                cm.CommandText = commandText;               
                var p = cm.CreateParameter();
                p.Value = ID;
                cm.Parameters.Add(p);

                var r = cm.ExecuteReader();

                r.Read();
                foreach (string s in fields)
                {
                    object obj = r.GetValue(r.GetOrdinal(s));
                    if (obj != null)
                    {
                        dict[s] = obj.ToString();
                    }
                    else
                    {
                        dict[s] = null;
                    }
                }
                r.Close();   
                
#else

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

        public static List<int> findLikeId(string table, string field, string value)
        {

            string commandText = "Select id from " + table + " where " + field + " like ?";

            return findIds(table, commandText, new List<String>(){value});

        }


        public static List<int> findIds(string table, string commandText, List<string> parameters)
        {
            List<int> ids = new List<int>();

            try
            {
                OpenDB();
#if MONO



                var cm = detailsDB.CreateCommand();
                cm.CommandText = commandText;
                foreach (String s in parameters)
                {
                    var p = cm.CreateParameter();
                    p.Value = s;
                    cm.Parameters.Add(p);
                }

                var r = cm.ExecuteReader();

                while (r.Read())
                {
                    object obj = r.GetValue(r.GetOrdinal("id"));
                    if (obj != null)
                    {
                        ids.Add(int.Parse((String)obj));
                    }
                }
                r.Close();   
                
#else

                RowsRet ret = null;

                Object [] ob = new Object[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    ob[i] = parameters[i];
                }

                ret = sqlDetailsDB.ExecuteCommand(commandText,ob);



                for (int i = 0; i < ret.Rows.Count; i++)
                {
                    Row row = ret.Rows[i];
                    ids.Add(int.Parse(row["id"]));
                }

#endif

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }

            return ids;


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
