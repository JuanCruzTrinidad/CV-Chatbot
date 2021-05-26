using System.Threading.Tasks;

namespace CV_Chatbot.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string title, string body);
    }
}
