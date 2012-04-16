using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CombatManager
{
	public interface ILogTarget
	{
		void WriteLine(string line);	
	}
	
    public static class DebugLogger
    {
        private static StreamWriter _Writer;
		private static ILogTarget _LogTarget;
		
        public static void WriteLine(string line)
        {
#if DEBUG
            LoadWriter();

            System.Diagnostics.Debug.WriteLine(line);
            _Writer.WriteLine(line);
			
#endif
			
			if (_LogTarget != null)
			{
				_LogTarget.WriteLine(line);	
			}
        }

		public static ILogTarget LogTarget
		{
			get
			{
				return _LogTarget;
			}
			set
			{
				_LogTarget = value;
			}
		}
		
        public static void LoadWriter()
        {
            if (_Writer == null)
            {
                _Writer = new StreamWriter(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "log.txt"));
            }
			
        }



    }
}
