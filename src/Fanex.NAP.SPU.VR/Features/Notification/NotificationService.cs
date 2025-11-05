namespace Fanex.NAP.SPU.VR.Features.Notification
{    
    using RestSharp;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Text;

    public interface INotificationService
    {
        INotificationService Message(MessageModel messageModel);

        INotificationService Config(string groupId, string environmentString);

        bool SendMessageError();

        bool SendMessageInfo();
    }

    public class NotificationService : INotificationService
    {
        private MessageModel _messageModel;
        private string _token;
        private string _groupId;
        private string _apiUrl;
        private string _proxy;              

        public INotificationService Message(MessageModel messageModel)
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
        public INotificationService Config(string groupId, string environmentString)
        {
            _token = "0A40aOc7CtpzjbBSJu8KNWQRoXyjRvf9";
            _groupId = groupId;
            _apiUrl = "https://bot.nexdev.net/skynex/api/messages/forwardwithtoken";
            _proxy = new HashSet<string> { "PRO", "KLDR" }.Contains(environmentString) ? "iusqd.gamora.prod:8801" : null;

            if (string.IsNullOrWhiteSpace(_groupId))
            {
                throw new ArgumentException("Group Id cannot be null", nameof(groupId));
            }
            
            return this;
        }              

        public bool SendMessageError()
        {
            var message = new StringBuilder();

            message.Append("\U0001F525 ERROR \U0001F525 in `[`**" + _messageModel.SystemName + "**`]`");

            return SendMessage(message);
        }

        public bool SendMessageInfo()
        {
            var message = new StringBuilder();

            message.Append("✅ INFO ✅ in `[`**" + _messageModel.SystemName + "**`]`");

            return SendMessage(message);
        }

        private bool SendMessage(StringBuilder message)
        {
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
