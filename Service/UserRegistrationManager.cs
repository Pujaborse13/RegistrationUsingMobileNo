using System;
using System.Collections.Generic;
using System.Text;
using RepositoryLayer.Migrations;
using System.Threading.Tasks;
using ManagerLayer.Interface;
using RepositoryLayer.Service;
using CommonLayer.Models;
using RepositoryLayer.Interface;

namespace ManagerLayer.Service
{
    public class UserRegistrationManager : IUserRegistrationManager
    {
        private readonly IUserRegistrationRepo userRegistrationRepo;


        public UserRegistrationManager(IUserRegistrationRepo userRegistrationRepo)
        { 
        
                this.userRegistrationRepo = userRegistrationRepo;
        
        }


        public async Task<string> RegisterUserUsingMobileNo(string mobileNo)
        { 
            return await userRegistrationRepo.RegisterUserUsingMobileNo(mobileNo);

        }

        public async Task<bool> SendOTP(string mobileNo)
        { 
        
            return await userRegistrationRepo.SendOTP(mobileNo);
        }



        public async Task<bool> VerifyOTP(string mobileNo, string otp)
        { 
            return await userRegistrationRepo.VerifyOTP(mobileNo, otp);
        
        }


    }
}
