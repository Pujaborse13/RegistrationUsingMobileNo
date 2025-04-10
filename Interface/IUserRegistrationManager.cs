using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Migrations;
using System.Threading.Tasks;
using CommonLayer.Models;

namespace ManagerLayer.Interface
{
    public interface IUserRegistrationManager
    {
        public Task<string> RegisterUserUsingMobileNo(string mobileNo);
        public Task<bool> SendOTP(string mobileNo);

        public Task<bool> VerifyOTP(string mobileNo, string otp);





    }
}
