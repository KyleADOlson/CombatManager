using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PF2WebRipper
{
    public static class ConsoleHelper
    {
        public class Banner
        {
            public Banner() { }

            public Banner(int x, int y, string text)
            {
                X = x;
                Y = y;
                Text = text;
            }

            public int X { get; set; }
            public int Y { get; set; }


            public string Text { get; set; }

        }

        static Dictionary<int, Banner> banners = new Dictionary<int, Banner>();



        static object consoleLock = new object();

        public static void WriteLine(string text)
        {
            lock (consoleLock)
            {
                Console.WriteLine(text);
            }
            DrawBanners();

        }

        public static void Write(string text)
        {
            lock (consoleLock)
            {
                Console.Write(text);
            }
            DrawBanners();

        }

        public static void WriteXY(int x, int y, string text)
        {
            lock (consoleLock)
            {
                int ox = Console.CursorLeft;
                int oy = Console.CursorTop;

                Console.SetCursorPosition(x, y);

                Console.Write(text);

                Console.SetCursorPosition(ox, oy);

            }
        }

        public static void SetBanner(int id, Banner banner)

        {
            lock(consoleLock)
            {
                banners[id] = banner;
            }
            DrawBanners();
        }


        public static void RemoveBanner(int id)

        {
            lock(consoleLock)
            {
                banners.Remove(id);
            }
        }

        static void DrawBanners()

        {
            List<Banner> copies = new List<Banner>();
            lock(consoleLock)
            {
                foreach (var kv in banners)
                {
                    Banner b = kv.Value;
                    copies.Add(new Banner() { X = b.X, Y = b.Y, Text = b.Text });
                }
            }
            foreach (Banner b in copies)
            {
                DrawBanner(b);
            }
        }

        static void DrawBanner(Banner b)
        {
            WriteXY(b.X, b.Y, b.Text);
        }

    }
}
