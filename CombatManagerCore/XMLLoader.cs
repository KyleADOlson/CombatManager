/*
 *  XMLLoader.cs
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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;

namespace CombatManager
{

    public static class XmlListLoader<T>
    {
        public static List<T> Load(string filename)
        {
            return XmlLoader<List<T>>.Load(filename);
        }

        public static List<T> Load(string filename, bool appData)
        {
            return XmlLoader<List<T>>.Load(filename, appData);
        }

        public static void Save(List<T> list, string filename, bool appData)
        {
            XmlLoader<List<T>>.Save(list, filename, appData);
        }

        public static void Save(List<T> list, string filename)
        {
            XmlLoader<List<T>>.Save(list, filename);
        }
    }

    public class XmlLoader<T>
    {

        private static Dictionary<string, string> xmlAttributeErrors;
        private static string lastFile;

        public const string AppDataDir = "Combat Manager";


        public static T Load(string filename)
        {
            return Load(filename, false);
        }

        public static T Load(string filename, bool appData)
        {
            T set = default(T);

            lastFile = filename;
			
#if MONO
			DateTime startTime = DateTime.Now;
			DebugLogger.WriteLine("Loading [" + filename + "]");
#endif

            try
            {
                // Open document
                XmlSerializer serializer = new XmlSerializer(typeof(T));

                serializer.UnknownNode += new XmlNodeEventHandler(serializer_UnknownNode);
                serializer.UnknownAttribute += new XmlAttributeEventHandler(serializer_UnknownAttribute);


                string path;
                if (!appData)
                {
                    // A FileStream is needed to read the XML document.
                    path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                }
                else
                {
                    path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    path = Path.Combine(path, AppDataDir);

                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }

                string file = Path.Combine(path, filename);

                if (new FileInfo(file).Exists)
                {
                    using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read))
                    {

                        xmlAttributeErrors = new Dictionary<string, string>();

                        set = (T)serializer.Deserialize(fs);

                        if (xmlAttributeErrors.Count > 0)
                        {
                            DebugLogger.WriteLine("XmlListLoader: " + lastFile);
                            foreach (string st in xmlAttributeErrors.Keys)
                            {
                                DebugLogger.WriteLine(st);
                            }
                        }


                        fs.Close();
                    }
                }
            }
            catch (Exception ex)
            {

                DebugLogger.WriteLine(ex.ToString());
                if (!appData)
                {
                    throw;
                }
            }
			
#if MONO
			DebugLogger.WriteLine("Finished [" + filename + "]  Time: " + 
				(DateTime.Now - startTime).TotalSeconds.ToString() + " secs");
#endif

            return set;
        }

        public static void Save(T list, string filename, bool appData)
        {
            string path;
            if (!appData)
            {
                // A FileStream is needed to read the XML document.
                path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                path = Path.Combine(path, AppDataDir);

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }


            string file = Path.Combine(path, filename);
            Save(list, file);
        }

        public static void Save(T list, string filename)
        {
            lastFile = filename;

            XmlSerializer serializer =
                new XmlSerializer(typeof(T));

            TextWriter writer = new StreamWriter(filename);
            XmlTextWriter xmlWriter = new XmlTextWriter(writer);
            xmlWriter.Formatting = Formatting.Indented;

            serializer.Serialize(xmlWriter, list);
            writer.Close();


        }

        static void serializer_UnknownAttribute(object sender, XmlAttributeEventArgs e)
        {
            if (xmlAttributeErrors != null)
            {
                if (!xmlAttributeErrors.ContainsKey(e.Attr.Name))
                {
                    xmlAttributeErrors[e.Attr.Name] = e.Attr.Name;
                }
            }
        }

        static void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            if (xmlAttributeErrors != null)
            {
                if (!xmlAttributeErrors.ContainsKey(e.Name))
                {
                    xmlAttributeErrors[e.Name] = e.Name;
                }
            }
        }

    }
}
