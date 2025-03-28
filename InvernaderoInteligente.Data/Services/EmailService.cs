using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using InvernaderoInteligente.Data.Interfaces;
using Microsoft.Extensions.Options;

namespace InvernaderoInteligente.Data.Services
{
    public class EmailService : IEmailService
    {

        private readonly EmailSettings _emailsettings;

        public EmailService(IOptions<EmailSettings> emailsettings)
        {
            _emailsettings = emailsettings.Value;   // Aqui estamos obteniendo las configuraciones de correo
        }

        public async Task EnviarCorreoAsync (string destinatario, string asunto, string cuerpo) 
        {
            using var cliente = new SmtpClient(_emailsettings.SmtpServer, _emailsettings.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailsettings.Email, _emailsettings.Password),
                EnableSsl = true
            };


            using var mensaje = new MailMessage
            {
                From = new MailAddress(_emailsettings.Email),
                Subject = asunto,
                Body = cuerpo
            };

            mensaje.To.Add(destinatario);


            await cliente.SendMailAsync (mensaje);

        }
    }
}
