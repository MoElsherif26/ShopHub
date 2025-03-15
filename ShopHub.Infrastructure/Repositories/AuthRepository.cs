using Microsoft.AspNetCore.Identity;
using ShopHub.Core.DTO;
using ShopHub.Core.Entities;
using ShopHub.Core.Interfaces;
using ShopHub.Core.Services;
using ShopHub.Core.Sharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopHub.Infrastructure.Repositories
{
    public class AuthRepository: IAuth
    {
        private readonly UserManager<AppUser> userManger;
        private readonly IEmailService emailService;
        private readonly SignInManager<AppUser> signInManager;


        public AuthRepository(UserManager<AppUser> userManger, IEmailService emailService, SignInManager<AppUser> signInManager)
        {
            this.userManger = userManger;
            this.emailService = emailService;
            this.signInManager = signInManager;
        }

        public async Task<string> RegisterAsync(RegisterDTO registerDTO)
        {
            if (registerDTO == null)
            {
                return null;
            }
            if (await userManger.FindByNameAsync(registerDTO.UserName) is not null)
            {
                return "This Username is already registered";
            }
            if (await userManger.FindByEmailAsync(registerDTO.Eamil) is not null)
            {
                return "This Email is already registered";
            }
            AppUser user = new AppUser()
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Eamil
            };
            var result = await userManger.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded is not true)
            {
                return result.Errors.ToList()[0].Description;
            }
            string token =await userManger.GenerateEmailConfirmationTokenAsync(user);
            // Send active email
            await SendEmail(user.Email, token, "active", "ActiveEmail", 
                "Please active your email click on button to active");

            return "Done";
        }
        public async Task SendEmail(string email, string code, string component, 
            string subject, string message)
        {
            var result = new EmailDTO(email, "m.o.elsherif2002@gmail.com",
                subject,
                EmailStringBody.send(email,
                code, component, message));

            await emailService.SendEmail(result);
        }


        public async Task<string> LoginAsync(LoginDTO login)
        {
            if (login == null)
            {
                return null;
            }
            var finduser = await userManger.FindByEmailAsync(login.Eamil);

            if (!finduser.EmailConfirmed)
            {
                string token = await userManger.GenerateEmailConfirmationTokenAsync(finduser);
                await SendEmail(finduser.Email, token, "active", "ActiveEmail",
                "Please active your email click on button to active");
                return "Please confirm your email first, we have send activate to your email";
            }
            var result = await signInManager.CheckPasswordSignInAsync(finduser, 
                login.Password,
                true);
            if (result.Succeeded)
            {
                return "Done";

            }
            return "Please check your email and password, something went wrong";
        }

    }
}
