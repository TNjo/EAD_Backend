using System.Net;
using System.Net.Mail;

namespace BackendServices.Services;

public class EmailService
{
    private readonly SmtpClient client;

    public EmailService()
    {
        // Configure the SMTP client with your Mailtrap settings
        client = new SmtpClient("bulk.smtp.mailtrap.io", 587)
        {
            Credentials = new NetworkCredential("api", "********ff98"),
            EnableSsl = true
        };
    }

    public void SendEmail()
    {
        try
        {
            // var mailMessage = new MailMessage
            // {
            //     From = new MailAddress("hello@demomailtrap.com"), // Set the sender's email address
            //     Subject = subject,
            //     Body = body,
            //     IsBodyHtml = true // Set to true if you are sending HTML content
            // };
            //
            // mailMessage.To.Add(toEmail);
            
            client.Send("hello@demomailtrap.com", "tharuneo37@gmail.com", "Hello world", "testbody");
            Console.WriteLine("Email sent successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending email: {ex.Message}");
        }
    }
}