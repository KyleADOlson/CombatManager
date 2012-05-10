/*
 *  SQL Lite.cs
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

﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Collections;
using System.Diagnostics;

namespace ScottsUtils
{
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

    #region Row
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
    #endregion
    #region SQLVariant
    public class SQLVariant
    {
        string m_data;
        public string Data
        {
            get
            {
                return m_data;
            }
        }

        public SQLVariant(string data)
        {
            m_data = data;
        }

        public static implicit operator string(SQLVariant value)
        {
            return value.Data;
        }

        public static implicit operator bool(SQLVariant value)
        {
            return SQL_Lite.StringToBool(value.Data);
        }

        public static implicit operator Int32(SQLVariant value)
        {
            return SQL_Lite.StringToInt32(value.Data);
        }

        public static implicit operator Int64(SQLVariant value)
        {
            return SQL_Lite.StringToInt64(value.Data);
        }

        public static implicit operator Single(SQLVariant value)
        {
            return SQL_Lite.StringToSingle(value.Data);
        }

        public static implicit operator Double(SQLVariant value)
        {
            return SQL_Lite.StringToDouble(value.Data);
        }

        public static implicit operator byte[](SQLVariant value)
        {
            return SQL_Lite.StringToByte(value.Data);
        }

        public static implicit operator DateTime(SQLVariant value)
        {
            return SQL_Lite.StringToDateTime(value.Data);
        }

        public static implicit operator SQLVariant(string value)
        {
            return new SQLVariant(value);
        }

        public static implicit operator SQLVariant(bool value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }

        public static implicit operator SQLVariant(Int32 value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }

        public static implicit operator SQLVariant(Int64 value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }

        public static implicit operator SQLVariant(Single value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }

        public static implicit operator SQLVariant(Double value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }

        public static implicit operator SQLVariant(DateTime value)
        {
            return new SQLVariant(SQL_Lite.ToString(value));
        }
    }
    #endregion
    #region RowDictionary
    public class RowDictionary : Dictionary<string, SQLVariant>
    {
        // Nothing extra
    }
    #endregion
    #region RowReader
    public class RowReader : List<RowDictionary>
    {
        public RowReader(RowsRet ret)
        {
            bool header = true;

            if (ret.Rows.Count > 1)
            {
                Capacity = ret.Rows.Count - 1;

                foreach (Row row in ret.Rows)
                {
                    if (!header)
                    {
                        Add(SQL_Lite.RowsToDict(ret.Rows[0], row));
                    }
                    else
                    {
                        header = false;
                    }
                }
            }
        }

        public static implicit operator RowReader(RowsRet value)
        {
            return new RowReader(value);
        }
    }
    #endregion
    #region RowsRet
    public class RowsRet : IEnumerable<Row>
    {
        public List<Row> Rows = new List<Row>();

        public Row _Headers;
        public Dictionary<String, int> _ColumnIndexes;

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
    #endregion
    #region SQL_Lite_Exception
    public class SQL_Lite_Exception : Exception
    {
        public SQL_Lite_Exception(string message)
            : base(message)
        {
        }
    }
    #endregion

    class TableRow : IEditableObject
    {
        // TODO

        #region IEditableObject Members

        public void BeginEdit()
        {
            throw new NotImplementedException();
        }

        public void CancelEdit()
        {
            throw new NotImplementedException();
        }

        public void EndEdit()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class Table : IBindingList
    {
        // TODO

        #region IBindingList Members

        public void AddIndex(PropertyDescriptor property)
        {
            // Nothing to do
        }

        public object AddNew()
        {
            throw new NotImplementedException();
        }

        public bool AllowEdit
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowNew
        {
            get { throw new NotImplementedException(); }
        }

        public bool AllowRemove
        {
            get { throw new NotImplementedException(); }
        }

        public void ApplySort(PropertyDescriptor property, ListSortDirection direction)
        {
            throw new NotImplementedException();
        }

        public int Find(PropertyDescriptor property, object key)
        {
            throw new NotImplementedException();
        }

        public bool IsSorted
        {
            get { throw new NotImplementedException(); }
        }

        // TODO: Remove this Pragma
#pragma warning disable 67
        public event ListChangedEventHandler ListChanged;

        public void RemoveIndex(PropertyDescriptor property)
        {
            throw new NotImplementedException();
        }

        public void RemoveSort()
        {
            throw new NotImplementedException();
        }

        public ListSortDirection SortDirection
        {
            get { throw new NotImplementedException(); }
        }

        public PropertyDescriptor SortProperty
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsChangeNotification
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSearching
        {
            get { throw new NotImplementedException(); }
        }

        public bool SupportsSorting
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(object value)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(object value)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotImplementedException();
        }

        public bool IsFixedSize
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public void Remove(object value)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public object this[int index]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsSynchronized
        {
            get { throw new NotImplementedException(); }
        }

        public object SyncRoot
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        #region IEnumerable Members

        public System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion
    }

    public class SQL_Lite : IDisposable
    {
        /* Define SQL_LITE_Time_Commands in project settings to time all 
         * SQL calls and provide the WriteTimings function to dump out 
         * the time required for SQL calls */

        #region String conversion helpers
        public static bool StringToBool(string value)
        {
            if (value == "" || value == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static Int32 StringToInt32(string value)
        {
            if (value == null || value == "")
            {
                return 0;
            }
            else
            {
                return Int32.Parse(value);
            }
        }
        public static Int64 StringToInt64(string value)
        {
            if (value == null || value == "")
            {
                return 0;
            }
            else
            {
                return Int64.Parse(value);
            }
        }
        public static Single StringToSingle(string value)
        {
            if (value == null || value == "")
            {
                return 0;
            }
            else
            {
                Int32 temp = Int32.Parse(value);
                byte[] bits = BitConverter.GetBytes(temp);
                return BitConverter.ToSingle(bits, 0);
            }
        }
        public static Double StringToDouble(string value)
        {
            if (value == null || value == "")
            {
                return 0;
            }
            else
            {
                Int64 temp = Int64.Parse(value);
                byte[] bits = BitConverter.GetBytes(temp);
                return BitConverter.ToDouble(bits, 0);
            }
        }
        public static byte[] StringToByte(string value)
        {
            if (value == null || value == "")
            {
                return null;
            }
            else
            {
                try
                {
                    return Convert.FromBase64String(value);
                }
                catch
                {
                    return null;
                }
            }
        }
        public static RowDictionary RowsToDict(RowsRet rows)
        {
            RowDictionary rowDict =
                new RowDictionary();

            List<string>.Enumerator headerRow =
                rows.Rows[0].Cols.GetEnumerator();

            foreach (string cell in rows.Rows[1].Cols)
            {
                headerRow.MoveNext();
                rowDict.Add(headerRow.Current, cell);
            }
            return rowDict;
        }

        public static RowDictionary RowsToDict(Row header, Row row)
        {
            RowDictionary rowDict =
                new RowDictionary();

            List<string>.Enumerator headerRow =
                header.Cols.GetEnumerator();

            foreach (string cell in row.Cols)
            {
                headerRow.MoveNext();
                if (!rowDict.ContainsKey(headerRow.Current))
                {
                    rowDict.Add(headerRow.Current, cell);
                }
            }

            return rowDict;
        }
        public static DateTime StringToDateTime(string value)
        {
            if (value == null || value == "")
            {
                return DateTime.MinValue;
            }
            else
            {
                return new DateTime(long.Parse(value));
            }
        }

        public static string ToString(DateTime value)
        {
            return value.Ticks.ToString();
        }
        public static string ToString(byte[] value)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return Convert.ToBase64String(value);
            }
        }
        public static string ToString(bool value)
        {
            return value ? "1" : "0";
        }
        public static string ToString(Int32 value)
        {
            return value.ToString();
        }
        public static string ToString(Int64 value)
        {
            return value.ToString();
        }
        public static string ToString(Single value)
        {
            return value.ToString();
        }
        public static string ToString(Double value)
        {
            return value.ToString();
        }
        #endregion
        #region Interop string helper class
        class StringMarshal : IDisposable
        {
            private IntPtr m_native = IntPtr.Zero;
            private string m_string = null;

            public static string Convert(IntPtr value)
            {
                StringMarshal ret = new StringMarshal(value);
                return ret.Value;
            }

            public IntPtr Native
            {
                get
                {
                    if (m_native == IntPtr.Zero)
                    {
                        MakeNative();
                    }
                    return m_native;
                }
            }

            public IntPtr OwnNative
            {
                get
                {
                    if (m_native == IntPtr.Zero)
                    {
                        MakeNative();
                    }
                    IntPtr ret = m_native;

                    m_native = IntPtr.Zero;
                    m_string = "";

                    return ret;
                }
            }

            public string Value
            {
                get
                {
                    if (m_string == null)
                    {
                        FreeNative();
                    }
                    return m_string;
                }
            }

            private void MakeNative()
            {
                m_native = Marshal.StringToHGlobalUni(m_string);
                m_string = null;
            }

            private void FreeNative()
            {
                m_string = Marshal.PtrToStringUni(m_native);
                Marshal.FreeHGlobal(m_native);
                m_native = IntPtr.Zero;
            }

            public StringMarshal(string value)
            {
                m_string = value;
            }

            public StringMarshal(int len)
            {
                m_native = Marshal.AllocHGlobal((len + 1) * 2);
            }

            public StringMarshal(IntPtr value)
            {
                m_native = value;
            }

            ~StringMarshal()
            {
                Dispose(false);
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (m_native != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(m_native);
                    m_native = IntPtr.Zero;
                }
            }
        }
        #endregion
        #region Interop delegates
        delegate void AddStringToBindDelegate(IntPtr hDB, IntPtr hBind, IntPtr szData);
        delegate void CloseBindObjectDelegate(IntPtr hDB, IntPtr hRet);
        delegate IntPtr CloseDatabaseDelegate(IntPtr hDB);
        delegate void CloseReturnDelegate(IntPtr hDB, IntPtr hRet);
        delegate IntPtr CreateBindObjectDelegate(IntPtr hDB);
        delegate IntPtr ExecuteCommandDelegate(IntPtr hDB, IntPtr szCommand, IntPtr hBind);
        delegate int GetChangesDelegate(IntPtr hDB);
        delegate IntPtr GetDBLastErrorDelegate(IntPtr hDB);
        delegate Int64 GetLastRowIDDelegate(IntPtr hDB);
        delegate IntPtr GetSQLiteVersionDelegate();
        delegate IntPtr GetStringDelegate(IntPtr hDB, IntPtr hRet, int nRow, int nCol);
        delegate void GetTableSize(IntPtr hDB, IntPtr hRet, ref int pRows, ref int pCols);
        delegate IntPtr OpenDatabaseDelegate(IntPtr szDatabase);

        [StructLayout(LayoutKind.Sequential)]
        struct Delegates
        {
            public AddStringToBindDelegate AddStringToBind;
            public CloseBindObjectDelegate CloseBindObject;
            public CloseDatabaseDelegate CloseDatabase;
            public CloseReturnDelegate CloseReturn;
            public CreateBindObjectDelegate CreateBindObject;
            public ExecuteCommandDelegate ExecuteCommand;
            public GetChangesDelegate GetChanges;
            public GetDBLastErrorDelegate GetDBLastError;
            public GetLastRowIDDelegate GetLastRowID;
            public GetSQLiteVersionDelegate GetSQLiteVersion;
            public GetStringDelegate GetString;
            public GetTableSize GetTableSize;
            public OpenDatabaseDelegate OpenDatabase;
        }

        abstract class Imports
        {
            abstract public void GetAllExports();

            public Delegates delegates;
        }

        class Imports32 : Imports
        {
            [DllImport("SQLite32.dll", EntryPoint = "#10")]
            static extern void _GetAllExports(int cookie,
                ref Delegates delegates);

            public override void GetAllExports()
            {
                delegates = new Delegates();
                _GetAllExports(0x0ef1590f, ref delegates);
            }
        }
        class Imports64 : Imports
        {
            [DllImport("SQLite64.dll", EntryPoint = "#10")]
            static extern void _GetAllExports(int cookie,
                ref Delegates delegates);

            public override void GetAllExports()
            {
                delegates = new Delegates();
                _GetAllExports(0x0ef1590f, ref delegates);
            }
        }
        #endregion
        #region Timing helper functions
#if SQL_LITE_Time_Commands
        private class SQLCommandTime : IComparable<SQLCommandTime>
        {
            public string Command;
            public DateTime LastStart;
            public DateTime LastEnd;
            public TimeSpan TotalTime = TimeSpan.Zero;
            public long TotalThreadTime = 0;
            public int TotalCalls = 0;
            public TimeSpan AverageTime = TimeSpan.Zero;
            public long AverageThreadTime = 0;
            public string ThreadID = CurrentThreadID;

            public static string CurrentThreadID
            {
                get
                {
                    if (System.Threading.Thread.CurrentThread.Name != null &&
                        System.Threading.Thread.CurrentThread.Name.Length > 0)
                    {
                        return System.Threading.Thread.CurrentThread.Name;
                    }
                    else
                    {
                        return System.Threading.Thread.
                            CurrentThread.ManagedThreadId.ToString();
                    }
                }
            }

            public int CompareTo(SQLCommandTime other)
            {
                if (AverageThreadTime != other.AverageThreadTime)
                {
                    return AverageThreadTime.CompareTo(
                        other.AverageThreadTime);
                }
                else if (AverageTime.TotalMilliseconds != 
                    other.AverageTime.TotalMilliseconds)
                {
                    return AverageTime.TotalMilliseconds.CompareTo(
                        other.AverageTime.TotalMilliseconds);
                }
                else if (TotalCalls != other.TotalCalls)
                {
                    return TotalCalls.CompareTo(other.TotalCalls);
                }
                else
                {
                    return Command.ToLower().CompareTo(
                        other.Command.ToLower());
                }
            }
        }
        private Dictionary<string, SQLCommandTime> m_timings = 
            new Dictionary<string, SQLCommandTime>();

        public void WriteTimings(string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("Command");
            sb.Append(",");
            sb.Append("Thread ID");
            sb.Append(",");
            sb.Append("Thread Time");
            sb.Append(",");
            sb.Append("Time");
            sb.Append(",");
            sb.Append("Calls");
            sb.Append(",");
            sb.Append("Average Thread Time");
            sb.Append(",");
            sb.Append("Average Time");
            sb.Append("\r\n");

            List<SQLCommandTime> sorted = new List<SQLCommandTime>();
            foreach (SQLCommandTime cur in m_timings.Values)
            {
                if (cur.TotalCalls == 1)
                {
                    cur.AverageTime = cur.TotalTime;
                    cur.AverageThreadTime = (long)(
                        ((double)cur.TotalThreadTime) / 
                        ((double)cur.TotalCalls) / ((double)10000));
                }
                else if (cur.TotalCalls > 1)
                {
                    cur.AverageTime = TimeSpan.FromMilliseconds(
                        ((double)cur.TotalTime.TotalMilliseconds) / 
                        ((double)cur.TotalCalls));
                    cur.AverageThreadTime = (long)(
                        ((double)cur.TotalThreadTime) / 
                        ((double)cur.TotalCalls) / ((double)10000));
                }
                sorted.Add(cur);
            }
            sorted.Sort();
            sorted.Reverse();

            foreach (SQLCommandTime cur in sorted)
            {
                sb.Append(EscapeCSV(cur.Command));
                sb.Append(",");
                sb.Append(EscapeCSV(cur.ThreadID));
                sb.Append(",");
                sb.Append(EscapeCSV(cur.TotalThreadTime.ToString()));
                sb.Append(",");
                sb.Append(EscapeCSV(
                    cur.TotalTime.TotalMilliseconds.ToString()));
                sb.Append(",");
                sb.Append(EscapeCSV(cur.TotalCalls.ToString()));
                sb.Append(",");
                sb.Append(EscapeCSV(cur.AverageThreadTime.ToString()));
                sb.Append(",");
                sb.Append(EscapeCSV(
                    cur.AverageTime.TotalMilliseconds.ToString()));
                sb.Append("\r\n");
            }

            File.WriteAllText(filename, sb.ToString());
        }

        private string EscapeCSV(string value)
        {
            if (value.Contains(",") ||
                value.Contains("\"") ||
                value.Contains("\n") ||
                value.Contains("\r"))
            {
                return "\"" + value.Replace("\"", "\"\"") + "\"";
            }
            else
            {
                return value;
            }
        }
#endif
        #endregion

        public object DbWorker;

        private IntPtr m_hDB = IntPtr.Zero;
        private Imports m_imports;
        private string m_file;
        private bool m_disposed = false;
        private int m_startRow = 0;

#if DEBUG
        public static bool ShowAllCommands = true;
#else
        public static bool ShowAllCommands = false;
#endif

        public bool SkipHeaderRow
        {
            set
            {
                m_startRow = value ? 1 : 0;
            }
            get
            {
                return (m_startRow == 1);
            }
        }

        public bool DatabaseObjectExists(string name)
        {
            RowsRet rows = null;
            rows = ExecuteCommand(
                    "SELECT COUNT(*) FROM sqlite_master WHERE name=?", name);

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

        public Int64 GetLastRowID()
        {
            return m_imports.delegates.GetLastRowID(m_hDB);
        }

        public int GetChanges()
        {
            return m_imports.delegates.GetChanges(m_hDB);
        }

        public string GetSQLiteVersion()
        {
            return StringMarshal.Convert(
                m_imports.delegates.GetSQLiteVersion());
        }

        public void ResetDB()
        {
            lock (DbWorker)
            {
                m_imports.delegates.CloseDatabase(m_hDB);
                System.IO.File.Move(m_file, m_file + ".old");
                using (StringMarshal sm = new StringMarshal(m_file))
                {
                    m_hDB = m_imports.delegates.OpenDatabase(sm.Native);
                }
            }
        }

        public RowsRet Explain(string sql)
        {
            return Explain(sql, true);
        }

        public RowsRet Explain(string sql, bool queryPlan)
        {
            RowsRet ret = ExecuteCommand(
                "EXPLAIN " + (queryPlan ? "QUERY PLAN " : "") + sql);

            return ret;
        }

        public RowsRet ExecuteCommand(string szCommand)
        {
            return ExecuteCommand(szCommand, IntPtr.Zero);
        }

        public RowsRet ExecuteCommand(string szCommand, params object[] args)
        {
            return ExecuteCommandParams(szCommand, args);
        }

        public RowsRet ExecuteCommandParams(string szCommand, object[] args)
        {
            IntPtr hBind = m_imports.delegates.CreateBindObject(m_hDB);

            foreach (object arg in args)
            {
                StringMarshal sm;
                if (arg is Int64)
                {
                    sm = new StringMarshal(ToString((Int64)arg));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is Boolean)
                {
                    sm = new StringMarshal(ToString((Boolean)arg));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is DateTime)
                {
                    sm = new StringMarshal(ToString((DateTime)arg));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is Int32)
                {
                    sm = new StringMarshal((ToString((Int32)arg)));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is Single)
                {
                    sm = new StringMarshal((ToString((Single)arg)));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is Double)
                {
                    sm = new StringMarshal((ToString((Double)arg)));
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg is string)
                {
                    sm = new StringMarshal((string)arg);
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else if (arg == null)
                {
                    sm = new StringMarshal(IntPtr.Zero);
                    m_imports.delegates.AddStringToBind(
                        m_hDB, hBind, sm.OwnNative);
                }
                else
                {
                    throw new Exception("Unknown type: " + arg.GetType().ToString());
                }
            }

            RowsRet ret = ExecuteCommand(szCommand, hBind);

            m_imports.delegates.CloseBindObject(m_hDB, hBind);

            return ret;
        }

        private RowsRet ExecuteCommand(string szCommand, IntPtr hBind)
        {
            if (ShowAllCommands)
            {
                Debug.WriteLine("Command: " + szCommand);
            }

            RowsRet ret = new RowsRet();

            IntPtr hRet = IntPtr.Zero;
            lock (DbWorker)
            {
                using (StringMarshal sm = new StringMarshal(szCommand))
                {
#if SQL_LITE_Time_Commands
                        SQLCommandTime time;
                        string normalized = szCommand.ToLower();
                        System.Text.RegularExpressions.Regex re = 
                            new System.Text.RegularExpressions.Regex(
                            "'[^']+'");
                        normalized = re.Replace(normalized, "-");
                        re = new System.Text.RegularExpressions.Regex(
                            "[0-9]+");
                        normalized = re.Replace(normalized, "#");

                        if (normalized.StartsWith("BEGIN"))
                        {
                            normalized = "[Compound query]";
                        }

                        normalized = SQLCommandTime.CurrentThreadID + 
                            " - " + normalized;

                        if (m_timings.ContainsKey(normalized))
                        {
                            time = m_timings[normalized];
                        }
                        else
                        {
                            time = new SQLCommandTime();
                            time.Command = szCommand;
                            m_timings.Add(normalized, time);
                        }

                        time.LastStart = DateTime.Now;

                        long temp;
                        long kernelStart;
                        long userStart;

                        WinAPI.GetThreadTimes(WinAPI.GetCurrentThread(), 
                            out temp, out temp, 
                            out kernelStart, out userStart);
#endif

                    hRet = m_imports.delegates.ExecuteCommand(
                        m_hDB, sm.Native, hBind);

#if SQL_LITE_Time_Commands
                        long kernelEnd;
                        long userEnd;

                        WinAPI.GetThreadTimes(
                            WinAPI.GetCurrentThread(), out temp, 
                            out temp, out kernelEnd, out userEnd);

                        time.LastEnd = DateTime.Now;
                        time.TotalThreadTime += (kernelEnd - kernelStart) + 
                            (userEnd - userStart);
                        time.TotalTime += time.LastEnd - time.LastStart;
                        time.TotalCalls ++;
#endif
                }
            }

            if (hRet == IntPtr.Zero)
            {
                throw new SQL_Lite_Exception("SQL Error: " +
                    StringMarshal.Convert(
                    m_imports.delegates.GetDBLastError(m_hDB)));
            }
            else
            {
                int rows = 0;
                int cols = 0;

                m_imports.delegates.GetTableSize(m_hDB, hRet,
                    ref rows, ref cols);

                ret.Rows.Capacity = rows;



                for (int row = 0; row < rows; row++)
                {
                    Row currow = new Row(ret);
                    if (!(row == 0 && m_startRow == 1))
                    {
                        ret.Rows.Add(currow);
                    }
                    if (row == 0)
                    {
                        ret.Headers = currow;
                    }
                    for (int col = 0; col < cols; col++)
                    {
                        currow.Cols.Add(StringMarshal.Convert(
                            m_imports.delegates.GetString(
                            m_hDB, hRet, row, col)));
                    }
                }

           

                m_imports.delegates.CloseReturn(m_hDB, hRet);

            }

            return ret;
        }

        public long DatabaseSize
        {
            get
            {
                try
                {
                    long ret = 0;

                    FileInfo db = new FileInfo(m_file);

                    string dir = Path.GetDirectoryName(db.FullName);
                    string file = Path.GetFileName(db.FullName);

                    foreach (string cur in Directory.GetFiles(dir, file + "*"))
                    {
                        FileInfo fi = new FileInfo(cur);
                        ret += fi.Length;
                    }

                    return ret;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public SQL_Lite(string File)
        {
            Open(File);
        }

        public SQL_Lite()
        {
            // Nothing to do
        }

        public void Open(string File)
        {
            DbWorker = new object();
            lock (DbWorker)
            {
#if DEBUG
                string path = Environment.GetEnvironmentVariable("PATH");
                path += @";..\..\Scott's News\bin\Debug";
                path += @";..\..\..\Scott's News\bin\Debug";
                path += @";..\..\..\..\Scott's News\bin\Debug";
                Environment.SetEnvironmentVariable("PATH", path);
#endif

                bool use64 = false;

                if (IntPtr.Size == 8)
                {
                    use64 = true;
                }

                if (!use64)
                {
                    try
                    {
                        m_imports = new Imports32();
                        m_imports.GetAllExports();
                        using (StringMarshal sm = new StringMarshal(File))
                        {
                            m_hDB = m_imports.delegates.OpenDatabase(sm.Native);
                        }
                        m_file = File;
                    }
                    catch (BadImageFormatException)
                    {
                        use64 = true;
                    }
                }

                if (use64)
                {
                    m_imports = new Imports64();
                    m_imports.GetAllExports();
                    using (StringMarshal sm = new StringMarshal(File))
                    {
                        m_hDB = m_imports.delegates.OpenDatabase(sm.Native);
                    }
                    m_file = File;
                }
            }
        }

        ~SQL_Lite()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!this.m_disposed)
            {
                if (disposing)
                {
                    // Deal with managed resources
                }

                if (m_hDB != IntPtr.Zero)
                {
                    m_imports.delegates.CloseDatabase(m_hDB);
                    m_hDB = IntPtr.Zero;
                }
            }

            m_disposed = true;
        }
    }
}
