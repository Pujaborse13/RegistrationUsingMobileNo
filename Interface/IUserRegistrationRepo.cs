using System;
using System.Threading.Tasks;
using CommonLayer.Models;

namespace RepositoryLayer.Interface
{
    public interface IUserRegistrationRepo 
    {

       public Task<string> RegisterUserUsingMobileNo(string mobileNo);
       public Task<bool> SendOTP(string mobileNo);
        public Task<bool> VerifyOTP(string mobileNo, string otp);




    }
}
