using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CombatManager.LocalService
{
    class BinaryResourceManager
    {
        static BinaryResourceManager manager;

        public static BinaryResourceManager Manager
        {
            get
            {
                if (manager == null)
                {
                    manager = new BinaryResourceManager();
                }
                return manager;

            }
            
        }

        public  string[] GetResourceNames()
        {
            var asm = Assembly.GetEntryAssembly();
            string resName = asm.GetName().Name + ".g.resources";
            using (var stream = asm.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                return reader.Cast<DictionaryEntry>().Select(entry => (string)entry.Key
             ).ToArray();
            }
        }

        public  UnmanagedMemoryStream FindResource(string resource)
        {
            var asm = Assembly.GetEntryAssembly();
            string resName = asm.GetName().Name + ".g.resources";
            using (var stream = asm.GetManifestResourceStream(resName))
            using (var reader = new System.Resources.ResourceReader(stream))
            {
                try
                {
                    return (UnmanagedMemoryStream)reader.Cast<DictionaryEntry>().First(entry => (string)entry.Key == resource).Value;
                }
                catch (InvalidOperationException)
                {
                    return null;
                }

            }
        }
    }
}
