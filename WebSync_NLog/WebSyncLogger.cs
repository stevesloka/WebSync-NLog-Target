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
            this.WebSyncChannel = "/GenuineChannels";
        }

        public string WebSyncURL { get; set; }
        public string WebSyncChannel { get; set; }

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = logEvent.FormattedMessage;

            logMessage = this.Layout.Render(logEvent);
            

            if (logMessage.Contains("[c:"))
            {
                int startIndex = logMessage.IndexOf("[c:", 0);
                int endIndex = logMessage.IndexOf("]", startIndex);

                WebSyncChannel = logMessage.Substring(startIndex, endIndex - startIndex);
                WebSyncChannel = WebSyncChannel.Remove(0, 3);
                logMessage = logMessage.Remove(startIndex, (endIndex - startIndex) + 2);  //remove the channel name from the beginning of message
            }

            PublishMessageToWebSync(logMessage, WebSyncChannel);
        }

        public void PublishMessageToWebSync(string message, string channel)
        {
            //verify WebSyncURL ends with "/"
            if (!WebSyncURL.EndsWith("/")) WebSyncURL += "/";

            //verify WebSyncChannel starts with "/"
            if (!WebSyncChannel.StartsWith("/")) WebSyncChannel = "/" + WebSyncChannel;

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
