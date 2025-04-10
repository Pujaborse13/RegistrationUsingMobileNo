using System.Threading.Tasks;
using CommonLayer.Models;
using ManagerLayer.Interface;
using Microsoft.AspNetCore.Mvc;
using RepositoryLayer.Entity;

namespace FundooNotesApp.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UserRegistrationController : Controller
    {
        private readonly IUserRegistrationManager userRegistrationManager;

        public UserRegistrationController(IUserRegistrationManager userRegistrationManager)
        { 
            this.userRegistrationManager = userRegistrationManager;
        }

        [HttpPost]
        [Route("UserRegistrationUsingMobileNo")]
        public  async Task<IActionResult> UserRegistration(string mobileNo)
        {
            var result = await userRegistrationManager.RegisterUserUsingMobileNo(mobileNo);


            if (result == "Exists")
            {
                return BadRequest(new ResponseModel<string> { Success = false, Message = "Mobile No already Exists " });
            }
            else {
               
                 return Ok(new ResponseModel<string> { Success = true, Message = "user Register Successfully" , Data = result });

            }


        }

        [HttpPost]
        [Route("SendOTP")]

        public async Task<IActionResult> SendOtp(string mobileNo)
        {

            var success = await userRegistrationManager.SendOTP(mobileNo);
            if (success)
            {
                return Ok(new { message = "OTP sent successfully" });

            }
            else
            {
                return BadRequest(new { message = "Failed to send OTP" });
            }
        }


        [HttpPost]
        [Route("OTPVerification")]
        public async Task<IActionResult> VerifyOTP(string mobileNo , string otp)
        { 
            var isVerified = await userRegistrationManager.VerifyOTP(mobileNo, otp);

            if (isVerified)
            {
                return Ok(new { Mesaage = "OTP verified Sucessfully" });

            }
            else {
                return BadRequest(new { message = "fail to verify OTP ! Please enter valid otp" });


            }




        }




    }
}
