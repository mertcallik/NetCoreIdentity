using System.Net;
using System.Net.Mail;

namespace NetIdentityApp.Models.Concreate
{
    public class SmtpEmailSemder:IEmailSender
    {

        private string _host;
        private int _port;
        private bool _enableSSL;
        private string _userName;
        private string _password;
        private string _sender;

        public SmtpEmailSemder(string host,int port,bool enableSSl,string userName,string password,string sender)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSl;
            _userName = userName;
            _password = password;
            _sender = sender;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_host, _port)
            {
                Credentials = new NetworkCredential(_userName, _password),
                EnableSsl = _enableSSL
            };


           return client.SendMailAsync(new MailMessage(_sender,email,subject,message){IsBodyHtml = true});
        }
    }
}
