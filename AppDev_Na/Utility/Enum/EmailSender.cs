using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace AppDev_Na.Utility.Enum
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            return null;
        }
    }
}