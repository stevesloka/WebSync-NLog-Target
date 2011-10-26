using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Targets;
using FM.WebSync.Core;

namespace SteveSloka.WebSyncNLogTarget
{
    [Target("WebSyncLogger")]
    public sealed class WebSyncLogger : TargetWithLayout
    {
        public WebSyncLogger()
        {
            this.WebSyncURL = "http://localhost";
        }

        public string WebSyncURL { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = this.Layout.Render(logEvent);

            PublishMessageToWebSync(logMessage, "/GenuineChannels");  //TODO
        }

        public void PublishMessageToWebSync(string message, string channel)
        {
            //verify WebSyncURL ends with "/"
            if (!WebSyncURL.EndsWith("/")) WebSyncURL += "/";

            Publisher publisher = new Publisher(new PublisherArgs
            {
                RequestUrl = this.WebSyncURL + "request.ashx"
            });

            var publication = publisher.Publish(new Publication
             {
                 Channel = channel,
                 DataJson = JSON.Serialize(new Payload { Text = message, Source = "NLog", Time = DateTime.Now })
             });

            if (publication.Successful == false)
            {
                Console.WriteLine("Could not publish: " + publication.Error);
            }
        }
    }
}
