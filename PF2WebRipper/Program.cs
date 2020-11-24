using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace PF2WebRipper
{
    class Program
    {
        string[] args;

        List<PF2WebMonsterInfo> monsterList;
        object monsterListLock = new object();
        public int infoEstimate = 0;
        public int infoDone = 0;
        public int processEstimate = 0;
        public int processDone = 0;
        public int processFail = 0;
        PF2WebRipper ripper;

        static void Main(string[] args)
        {
            Program p = new Program(args);
            p.Run();

        }

        public Program(string[] args)
        {
            this.args = args;
        }

        public void Run()
        {
            PF2WebRipper2 webRipper2 = new PF2WebRipper2();
            webRipper2.MessageCallback += (text) =>
            {
                ConsoleHelper.WriteLine(text);
            };
            webRipper2.GetPages().Wait();
            webRipper2.ParsePages().Wait();
               
            //Load().Wait();

            //FailText().SpitFile("__Failures.txt");


            TheCurrentKeyWaitRoutine();
        }

        /*string FailText()
        {
            StringBuilder res = new StringBuilder();
            foreach (PF2WebListItem item in failItems)
            {
                res.Append(item.FileName + "\r\n");
            }
            return res.ToString();
        }*/



        public void TheCurrentKeyWaitRoutine()
        {
            ConsoleHelper.WriteLine("PRESS ANY KEY");
            for (int i = 0; i < 400; i++)
            {
                if (Console.KeyAvailable)
                {
                    break;
                }
                Thread.Sleep(10);
            }

        }

        /*public async Task<bool> Load()
        {

            ripper = new PF2WebRipper();
            var infoList = await ripper.GetMonsters();

            infoEstimate = infoList.Count;
            infoDone = 0;

            ConsoleHelper.WriteLine("Monster Page Count: " + infoList.Count);

            ripper.MatchText.SpitFile("matchtext.txt");
            ripper.MatchText.SpitJSRegex("matchfix.txt");


            var infoTasks = from r in infoList select ripper.DownloadPage(r, DownloadPageDone);


            await Task.WhenAll(infoTasks.ToArray());

            lock (monsterListLock)
            {
                monsterList = new List<PF2WebMonsterInfo>();
                processEstimate = monsterList.Count;
                processDone = 0;
            }

            var parseTasks = from x in infoList select
                           ripper.GetInfo(x, MonsterDone); ;


            await Task.WhenAll(parseTasks.ToArray());

            return true;

        }

        

        void DownloadPageDone(PF2WebRipper.DownloadPageRes res)
        {
            int done = 0;
            int total = 0;
            lock (monsterListLock)
            {
                infoDone++;

                done = infoDone;
                total = infoEstimate;



                ConsoleHelper.SetBanner(30, new ConsoleHelper.Banner(0, 1, "   [ " + done + " / " + total + " ]                "));
           
            }
        }

        List<PF2WebListItem> failItems = new List<PF2WebListItem>();


        void MonsterDone(PF2WebRipper.GetInfoRes res)
        {


            if (res.Result)
            {
                ConsoleHelper.WriteLine("^ " + res.Item.Name);
            }
            else
            {
                ConsoleHelper.WriteLine("XXX " + res.Item.Name);

            }

            //ConsoleHelper.WriteLine("M: " + l[i].Name);
            foreach (var ff in res.List )
            {
                ConsoleHelper.WriteLine(ff.Name);
            }

            lock (monsterListLock)
            {
                processDone++;
                if (res.Result)
                {
                    monsterList.AddRange(res.List);
                }
                else
                {
                    processFail++;
                    failItems.Add(res.Item);

                }
                int good = processDone - processFail;
                ConsoleHelper.SetBanner(0, new ConsoleHelper.Banner(30, 1, "" + processDone + "/" + processEstimate + "    [+] " + good + " [-] " + processFail));
                        
            }
        }*/




        public List<PF2WebMonsterInfo> MonsterList
        {
            get
            {
                List<PF2WebMonsterInfo> list;
                lock (monsterListLock)
                {
                    list = monsterList.CopyOrCreate();
                }
                return list;
            }
        }


    }
}
