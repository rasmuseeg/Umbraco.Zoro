using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zoro.Application.Notifications.MailMessages
{
    public interface IEmailButton
    {
        string Url { get; set; }
        string Label { get; set; }
    }
}
