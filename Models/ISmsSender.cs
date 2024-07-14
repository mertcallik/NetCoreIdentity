namespace NetIdentityApp.Models
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
