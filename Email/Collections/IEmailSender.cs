using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Email.Dto;

namespace Email.Service
{
    public interface IEmailSender
    {
        Task SendEmailAsync(EmailDto emailDto, string action);
    }
}