using CampusConnect.Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace CampusConnect.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailConfirmationAsync(string email, string confirmationLink)
    {
        var subject = "Confirmă-ți adresa de email - CampusConnect";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #f9f9f9; padding: 30px; border-radius: 10px;'>
                    <h2 style='color: #333;'>Bun venit la CampusConnect!</h2>
                    <p style='color: #666; font-size: 16px;'>
                        Mulțumim că te-ai înregistrat. Pentru a-ți activa contul, te rugăm să confirmi adresa de email.
                    </p>
                    <div style='margin: 30px 0;'>
                        <a href='{confirmationLink}' 
                           style='background-color: #4CAF50; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Confirmă Email
                        </a>
                    </div>
                    <p style='color: #999; font-size: 14px;'>
                        Dacă nu ai creat acest cont, poți ignora acest email.
                    </p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        Sau copiază și lipește acest link în browser:<br/>
                        <span style='word-break: break-all;'>{confirmationLink}</span>
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    public async Task SendPasswordResetAsync(string email, string resetLink)
    {
        var subject = "Resetare parolă - CampusConnect";
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; padding: 20px;'>
                <div style='max-width: 600px; margin: 0 auto; background-color: #f9f9f9; padding: 30px; border-radius: 10px;'>
                    <h2 style='color: #333;'>Resetare Parolă</h2>
                    <p style='color: #666; font-size: 16px;'>
                        Am primit o cerere de resetare a parolei pentru contul tău.
                    </p>
                    <div style='margin: 30px 0;'>
                        <a href='{resetLink}' 
                           style='background-color: #2196F3; color: white; padding: 15px 30px; text-decoration: none; border-radius: 5px; display: inline-block;'>
                            Resetează Parola
                        </a>
                    </div>
                    <p style='color: #999; font-size: 14px;'>
                        Dacă nu ai solicitat resetarea parolei, poți ignora acest email.
                    </p>
                    <p style='color: #999; font-size: 12px; margin-top: 30px;'>
                        Link-ul expiră în 1 oră.<br/>
                        <span style='word-break: break-all;'>{resetLink}</span>
                    </p>
                </div>
            </body>
            </html>
        ";

        await SendEmailAsync(email, subject, body);
    }

    private async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        
        // Sender
        var senderEmail = _configuration["EmailSettings:SenderEmail"];
        var senderName = _configuration["EmailSettings:SenderName"];
        message.From.Add(new MailboxAddress(senderName, senderEmail));

        // Recipient
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = subject;

        // Body
        var bodyBuilder = new BodyBuilder
        {
            HtmlBody = htmlBody
        };
        message.Body = bodyBuilder.ToMessageBody();

        // Send
        using var client = new SmtpClient();
        try
        {
            var smtpServer = _configuration["EmailSettings:SmtpServer"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            var username = _configuration["EmailSettings:Username"];
            var password = _configuration["EmailSettings:Password"];

            await client.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
            
            // Authenticate only if credentials are provided
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                await client.AuthenticateAsync(username, password);
            }

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            // Log error (în producție ar trebui să folosești un logger)
            Console.WriteLine($"Error sending email: {ex.Message}");
            throw;
        }
    }
}
