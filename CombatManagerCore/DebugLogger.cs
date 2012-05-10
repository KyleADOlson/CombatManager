/*
 *  DebugLogger.cs
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
