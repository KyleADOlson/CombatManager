/*
 *  DBLoader.cs
 *
 *  Copyright (C) 2010-2012 Kyle Olson, kyle@kyleolson.com
 *
 *  This program is free software; you can redistribute it and/or
 *  modify it under the terms of the GNU General Public License
 *  as published by the Free Software Foundation; either version 2
 *  of the License, or (at your option) any later version.
 *
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 * 
 *  You should have received a copy of the GNU General Public License
 *  along with this program; if not, write to the Free Software
 *  Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 *
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml.Serialization;
using ScottsUtils;
using System.IO;
using System.Collections;
#if MONO
using System.Data;
using Mono.Data.Sqlite;
#endif

namespace CombatManager
{
	
#if MONO
	public static class ExtSQL
	{
		public static RowsRet ExecuteCommand(this SqliteConnection sql, string command)
		{
            System.Diagnostics.Debug.WriteLine(command);
			SqliteCommand cmd =  sql.CreateCommand();
			cmd.CommandText = command;
			
			SqliteDataReader rd = cmd.ExecuteReader();
			
			return new RowsRet(rd);;
			
		}
		
		public static RowsRet ExecuteCommand(this SqliteConnection sql, string command, object[] param)
		{
			SqliteCommand cmd =  sql.CreateCommand();
			cmd.CommandText = command;
			foreach (object obj in param)
			{
				cmd.Parameters.Add(CreateParam(obj));
			}
			SqliteDataReader rd = cmd.ExecuteReader();
			
			return new RowsRet(rd);
			
		}
		
		
		
		public static bool DatabaseObjectExists(this SqliteConnection sql, string table)
		{
            RowsRet rows = null;
            rows = sql.ExecuteCommand(
                    "SELECT COUNT(*) FROM sqlite_master WHERE name=?", new object[] {table});
			int m_startRow = 1;
			
            if (rows == null || rows.Rows.Count < ((m_startRow == 0) ? 2 : 1))
            {
                return false;
            }
            else
            {
                int count = int.Parse(rows.Rows[(m_startRow == 0) ? 1 : 0].Cols[0]);

                if (count >= 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

			
		public static SqliteParameter CreateParam(object obj)
		{
			if (obj is  int || obj is int?)
			{
				return new SqliteParameter(DbType.Int32, obj);
			}
			if (obj is string)
			{
				return new SqliteParameter(DbType.String, obj);
			}
			if (obj is Int64 || obj is Int64?)
			{
				
				return new SqliteParameter(DbType.Int64, obj);
			}
			if (obj is bool || obj is bool?)
			{
				return new SqliteParameter(DbType.Boolean, obj);	
			}
			if (obj == null)
			{
				
				return new SqliteParameter(DbType.String, null);
			}
			else
			{
				return new SqliteParameter(DbType.String, obj.ToString());
			}
				
		}
	}
	
	    public class RowInsensitiveComparer : IComparer<string> , IEqualityComparer<string>
    {
        public int Compare(string a, string b)
        {
            return String.Compare(a, b, true);
        }

        public bool Equals(string a, string b)
        {
            return Compare(a, b) == 0;
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
	
	public class Row
    {
        public Row(RowsRet ResultSet)
        {
            this._ResultSet = ResultSet;
        }

        public RowsRet _ResultSet;

        public List<string> Cols = new List<string>();

        public String this[String name]
        {
            get
            {
                string val = null;

                int column = _ResultSet.ColumnIndex(name);

                if (column != -1)
                {
                    val = Cols[column];
                }
                return val;
            }
            set
            {
                
                int column = _ResultSet.ColumnIndex(name);

                if (column != -1)
                {
                    Cols[column] = value;
                }
            }
        }

        public bool BoolValue(string value)
        {
            return this[value] == "1";
        }

        public Nullable<int> NullableIntValue(string value)
        {
            Nullable<int> val = null;

            int num;
            if (int.TryParse(this[value], out num))
            {
                val = num;
            }

            return val;
        }

        public int IntValue(string value)
        {
            int num = 0;
            int.TryParse(this[value], out num);
            return num;
        }


    }
	
    public class RowsRet : IEnumerable<Row>
    {
        public List<Row> Rows = new List<Row>();

        public Row _Headers;
        public Dictionary<String, int> _ColumnIndexes = new Dictionary<String, int>();
		
		public RowsRet()
		{
			
		}
		
		public RowsRet(SqliteDataReader rd)
		{

            #if !MONO
			if (rd.Read())
			{
				_Headers = new Row(this);
				
				for (int i=0; i<rd.FieldCount; i++)
				{
                    _ColumnIndexes[rd.GetString(i)] = i;
	
					_Headers.Cols.Add(rd.GetString(i));
				}
			}
			
			while (rd.Read())
			{
				Row row = new Row(this);
				for (int i=0; i<rd.FieldCount; i++)
				{
					row.Cols.Add(rd.GetString(i));
						
				}
				this.Rows.Add(row);
			}
            #else
            bool first = true;
            while (rd.Read())
            {
                if (first)
                {
                    first = false;
                    _Headers = new Row(this);

                    for (int i=0; i<rd.FieldCount; i++)
                    {
                        
                        _ColumnIndexes[rd.GetName(i)] = i;

                        _Headers.Cols.Add(rd.GetName(i));
                    }
                }


                Row row = new Row(this);
                int count = rd.FieldCount;
                for (int i=0; i<count; i++)
                {
                    Type t = rd.GetFieldType(i);

                    System.Diagnostics.Debug.WriteLine(rd.GetName(i) + " " + t.ToString() + " " + i);

                    if (rd.IsDBNull(i))
                    {
                        row.Cols.Add(null);
                    }
                    else if (t == typeof(Int64))
                    {
                        row.Cols.Add(rd.GetInt64(i).ToString());
                    }
                    else if (t == typeof(String))
                    {
                        row.Cols.Add(rd.GetString(i));
                    }
                    else if (t == typeof(Object))
                    {
                        row.Cols.Add("");
                    }

                }
                this.Rows.Add(row);
            }
            #endif
		}

        public Row Headers
        {
            get
            {
                return _Headers;
            }
            set        
            {
                if (_Headers != value)
                {
                    _Headers = value;
                    _ColumnIndexes = null;
                }
            }
        }

        public int ColumnIndex(String name)
        {
            if (_ColumnIndexes == null)
            {
                _ColumnIndexes = new Dictionary<string, int>(new RowInsensitiveComparer());
                
                for (int i = 0; i < Headers.Cols.Count; i++)
                {
                    _ColumnIndexes.Add(Headers.Cols[i], i);
                }
            }

            int column = -1;

            _ColumnIndexes.TryGetValue(name, out column);

            return column;
        }

        public bool HasColumn(string name)
        {
            return (ColumnIndex(name) != -1);
        }
            
        public string ViewableString
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                int[] colSize = new int[Rows[0].Cols.Count];

                foreach (Row row in Rows)
                {
                    int i = 0;
                    foreach (string col in row.Cols)
                    {
                        colSize[i] = Math.Min(Math.Max(col.Length,
                            colSize[i]), 40);
                        i++;
                    }
                }

                int j = 0;
                foreach (Row row in Rows)
                {
                    List<string> toPrint = new List<string>();
                    bool needPrint = true;

                    foreach (string col in row.Cols)
                    {
                        toPrint.Add(col);
                    }

                    while (needPrint)
                    {
                        List<string> next = new List<string>();
                        StringBuilder line = new StringBuilder();
                        needPrint = false;

                        for (int i = 0; i < toPrint.Count; i++)
                        {
                            if (toPrint[i].Length > colSize[i])
                            {
                                line.Append(toPrint[i].Substring(
                                    0, colSize[i]));
                                line.Append(" ");
                                needPrint = true;
                                next.Add(" " + toPrint[i].Substring(
                                    colSize[i]));
                            }
                            else
                            {
                                next.Add("");
                                line.Append(toPrint[i]);
                                line.Append(new string(' ',
                                    colSize[i] - toPrint[i].Length + 1));
                            }
                        }
                        toPrint = next;
                        sb.AppendLine(line.ToString().TrimEnd(' '));
                    }

                    if (j == 0)
                    {
                        for (int col = 0; col < row.Cols.Count; col++)
                        {
                            sb.Append(new string('-', colSize[col]));
                            if (col < row.Cols.Count - 1)
                            {
                                sb.Append(" ");
                            }
                        }
                        sb.AppendLine();
                    }
                    j++;
                }

                return sb.ToString();
            }
        }

        public IEnumerator<Row> GetEnumerator()
        {
            return Rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Rows.GetEnumerator();
        }
    }
		
	
#endif
   
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue)]
    public class DBLoaderIgnoreAttribute : Attribute
    {
        public DBLoaderIgnoreAttribute() { }
    }


    public class DBLoader<T> : IDisposable where T : IDBLoadable
    {
        private DBTableDesc _RootTableDesc;
        private Dictionary<string, int> nextIndexForTable = new Dictionary<string, int>();
        private Dictionary<int, T> itemDictionary = new Dictionary<int,T>();
		
#if !MONO		
        private SQL_Lite sql;
#else
		Mono.Data.Sqlite.SqliteConnection sql;
#endif
	
        public static string CreateTableStatementForDesc(DBTableDesc table)
        {
            String str = "CREATE TABLE " + table.Name + "(ID INTEGER PRIMARY KEY ASC";

            if (!table.Primary)
            {
                str += ", OwnerID INTEGER";
            }

            foreach (DBFieldDesc field in table.Fields)
            {
                str += ", " + field.Name + " " + field.Type + (field.Nullable ? "" : " NOT NULL");

            }

            str += ");";

            return str;


        }

        public DBLoader(String filename)
        {
            Type type = typeof(T);

            string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            path = Path.Combine(path, "Combat Manager");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }


            try
            {
                string fullfilename = Path.Combine(path, filename);
				
#if !MONO
				sql = new SQL_Lite();
                sql.SkipHeaderRow = true;
                sql.Open(fullfilename);
                string backtext = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                string backname = fullfilename + backtext + ".db";
#else
                sql = new SqliteConnection("DbLinqProvider=Sqlite;Data Source=" + fullfilename);

                sql.Open();
#endif
				


				
			
#if !MONO
                //make a backup
                if (File.Exists(fullfilename) && !(File.Exists(backname)))
                {
                    try
                    {
                        File.Copy(fullfilename, backname);
                    }
                    catch (IOException ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                    }
                }
#endif



                bool needsCopy = false;

#if !MONO
				try
                {
                    sql.ExecuteCommand("SELECT name FROM sqlite_master");
                }
                catch (SQL_Lite_Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    sql.Dispose();
                    sql = null;
                    
                    string errorname = fullfilename + ".error.db";

                    if (File.Exists(errorname))
                    {
                        File.Delete(errorname);
                    }
                    File.Move(fullfilename, errorname);

                    sql = new SQL_Lite();
                    sql.SkipHeaderRow = true;

                    sql.Open(fullfilename);

                }
#endif
				
                List<DBTableDesc> tables = GetTablesForType(type);

                foreach (DBTableDesc desc in tables)
                {
                    RowsRet ret = sql.ExecuteCommand("SELECT name FROM sqlite_master WHERE type='table' AND name=?",
                        new object[] { desc.Name });

                    if (ret.Rows.Count == 0)
                    {

                        String str = CreateTableStatementForDesc(desc);
                        sql.ExecuteCommand(str);
                    }
                    else
                    {
                        if (!TableMatchesDescription(sql, desc))
                        {
                            needsCopy = true;
                            break;
                        }

                    }

                }

                if (needsCopy)
                {
                    string newfile = fullfilename + ".tmp";
                    if (File.Exists(newfile))
                    {
                        File.Delete(newfile);
                    }
					
#if !MONO
                    SQL_Lite sql2 = new SQL_Lite();
                    sql2.SkipHeaderRow = true;

                    sql2.Open(newfile);
#else
                    SqliteConnection sql2 = new SqliteConnection("DbLinqProvider=Sqlite;Data Source=" + newfile);
					
#endif
					
					
                    foreach (DBTableDesc table in tables)
                    {
                        CopyTable(sql, sql2, table);

                    }

                    sql.Dispose();
                    sql2.Dispose();

                    File.Delete(fullfilename);
                    File.Move(newfile, fullfilename);
					
#if !MONO
                    sql = new SQL_Lite();
					
                    sql2.SkipHeaderRow = true;
                    sql.Open(fullfilename);
#else
                    sql = new SqliteConnection("DbLinqProvider=Sqlite;Data Source=" + fullfilename);
#endif
                }


                LoadTableNextIndexes(tables);
                LoadDBItems();
            }
#if !MONO
            catch (SQL_Lite_Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
#else
			finally
			{
			}
#endif

        }
#if MONO
        private bool TableMatchesDescription(SqliteConnection sql, DBTableDesc table)
#else
        private bool TableMatchesDescription(SQL_Lite sql, DBTableDesc table)
#endif
        {
            RowsRet ret = sql.ExecuteCommand("PRAGMA table_info(" + table.Name + ");");

            Dictionary<string, DBFieldDesc> fieldInfo = new Dictionary<string, DBFieldDesc>();

            foreach (var field in table.Fields)
            {
                fieldInfo.Add(field.Name, field);

            }



            bool tableMatches = true;
            foreach (Row row in ret.Rows)
            {
                string name = row["name"];
                if (fieldInfo.ContainsKey(row["name"]))
                {

                    fieldInfo.Remove(row["name"]);
                }
                else
                {
                    if (name != "ID" && name != "OwnerID")
                    {
                        tableMatches = false;
                        break;
                    }
                }
            }

            if (fieldInfo.Count > 0)
            {
                tableMatches = false;
            }

            return tableMatches;
        }
		
#if !MONO
        private static void CopyTable(SQL_Lite sql, SQL_Lite sql2, DBTableDesc table)
#else
			
        private static void CopyTable(SqliteConnection sql, SqliteConnection sql2, DBTableDesc table)
#endif
        {
            String str = CreateTableStatementForDesc(table);
            sql2.ExecuteCommand(str);

            if (sql.DatabaseObjectExists(table.Name))
            {
                RowsRet data = sql.ExecuteCommand("Select * from " + table.Name);

                List<string> validOldFields = new List<string>();

                foreach (DBFieldDesc desc in table.Fields)
                {
                    if (data.HasColumn(desc.Name))
                    {
                        validOldFields.Add(desc.Name);
                    }
                }


                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("Insert into " + table.Name + " (ID");
                int count = validOldFields.Count + 1; ;
                if (!table.Primary)
                {
                    commandBuilder.Append(", OwnerID");
                    count++;
                }
                foreach (string strField in validOldFields)
                {
                    commandBuilder.Append(", " + strField);
                }
                commandBuilder.Append(") VALUES ( ?");
                for (int i = 1; i < count; i++)
                {
                    commandBuilder.Append(", ?");
                }
                commandBuilder.Append(");");
                string command = commandBuilder.ToString();

                foreach (Row row in data.Rows)
                {
                    List<object> values = new List<object>();
                    values.Add(row["ID"]);
                    if (!table.Primary)
                    {
                        values.Add(row["OwnerID"]);
                    }

                    foreach (string strField in validOldFields)
                    {
                        values.Add(row[strField]);
                    }

                    object[] objParams = values.ToArray();
                    sql2.ExecuteCommand(command, objParams);
                }
            }
        }

        private static List<DBTableDesc> GetTablesForType(Type startType)
        {
            return GetTablesForType(startType, null, false);
        }

        private DBTableDesc RootTableDesc
        {
            get
            {
                if (_RootTableDesc == null)
                {
                    _RootTableDesc = GetTablesForType(typeof(T), null, true)[0];

                }

                return _RootTableDesc;
            }
        }

        private static List<DBTableDesc> GetTablesForType(Type startType, string baseName, bool addSubtables)
        {

            Dictionary<string, DBTableDesc> tableDesc = new Dictionary<string, DBTableDesc>();

            DBTableDesc mainTable = new DBTableDesc(startType.Name, startType);
            if (baseName != null && baseName.Length > 0)
            {
                mainTable.Name = baseName + mainTable.Name;
            }
            else
            {
                mainTable.Primary = true;
            }


            tableDesc.Add(mainTable.Name, mainTable);



            List<PropertyInfo> propInfo = GetUsableFields(startType);
            System.Diagnostics.Debug.Assert(propInfo.Count > 0);

            string subtableBaseName = startType.Name + "__";

            if (baseName != null && baseName.Length > 0)
            {
                subtableBaseName = baseName + subtableBaseName;
            }


            foreach (PropertyInfo info in propInfo)
            {
                Type type = info.PropertyType;
                DBFieldDesc desc = GetDescForType(info.Name, type, info);
                DBTableDesc fieldSubtable = null;

                if (desc != null)
                {
                    mainTable.Fields.Add(desc);
                }
                else if (IsEnumerationTableType(type))
                {
                    DBFieldDesc subdesc = GetDescForType(info.Name, type.GetGenericArguments()[0], info);

                    if (subdesc != null)
                    {
                        DBTableDesc subtable = new DBTableDesc(subtableBaseName + info.Name, info.PropertyType);
                        subtable.Fields.Add(subdesc);
                        tableDesc.Add(subtable.Name, subtable);


                        fieldSubtable = subtable;
                    }
                    else
                    {
                        List<DBTableDesc> newList = GetTablesForType(type.GetGenericArguments()[0], subtableBaseName, addSubtables);
                        System.Diagnostics.Debug.Assert(newList.Count > 0);


                        fieldSubtable = newList[0];

                        foreach (DBTableDesc subtable in newList)
                        {
                            System.Diagnostics.Debug.Assert(subtable.Fields.Count > 0);
                            if (!tableDesc.ContainsKey(subtable.Name))
                            {
                                tableDesc.Add(subtable.Name, subtable);
                            }
                        }
                    }
                }
                else
                {
                    List<DBTableDesc> newList = GetTablesForType(type, subtableBaseName, addSubtables);

                    System.Diagnostics.Debug.Assert(newList.Count > 0);
                    foreach (DBTableDesc subtable in newList)
                    {
                        if (!tableDesc.ContainsKey(subtable.Name))
                        {
                            tableDesc.Add(subtable.Name, subtable);
                        }
                    }

                    fieldSubtable = newList[0];
                }

                if (fieldSubtable != null && addSubtables)
                {
                    DBFieldDesc fieldDesc = new DBFieldDesc();
                    fieldDesc.Name = info.Name;
                    fieldDesc.Subtable = fieldSubtable;
                    fieldDesc.Info = info;
                    mainTable.Fields.Add(fieldDesc);
                }
            }


            List<DBTableDesc> tables = new List<DBTableDesc>();
            foreach (DBTableDesc desc in tableDesc.Values)
            {
                tables.Add(desc);
            }

            return tables;
        }

        private static DBFieldDesc GetDescForType(string fieldname, Type type, PropertyInfo info)
        {

            if (type == typeof(Int32) || type == typeof(Int64) || type == typeof(bool))
            {
                return new DBFieldDesc(fieldname, "INTEGER", false, info);
            }
            else if (type == typeof(Nullable<Int32>) || type == typeof(Nullable<Int64>))
            {
                return new DBFieldDesc(fieldname, "INTEGER", true, info);
            }
            else if (type == typeof(string))
            {
                return new DBFieldDesc(fieldname, "TEXT", true, info);
            }

            return null;
        }



        private static List<PropertyInfo> GetUsableFields(Type type)
        {
            List<PropertyInfo> fields = new List<PropertyInfo>();

            Type t = type;

            foreach (PropertyInfo info in t.GetProperties())
            {

                bool ignore = false;

                if (!info.CanRead || !info.CanWrite)
                {
                    ignore = true;
                }

                if (!ignore)
                {
                    if (info.GetGetMethod().IsStatic)
                    {
                        ignore = true;
                    }
                }


                if (!ignore)
                {
                    foreach (object attr in info.GetCustomAttributes(typeof(XmlIgnoreAttribute), true))
                    {
                        ignore = true;
                        break;
                    }
                }

                if (!ignore)
                {
                    foreach (object attr in info.GetCustomAttributes(typeof(DBLoaderIgnoreAttribute), true))
                    {
                        ignore = true;
                        break;
                    }
                }

                if (!ignore)
                {
                    if (info.Name == "DBLoaderID")
                    {
                        ignore = true;
                    }

                }

                if (!ignore)
                {
                    fields.Add(info);
                }
            }

            return fields;
        }

        private void LoadDBItems()
        {

            List<T> values = LoadValues();

            foreach (T item in values)
            {
                itemDictionary.Add(item.DBLoaderID, item);
            }
        }

        private List<T> LoadValues()
        {
            List<T> items = new List<T>();
            foreach (T item in LoadValues(RootTableDesc, 0))
            {
                items.Add(item);
            }
            return items;
        }

        private List<object> LoadValues(DBTableDesc table, int owner)
        {
            List<object> valueList = new List<object>();


            RowsRet ret;


            if (table.Primary)
            {
                ret = sql.ExecuteCommand("SELECT * FROM " + table.Name + ";");
            }
            else
            {
                ret = sql.ExecuteCommand("SELECT * FROM " + table.Name + " WHERE OwnerID = ?;", new object[] {owner});

            }

            foreach (Row row in ret.Rows)
            {
                ConstructorInfo info = table.Type.GetConstructor(new Type[] { });
                Object item = info.Invoke(new object[]{});

                int index = row.IntValue("ID");


                foreach (DBFieldDesc field in table.ValueFields)
                {
                    string stringVal = row[field.Name];

                    object obVal = ParseObjectForType(stringVal, field.Info.PropertyType);


                    field.Info.GetSetMethod().Invoke(item, new object[] { obVal });
                }

                foreach (DBFieldDesc field in table.SubtableFields)
                {
                    if (IsEnumerationTableType(field.Info.PropertyType))
                    {
                        List<object> list;

                        if (IsSimpleValueEnumeration(field.Info.PropertyType))
                        {
                            list = LoadSimpleValues(field.Subtable, index);
                        }
                        else
                        {
                            list = LoadValues(field.Subtable, index);
                        }

    
                        ConstructorInfo cons = field.Info.PropertyType.GetConstructor(new Type[] { });
                        IList newList = (IList)cons.Invoke(new object[0]);
                        foreach (object listitem in list)
                        {
                            newList.Add(listitem);
                        }

                        field.Info.GetSetMethod().Invoke(item, new object[] { newList });
                        
                    }
                    else
                    {
                        List<object> list = LoadValues(field.Subtable, index);

                        if (list.Count > 0)
                        {

                            field.Info.GetSetMethod().Invoke(item, new object[] { list[0] });
                        }
                    }
                }

                if (table.Primary)
                {
                    IDBLoadable loadable = (IDBLoadable)item;
                    loadable.DBLoaderID = index;
                }

                valueList.Add(item);


            }

            return valueList;
        }

        public List<object> LoadSimpleValues(DBTableDesc table, int owner)
        {
            
            string field = table.Fields[0].Name;
            Type type = table.Type.GetGenericArguments()[0];

            RowsRet ret = sql.ExecuteCommand("SELECT " + field + " FROM " + table.Name + " WHERE OwnerID = ?;", new object[] { owner });

            List<object> list = new List<object>();


            foreach (Row row in ret.Rows)
            {
                list.Add(ParseObjectForType(row.Cols[0], type));
            }

            return list;
        }

        public Object ParseObjectForType(string text, Type t)
        {
            try
            {
                if (t == typeof(Int32))
                {
                    return Int32.Parse(text);
                }
                if (t == typeof(Int64))
                {
                    return Int64.Parse(text);
                }
                if (t == typeof(Nullable<Int32>))
                {
                    Nullable<Int32> val = null;

                    int num;
                    if (Int32.TryParse(text, out num))
                    {
                        val = num;
                    }

                    return val;
                }
                if (t == typeof(Nullable<Int64>))
                {
                    Nullable<Int64> val = null;

                    Int64 num;
                    if (Int64.TryParse(text, out num))
                    {
                        val = num;
                    }

                    return val;
                }
                if (t == typeof(bool))
                {
                    return text == "1";
                }
                if (t == typeof(string))
                {
                    return text;
                }
            }
            catch (Exception)
            {
                throw;
            }

            System.Diagnostics.Debug.Assert(false);
            return null;
            

        }

        public int AddItem(T item)
        {

            sql.ExecuteCommand("BEGIN TRANSACTION");
            int index =  InsertItem(item);
            sql.ExecuteCommand("END TRANSACTION");
            itemDictionary.Add(index, item);
            return index;
        }

        public void AddItems(IEnumerable<T> items)
        {
            sql.ExecuteCommand("BEGIN TRANSACTION");
            foreach (T item in items)
            {
                InsertItem(item);
            }
            sql.ExecuteCommand("END TRANSACTION");
            foreach (T item in items)
            {
                itemDictionary.Add(item.DBLoaderID, item);
            }
        }

        private void LoadTableNextIndexes(List<DBTableDesc> tables)
        {
            foreach (DBTableDesc table in tables)
            {
                UpdateNextIndexForTableFromDB(table.Name);
            }
        }

        private void UpdateNextIndexForTableFromDB(string tableName)
        {
            string statement = "SELECT MAX(ID) FROM " + tableName + ";" ;

            RowsRet ret = sql.ExecuteCommand(statement);
            if (ret.Rows.Count == 0)
            {
                nextIndexForTable[tableName] = 1;
            }
            else
            {
                nextIndexForTable[tableName] = ret.Rows[0].IntValue("ID") + 1;
            }
        }


        private int InsertItem(T item)
        {
            DBTableDesc table = RootTableDesc;

            int val =  InsertItemForTable(item, table, 0);

            return val;
        }

        private int InsertItemForTable(object item, DBTableDesc table, int owner)
        {

            List<object> insertParams = new List<object>();

            
            int index;
            if (table.Primary && ((IDBLoadable)item).DBLoaderID != 0)
            {
                index = ((IDBLoadable)item).DBLoaderID;
            }
            else
            {
                index = GetNextIndex(table);
            }


            int count = 1;
            insertParams.Add(index);

            if (!table.Primary)
            {
                insertParams.Add(owner);
                count++;
            }

            StringBuilder strb = new StringBuilder();
            strb.Append("INSERT INTO " + table.Name + "(ID");
            if (!table.Primary)
            {
                strb.Append(", OwnerID");
            }
            List<DBFieldDesc> subtables = new List<DBFieldDesc>();
            foreach (DBFieldDesc field in table.Fields)
            {
                if (field.Subtable == null)
                {
                    strb.Append(", " + field.Name);
                    insertParams.Add(field.Info.GetGetMethod().Invoke(item, new object[]{}));
                    count++;
                }
                else
                {
                    subtables.Add(field);
                }
            }

            strb.Append(") VALUES (?");

            for (int i=1; i<count; i++)
            {
                strb.Append(", ?");
            }

            strb.Append(");");

            string command = strb.ToString();
            sql.ExecuteCommand(command, insertParams.ToArray());

            InsertItemSubtables(item, table, index);


            if (table.Primary)
            {
                IDBLoadable loadable = (IDBLoadable)item;
                loadable.DBLoaderID = index;
            }

            return index;
        }

        private void InsertItemSubtables(object item, DBTableDesc table, int index)
        {
            foreach (DBFieldDesc desc in table.SubtableFields)
            {

                Object subitem = desc.Info.GetGetMethod().Invoke(item, new object[] { });

                if (subitem != null)
                {
                    if (IsEnumerationTableType(desc.Info.PropertyType))
                    {
                        foreach (Object o in ((IEnumerable)subitem))
                        {
                            if (IsSimpleValueEnumeration(desc.Info.PropertyType))
                            {
                                InsertSimpleSubtableItem(o, desc.Subtable, index);
                            }
                            else
                            {
                                InsertItemForTable(o, desc.Subtable, index);
                            }
                        }

                    }
                    else
                    {
                        InsertItemForTable(subitem, desc.Subtable, index);
                    }
                }
            }
        }


        private int InsertSimpleSubtableItem(object item, DBTableDesc table, int owner)
        {
            string command = "INSERT INTO " + table.Name + " (ID, OwnerID, " + 
                table.Fields[0].Name +") VALUES (?,?,?);";

            int index = GetNextIndex(table);

            object[] insertParams = new object[] { index, owner, item };

            sql.ExecuteCommand(command, insertParams);

            return index;

        }

        public void DeleteItems(IEnumerable<T> items)
        {

            List<String> statementList = new List<string>();
            List<object[]> paramList = new List<object[]>();
            foreach (T item in items)
            {
                CreateDeleteTableItemStatements(RootTableDesc, item.DBLoaderID, statementList, paramList);

            }


            RunTransaction(statementList, paramList);

            foreach (T item in items)
            {
                itemDictionary.Remove(item.DBLoaderID);
            }
        }

        public void DeleteItem(T item)
        {
            if (item.DBLoaderID == 0)
            {
                throw new ArgumentException("Invalid DBLoaderID", "item");
            }

            List<String> statementList = new List<string>();
            List<object[]> paramList = new List<object[]>();
            CreateDeleteTableItemStatements(RootTableDesc, item.DBLoaderID, statementList, paramList);

            RunTransaction(statementList, paramList);

            itemDictionary.Remove(item.DBLoaderID);
        }

        public void RunTransaction(List<string> statementList, List<object[]> paramList)
        {
            
            sql.ExecuteCommand("BEGIN TRANSACTION;");

            RunStatementList(statementList, paramList);

            sql.ExecuteCommand("END TRANSACTION;");
        }

        public void RunStatementList(List<string> statementList, List<object[]> paramList)
        {
            for (int i = 0; i < statementList.Count; i++)
            {
                sql.ExecuteCommand(statementList[i], paramList[i]);
            }
        }

        public void UpdateItem(T item)
        {


            sql.ExecuteCommand("BEGIN TRANSACTION;");
            
            List<String> statementList = new List<string>();
            List<object[]> paramList = new List<object[]>();
            CreateDeleteTableItemStatements(RootTableDesc, item.DBLoaderID, statementList, paramList);

            RunStatementList(statementList, paramList);
            InsertItemForTable(item, RootTableDesc, 0); 

            sql.ExecuteCommand("END TRANSACTION;");

        }


        public void CreateDeleteTableItemStatements(DBTableDesc basetable, int index, List<String> statementList, List<object[]> paramList)
        {
            CreateDeleteTableItemStatements(basetable, index, statementList, paramList, false);
        }

        public void CreateDeleteTableItemStatements(DBTableDesc basetable, int index, List<String> statementList, List<object[]> paramList, bool ignoreCurrent)
        {
            object[] idParam = new object[]{index};
            if (!ignoreCurrent)
            {
                statementList.Add("DELETE FROM " + basetable.Name + " WHERE ID = ?;");
                paramList.Add(idParam);
            }

            foreach (DBFieldDesc field in basetable.SubtableFields)
            {
                DBTableDesc table = field.Subtable;
                RowsRet ret = sql.ExecuteCommand("SELECT ID FROM " + table.Name + " WHERE OwnerID = ?", idParam);

                foreach (Row row in ret.Rows)
                {
                    CreateDeleteTableItemStatements(table, row.IntValue("ID"), statementList, paramList);
                }
            }
        }

        private int GetNextIndex(DBTableDesc table)
        {

            int index = nextIndexForTable[table.Name];
            nextIndexForTable[table.Name] = index + 1;

            return index;
        }

        private static bool IsEnumerationTableType(Type type)
        {
            return type.GetInterface("IEnumerable") != null && type.IsGenericType == true;
        }

        private static bool IsSimpleValueEnumeration(Type enumType)
        {
            Type type = enumType.GetGenericArguments()[0];

            return type == typeof(Int32) || type == typeof(Int64) || type == typeof(bool)  ||
                type == typeof(Nullable<Int32>) || type == typeof(Nullable<Int64>) ||
                type == typeof(string);
        }

        public void Dispose()
        {
            if (sql != null)
            {
                sql.Dispose();
                sql = null;
            }

        }

        public IEnumerable<T> Items
        {
            get
            {
                if (itemDictionary == null)
                {
                    LoadDBItems();
                }

                return itemDictionary.Values;
            }
        }
    }
}
