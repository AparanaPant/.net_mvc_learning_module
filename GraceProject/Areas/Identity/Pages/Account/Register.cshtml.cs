// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using GraceProject.Data;
using Microsoft.EntityFrameworkCore;
using GraceProject.Models;
using System.Net.Mail;
using System.Net;
using Serilog;
using Microsoft.AspNetCore.Mvc.Rendering;
namespace GraceProject.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUserStore<ApplicationUser> _userStore;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly GraceDbContext _context;

        public RegisterModel(
            GraceDbContext context,
            UserManager<ApplicationUser> userManager,
            IUserStore<ApplicationUser> userStore,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public List<string> SelectedCourses { get; set; } = new List<string>();

        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// 

        public class InputModel
        {
            [DataType(DataType.Text)]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }


            [DataType(DataType.Text)]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Gender")]
            public Gender Gender { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "Street Address")]
            public string StreetAddress { get; set; }


            [Display(Name = "City")]
            public string City { get; set; }


            [Display(Name = "State")]
            public string State { get; set; }


            [Display(Name = "ZIP Code")]
            public string ZIPCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "School")]
            public string SchoolName { get; set; }
            public string UserType { get; set; } // "Teacher", "Student", "Guest"
            public int? SchoolID { get; set; }

            public string NewSchoolName { get; set; }
            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }

        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            var schools = await _context.Schools
                        .Select(s => new SelectListItem
                        {
                            Value = s.SchoolID.ToString(), 
                            Text = s.SchoolName
                        })
                        .ToListAsync();

            ViewData["Schools"] = new SelectList(schools, "Value", "Text");

            // Fetch courses from the database
            var courses = await _context.Course.ToListAsync();
            ViewData["Courses"] = courses;
        }

        public async Task<IActionResult> OnPostAsync(string userType, string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (Request.Form["Input.SchoolID"] == "other")
            {
                ModelState.Remove("Input.SchoolID");
            }
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.Gender = Input.Gender;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);

                    if (SelectedCourses != null && SelectedCourses.Any())
                    {
                        foreach (var courseId in SelectedCourses)
                        {
                            var enrollment = new Enrollment
                            {
                                CourseID = courseId,
                                StudentUserID = userId
                            };
                            _context.Enrollment.Add(enrollment);
                        }
                        await _context.SaveChangesAsync();
                    }
                    if (!string.IsNullOrEmpty(userType))
                    {
                        var roleExists = await _context.Roles.AnyAsync(r => r.Name == userType);
                        if (!roleExists)
                        {
                            await _context.Roles.AddAsync(new IdentityRole(userType));
                            await _context.SaveChangesAsync();
                        }

                        await _userManager.AddToRoleAsync(user, userType);
                    }

                    var address = new Address
                    {   Country = Input.Country ?? "USA",
                        StreetAddress = Input.StreetAddress,
                        City = Input.City,
                        State = Input.State,
                        ZIPCode = Input.ZIPCode,
                        UserId = userId
                    };

                    // Add the address to the AuthDbContext.
                    _context.Address.Add(address);
                    await _context.SaveChangesAsync();

                    // Add new school if 'other' is selected
                    if (Input.SchoolID == null && !string.IsNullOrWhiteSpace(Input.NewSchoolName))
                    {
                        var newSchool = new School
                        {
                            SchoolName = Input.NewSchoolName,
                            Country = Input.Country,
                            SchoolAddresses = new List<SchoolAddress>()
                        };

                        if (!string.IsNullOrWhiteSpace(Input.AddressLine1))
                        {
                            var schoolAddress = new SchoolAddress
                            {
                                State = Input.State,
                                AddressLine1 = Input.AddressLine1,
                                AddressLine2 = Input.AddressLine2,
                                City = Input.City,
                                ZIPCode = Input.ZIPCode
                            };

                            newSchool.SchoolAddresses.Add(schoolAddress);
                        }

                        _context.Schools.Add(newSchool);
                        await _context.SaveChangesAsync();

                        Input.SchoolID = newSchool.SchoolID; // Set the new school ID for the user
                    }

                    if (userType == "EDUCATOR" || userType == "STUDENT")
                    {
                        var userSchool = new UserSchool
                        {
                            UserID = user.Id,
                            SchoolID = Input.SchoolID.Value
                        };
                        _context.UserSchools.Add(userSchool);
                        await _context.SaveChangesAsync();

                    }
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                   
                    await SendEmailAsync(Input.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        //private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        //{

        //    //TODO
        //    //INSERT YOUR OWN MAIL SERVER CREDENTIALS
        //    // message.From = ?
        //    // message.Port = ?
        //    // message.Host = ?
        //    // smtpClient.Credentials = new NetworkCredential(?Username,?Password);
        //    try
        //    {

        //        MailMessage message = new MailMessage();
        //        SmtpClient smtpClient = new SmtpClient();
        //        message.From = new MailAddress("noreplygrace7@gmail.com");
        //        message.To.Add(email);
        //        message.Subject = subject;
        //        message.IsBodyHtml = true;
        //        message.Body = confirmLink;

        //        smtpClient.Port = 587;
        //        smtpClient.Host = "smtp.gmail.com";

        //        Console.WriteLine("this is working #########################");
        //        smtpClient.EnableSsl = true;
        //        smtpClient.UseDefaultCredentials = false;
        //        smtpClient.Credentials = new NetworkCredential("noreplygrace7@gmail.com", "wmgj cbrc ryhs wjmw");
        //        smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        //        smtpClient.Send(message);
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
        //        Console.WriteLine("this is expetion******" + e);
        //        return false;
        //    }
        //}

        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
            SmtpClient smtpClient = new SmtpClient("mailrelay.auburn.edu");
            smtpClient.Port = 25;
            smtpClient.EnableSsl = false;


            string senderEmail = "grace@auburn.edu";

            // Configure the email message
            MailMessage message = new MailMessage
            {
                From = new MailAddress(senderEmail),
                Subject = subject,
                Body = confirmLink,
                IsBodyHtml = true
            };
            message.To.Add(new MailAddress(email));

            try
            {
                // Send the email
                await smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception e)
            {
     
                return false;
            }
        }

        private ApplicationUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<ApplicationUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(ApplicationUser)}'. " +
                    $"Ensure that '{nameof(ApplicationUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<ApplicationUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<ApplicationUser>)_userStore;
        }
    }
}
