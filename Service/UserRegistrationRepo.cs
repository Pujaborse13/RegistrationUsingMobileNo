using System;
using System.Collections.Generic;
using System.Text;
using CommonLayer.Models;
using System.Threading.Tasks;
using RepositoryLayer.Interface;
using RepositoryLayer.Context;
using RepositoryLayer.Migrations;
using System.Linq;
using Twilio;
using RepositoryLayer.Entity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Twilio.Rest.Verify.V2.Service;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RepositoryLayer.Service
{
    public class UserRegistrationRepo : IUserRegistrationRepo
    {

        private readonly FundooDBContext fcontext;
        private readonly IConfiguration configuration;
        private readonly string verifyServiceSid;



        public UserRegistrationRepo(FundooDBContext fcontext, IConfiguration configuration)
        {
            this.fcontext = fcontext;
            this.configuration = configuration;
            TwilioClient.Init(configuration["Twilio:AccountSID"], configuration["Twilio:AuthToken"]);

            verifyServiceSid = configuration["Twilio:VerifyServiceSID"];

        }


        public async Task<string> RegisterUserUsingMobileNo(string mobileNo)

        {
            var userExits = fcontext.UserRegistration.FirstOrDefault(u => u.MobileNo == mobileNo);

            if (userExits == null)
            {
                UserRegistrationEntity user = new UserRegistrationEntity()
                {
                    MobileNo = mobileNo
                };

                fcontext.UserRegistration.Add(user);
                await fcontext.SaveChangesAsync();
                return "User registered successfully using mobile no";

            }

            else
            {
                return "Exists";
            }

        }


        //Send OTP 
        public async Task<bool> SendOTP(string mobileNo)
        {
            var userExits = fcontext.UserRegistration.FirstOrDefault(u => u.MobileNo == mobileNo);

            if (userExits == null)
            { 
                return false;
            }

            var result = await VerificationResource.CreateAsync(
            to: $"+91{mobileNo}",
            channel: "sms",
            pathServiceSid: verifyServiceSid
        );
            return result.Status == "pending";


        }

        // OTP verification
        public async Task<bool> VerifyOTP(string mobileNo, string otp)
        {
            var checkotp = await VerificationCheckResource.CreateAsync(
                            to: $"+91{mobileNo}",
                            code: otp,
                            pathServiceSid: verifyServiceSid
             );

            return checkotp.Status == "approved";

        }

       








    }



}



