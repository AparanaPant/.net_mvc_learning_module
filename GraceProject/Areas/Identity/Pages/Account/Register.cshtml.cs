// Licensed to the .NET Foundation
// one or more agreements.
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

       
        [BindProperty]
        public InputModel Input { get; set; }

        [BindProperty]
        public List<string> SelectedCourses { get; set; } = new List<string>();

        public string ReturnUrl { get; set; }

       
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        

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

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Display(Name = "Selected Courses")]
            public List<string> SelectedCourses { get; set; } = new List<string>();


            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Display(Name = "School")]
            public string SchoolName { get; set; }
            public string UserType { get; set; } 
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
            ViewData["Courses"] = courses ?? new List<Course>();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (!ModelState.IsValid) return Page();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                _logger.LogInformation("User created successfully.");

                // Send email confirmation
                //await SendEmailConfirmation(user, userId, returnUrl);

                if (Input.SchoolID == null && !string.IsNullOrWhiteSpace(Input.NewSchoolName))
                {
                    Input.SchoolID = await SaveNewSchool();
                }

                switch (Input.UserType.ToUpper())
                {
                    case "STUDENT":
                        await HandleStudentRegistration(user);
                        break;
                    case "EDUCATOR":
                        await HandleEducatorRegistration(user);
                        break;
                    case "ADMIN":
                        await HandleAdminRegistration(user);
                        break;
                }

                await AssignUserRole(Input.UserType, user);
                await SaveUserAddress(user.Id);

                await transaction.CommitAsync(); // Commit transaction if everything succeeds
                _logger.LogInformation("Transaction committed.");

                if (_userManager.Options.SignIn.RequireConfirmedAccount)
                {
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl });
                }
                else
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Rollback transaction on failure
                _logger.LogError($"Transaction rolled back: {ex.Message}");
                ModelState.AddModelError(string.Empty, "An error occurred while processing your registration.");
                return Page();
            }
        }

        private async Task HandleStudentRegistration(ApplicationUser user)
        {
            if (Input.SchoolID.HasValue)
            {
                _context.UserSchools.Add(new UserSchool { UserID = user.Id, SchoolID = Input.SchoolID.Value });
            }

            await _context.SaveChangesAsync();
        }

        private async Task HandleEducatorRegistration(ApplicationUser user)
        {
            if (Input.SchoolID.HasValue)
            {
                _context.UserSchools.Add(new UserSchool { UserID = user.Id, SchoolID = Input.SchoolID.Value });
            }

            if (Input.SelectedCourses != null && Input.SelectedCourses.Any())
            {
                foreach (var courseId in Input.SelectedCourses)
                {
                    var newSession = new Session
                    {
                        CourseID = courseId,
                        StartDate = DateTime.UtcNow,
                        EndDate = DateTime.UtcNow.AddMonths(6),
                    };

                    _context.Session.Add(newSession);
                    await _context.SaveChangesAsync();


                    var educatorSession = new EducatorSession
                    {
                        EducatorID = user.Id,
                        SessionID = newSession.SessionID
                    };

                    _context.EducatorSession.Add(educatorSession);
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task HandleAdminRegistration(ApplicationUser user)
        {
            _logger.LogInformation($"Admin user {user.Email} registered.");
        }

        private async Task<int?> SaveNewSchool()
        {
            if (string.IsNullOrWhiteSpace(Input.NewSchoolName))
                return null; // No new school was provided

            var newSchool = new School
            {
                SchoolName = Input.NewSchoolName,
                Country = Input.Country,
                SchoolAddresses = new List<SchoolAddress>()
            };

            if (!string.IsNullOrWhiteSpace(Input.AddressLine1))
            {
                newSchool.SchoolAddresses.Add(new SchoolAddress
                {
                    State = Input.State,
                    AddressLine1 = Input.AddressLine1,
                    AddressLine2 = Input.AddressLine2,
                    City = Input.City,
                    ZIPCode = Input.ZIPCode
                });
            }

            _context.Schools.Add(newSchool);
            await _context.SaveChangesAsync();

            return newSchool.SchoolID; 
        }



        private async Task AssignUserRole(string userType, ApplicationUser user)
        {
            var roleExists = await _context.Roles.AnyAsync(r => r.Name == userType);
            if (!roleExists)
            {
                await _context.Roles.AddAsync(new IdentityRole(userType));
                await _context.SaveChangesAsync();
            }
            await _userManager.AddToRoleAsync(user, userType);
        }

        private async Task SaveUserAddress(string userId)
        {
            var address = new Address
            {
                Country = Input.Country ?? "USA",
                StreetAddress = Input.StreetAddress,
                City = Input.City,
                State = Input.State,
                ZIPCode = Input.ZIPCode,
                UserId = userId
            };
            _context.Address.Add(address);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> SendEmailAsync(string email, string subject, string confirmLink)
        {
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
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential("noreplygrace7@gmail.com", "wmgj cbrc ryhs wjmw");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.Send(message);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("this is exception" + e);
                return false;
            }
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
        //        return true;
        //    }
        //    catch (Exception e)
        //    {
     
        //        return false;
        //    }
        //}

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
