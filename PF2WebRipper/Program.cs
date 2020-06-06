using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PF2WebRipper
{
    class Program
    {
        static void Main(string[] args)
        {
            PF2WebRipper ripper = new PF2WebRipper();
            var t = ripper.GetMonsters();
            t.Wait() ;
            var monsterList = t.Result;
            Console.WriteLine("Monster Page Count: " + monsterList.Count);

            ripper.BuildMatchText().SpitFile("matchtext.txt");
            ripper.BuildMatchText().SpitJSRegex("matchfix.txt");


            var v = new List<PF2WebMonsterInfo>();
            for (int i = 1; i < monsterList.Count;  i++)
            {
                var t2 = ripper.GetInfo(monsterList[i]);
                t2.Wait();
                //Console.WriteLine("M: " + l[i].Name);
                foreach (var ff in t2.Result)
                {
                   Console.WriteLine(ff.Name);
                }
                if (t2.Result.Count == 0)
                {
                    Console.WriteLine("XX " + monsterList[i].Name);
                }
                v.AddRange(t2.Result);
            }

            TheCurrentKeyWaitRoutine();
        }

        static void TheCurrentKeyWaitRoutine()
        {
            Console.WriteLine("PRESS ANY KEY");
            for (int i = 0; i < 400; i++)
            {
                if (Console.KeyAvailable)
                {
                    break;
                }
                Thread.Sleep(10);
            }

        }
    }
}
