using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EmbedIO;

namespace CombatManager.LocalService
{
    class ImageServer : WebModuleBase
    {
        public ImageServer() : base("/img/")
        {

        }




        public override bool IsFinalHandler => true;

        protected override Task OnRequestAsync(IHttpContext context)
        {
            Match m = Regex.Match(context.RequestedPath, "\\A/(?<type>[a-zA-Z]+)/(?<name>[-@_a-zA-Z0-9]+(\\.(?<ext>(png)|(jpg)|(jpeg)))?)\\Z");

            if (m != null)
            {
                string type = m.Value("type");
                string name = m.Value("name");

                if (type == "icon")
                {
                    var s = GetIcon16(name);
                    if (s != null)
                    {
                        SendUnmanagedStream(context, "images/png", s);

                    }
                }
                if (type == "image")
                {
                    string ext = m.Value("ext");
                    if (ext != null)
                    {
                        SendUnmanagedStream(context, "image/" + 
                            ((ext == "png")?"png":"jpeg"), GetImage(name));


                        
                    }

                }


            }


            return Task.CompletedTask;
        }

        private void SendUnmanagedStream(IHttpContext context, String contentType, Stream umStream)
        {
            context.Response.ContentType = contentType;
            context.Response.StatusCode = 200;


            using (Stream outStream = context.OpenResponseStream())
            {
                umStream.CopyTo(outStream);
            }
        }


        private Stream GetIcon16(string icon)
        {
            return BinaryResourceManager.Manager.FindResource(GetIcon16Name(icon));
        }

        private Stream GetImage(string image)
        {
            return BinaryResourceManager.Manager.FindResource("images/" + image);

        }

        private string GetIcon16Name(string icon)
        {
            return "images/" + icon + "-16.png";
        }



    }
}
