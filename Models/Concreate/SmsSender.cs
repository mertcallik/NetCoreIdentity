using RestSharp;
using RestSharp.Authenticators;
using Twilio;
using Twilio.Rest.Verify.V2.Service;
using Twilio.Types;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace NetIdentityApp.Models.Concreate
{
    public class SmsSender : ISmsSender
    {

        private string accountSid = "";
        private string authToken = "";

        public async Task SendSmsAsync(string number, string message)
        {
            TwilioClient.Init(accountSid, authToken);
            var messageOptions = new CreateMessageOptions(
                new PhoneNumber("+905414625256"));
            messageOptions.From = new PhoneNumber("+15014944939");
            messageOptions.Body = $"Aurora'da oturum açmak için {message} kodunu kullanabilirsiniz";

            var messageSendOpt = MessageResource.Create(messageOptions);
            Console.WriteLine(messageSendOpt.Body);

        }
    }
}
