namespace Fanex.NAP.SPU.VR.Features.Notification
{    
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public class NotificationService
    {
        private MessageModel _messageModel;
        private string _token;
        private string _groupId;
        private string _apiUrl;
        private string _proxy;

        private static DateTimeOffset LastTime { get; set; }

        private static string LastMessage { get; set; }

        private static int Counter { get; set; }

        public NotificationService(string environmentString)
        {
            _proxy = new HashSet<string> { "PRO", "KLDR" }.Contains(environmentString) ? "iusqd.gamora.prod:8801" : null;
        }

        public NotificationService Message(MessageModel messageModel)
        {
            if (messageModel != null)
            {
                _messageModel = messageModel;
            }
            else
            {
                throw new ArgumentException("Parameter cannot be null", nameof(messageModel));
            }

            return this;
        }

        /// <summary>
        /// Config telegram api setting.
        /// </summary>     
        public NotificationService Config(string groupId)
        {
            _token = "0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9";
            _groupId = groupId;
            _apiUrl = "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken";                      

            if (string.IsNullOrWhiteSpace(_groupId))
            {
                throw new ArgumentException("Group Id cannot be null", nameof(groupId));
            }
            
            return this;
        }

        /// <summary>
        /// Send message with delay in second
        /// </summary>
        /// <param name="delayInSecond">Delay time in second</param>
        /// <param name="isInfo">The type of message(info or error)</param>
        public void SendWithDelay(int delayInSecond = 0, bool isInfo = false)
        {
            if (delayInSecond != 0)
            {
                var second = (DateTimeOffset.Now - LastTime).TotalSeconds;

                if (second >= delayInSecond)
                {
                    if (_messageModel.Message != LastMessage)
                    {
                        Counter = 0;
                    }

                    if (Counter == 0 || (_messageModel.Message == LastMessage && Counter < 3))
                    {
                        var result = isInfo ? SendMessageInfo() : SendMessageError();

                        if (result)
                        {
                            LastTime = DateTimeOffset.Now;
                            LastMessage = _messageModel.Message;
                            Counter++;

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Send message without delay time
        /// </summary>
        /// <param name="isInfo">Type of message(info or error)</param>
        public void SendWithoutDelay(bool isInfo = false)
        {
            if (isInfo)
            {
                SendMessageInfo();
            }
            else
            {
                SendMessageError();
            }
        }

        private bool SendMessageError()
        {
            var message = new StringBuilder();

            message.Append("\U0001F525 ERROR \U0001F525 in `[`**" + _messageModel.SystemName + "**`]`");
            message.Append("\n\r");
            message.Append("**Environment**: ");
            message.Append(_messageModel.Environment);
            message.Append("\n\r");
            message.Append("**Timestamp(GMT%2B7)**: ");
            message.Append(_messageModel.Timestamp);
            message.Append("\n\r");
            message.Append("**Message**: ");
            message.Append(_messageModel.Message);

            if (!string.IsNullOrWhiteSpace(_messageModel.MoreInfo))
            {
                message.Append("\n\r");
                message.Append("**More Info**: ");
                message.Append(_messageModel.MoreInfo);
            }

            var client = new RestClient($"{_apiUrl}?token={_token}&message={message.ToString()}&conversationId={_groupId}");

            if (!string.IsNullOrEmpty(_proxy))
            {
                var proxy = new WebProxy(_proxy);
                client.Proxy = proxy;
            }

            var request = new RestRequest(Method.POST);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.Execute(request).StatusCode == System.Net.HttpStatusCode.OK;
        }

        private bool SendMessageInfo()
        {
            var message = new StringBuilder();

            message.Append("✅ INFO ✅ in `[`**" + _messageModel.SystemName + "**`]`");
            message.Append("\n\r");
            message.Append("**Environment**: ");
            message.Append(_messageModel.Environment);
            message.Append("\n\r");
            message.Append("**Timestamp(GMT%2B7)**: ");
            message.Append(_messageModel.Timestamp);
            message.Append("\n\r");
            message.Append("**Message**: ");
            message.Append(_messageModel.Message);

            if (!string.IsNullOrWhiteSpace(_messageModel.MoreInfo))
            {
                message.Append("\n\r");
                message.Append("**More Info**: ");
                message.Append(_messageModel.MoreInfo);
            }
         
            var client = new RestClient($"{_apiUrl}?token={_token}&message={message.ToString()}&conversationId={_groupId}");

            if (!string.IsNullOrEmpty(_proxy))
            {
                var proxy = new WebProxy(_proxy);
                client.Proxy = proxy;
            }

            var request = new RestRequest(Method.POST);

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            return client.Execute(request).StatusCode == System.Net.HttpStatusCode.OK;
        }

    }
}
