// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable
using System.Net;
using System.Net.Mail;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GraceProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Serilog;

namespace GraceProject.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<ApplicationUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("/Identity/Account/ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);
                await SendEmailAsync(
                    Input.Email,
                    "Reset Password",
                    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                return RedirectToPage("/Identity/Account/ForgotPasswordConfirmation");
            }

            return Page();
        }
        //private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        //{
        //    SmtpClient smtpClient = new SmtpClient("mailrelay.auburn.edu");
        //    smtpClient.Port = 25;
        //    smtpClient.EnableSsl = false;


        //    string senderEmail = "grace@auburn.edu";

        //    // Configure the email message
        //    MailMessage message = new MailMessage
        //    {
        //        From = new MailAddress(senderEmail),
        //        Subject = subject,
        //        Body = confirmLink,
        //        IsBodyHtml = true
        //    };
        //    message.To.Add(new MailAddress(email));

        //    try
        //    {
        //        // Send the email
        //        await smtpClient.SendMailAsync(message);
        //        Console.WriteLine("Email sent successfully");
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Error("Exception occurred while sending email:$$$$$$$$$$ " + e);
        //        return false;
        //    }
        //}
        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            Console.WriteLine("I am inside @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");

            //TODO
            //INSERT YOUR OWN MAIL SERVER CREDENTIALS
            // message.From = ?
            // message.Port = ?
            // message.Host = ?
            // smtpClient.Credentials = new NetworkCredential(?Username,?Password);
            try
            {

                MailMessage message = new MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                message.From = new MailAddress("noreplygrace7@gmail.com");
                message.To.Add(email);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = confirmLink;

                smtpClient.Port = 587;
                smtpClient.Host = "smtp.gmail.com";

                Console.WriteLine("this is working #########################");
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("noreplygrace7@gmail.com", "wmgj cbrc ryhs wjmw");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("this is expetion******" + e);
                return false;
            }
        }
    }
}
