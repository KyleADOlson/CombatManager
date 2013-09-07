using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CombatManager
{
    class Program
    {

        static Mutex mutex = new Mutex(true, "{58405CB6-B225-445B-8109-0ABBB6914F3C}");

        [STAThread]
        public static void Main(string[] args)
        {
            if (!mutex.WaitOne(TimeSpan.Zero, true))
            {

            }
            else
            {

                App app = new App();
                app.InitializeComponent();
                app.Run();

            }
        }
    }
}
