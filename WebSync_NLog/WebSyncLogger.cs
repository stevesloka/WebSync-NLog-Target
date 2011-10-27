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
            
            if (logMessage.StartsWith("[c:"))
            {
                string[] splitMsg = logMessage.Split(new char[] { '|' }, 1);

                WebSyncChannel = splitMsg[0].Remove(0, 3); //remove the "[:c" from the beginning of message
                //TODO logEvent.FormattedMessage = splitMsg[1].Remove(0, WebSyncChannel.Length); //remove the channel name from the beginning of message
            }

            logMessage = this.Layout.Render(logEvent);

            PublishMessageToWebSync(logEvent.FormattedMessage, WebSyncChannel);
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
