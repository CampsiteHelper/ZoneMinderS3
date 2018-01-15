using System;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace ZMS3Sync
{
    public class SendSESEmail
    {
        public SendSESEmail()
        {

        }

        private static DateTime lastSent = DateTime.Now.AddDays(-1);


        public static void sendMail(string subject, string filepath)
        {

            if ((DateTime.Now - lastSent).TotalMinutes < 5)
            {
                //don't send more than one every five minutes..
                return;
            }
            lastSent = DateTime.Now;


            // Replace sender@example.com with your "From" address. 
            // This address must be verified with Amazon SES.
            var FROM = U.config["FROM_ADDDRESS"];
            var FROMNAME = "Zone Uploader";


            // Replace recipient@example.com with a "To" address. If your account 
            // is still in the sandbox, this address must be verified.
            var TO = U.config["TO_ADDDRESS"];

            // Replace smtp_username with your Amazon SES SMTP user name.
            var SMTP_USERNAME = Environment.GetEnvironmentVariable("AWS_SES_USERNAME");


            // Replace smtp_password with your Amazon SES SMTP user name.
            var SMTP_PASSWORD = Environment.GetEnvironmentVariable("AWS_SES_PWD");


            // If you're using Amazon SES in a region other than US West (Oregon), 
            // replace email-smtp.us-west-2.amazonaws.com with the Amazon SES SMTP  
            // endpoint in the appropriate Region.
            var HOST = U.config["SMTP_SERVER_HOST"];

            // The port you will connect to on the Amazon SES SMTP endpoint. We
            // are choosing port 587 because we will use STARTTLS to encrypt
            // the connection.
            const int PORT = 587;

            if (String.IsNullOrEmpty(HOST) ||
               String.IsNullOrEmpty(SMTP_PASSWORD) ||
               String.IsNullOrEmpty(SMTP_USERNAME) ||
               String.IsNullOrEmpty(FROM) ||
               String.IsNullOrEmpty(HOST))
            {
                U.log("Email is missing configuration, not sending");
                return;

            }

            // The subject line of the email

            // The body of the email
            const String BODY = "Please find an alarm image below";

            // Create and build a new MailMessage object
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            message.To.Add(new MailAddress(TO));
            message.Subject = subject;
            message.Body = BODY;
            if (!String.IsNullOrEmpty(filepath) && File.Exists(filepath))
                message.Attachments.Add(new Attachment(filepath));

            // Create and configure a new SmtpClient
            SmtpClient client =
                new SmtpClient(HOST, PORT);
            // Pass SMTP credentials
            client.Credentials =
                new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
            // Enable SSL encryption
            client.EnableSsl = true;

            // Send the email. 
            try
            {
                U.log("Attempting to send email...");
                client.Send(message);
                U.log("Email sent!");
            }
            catch (Exception ex)
            {
                U.log("The email was not sent.");
                U.log("Error message:", ex.Message);
            }

        }

    }
}
