using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AccountController(DataContext context, ITokenService tokenService) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>>Register(RegisterDTO registerDto){
            if(await UserExists(registerDto.UserName)) return BadRequest("Username is taken");
            using var hmac = new HMACSHA512();
            var newUser = new AppUser(){
                UserName = registerDto.UserName.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key
            };
            context.Users.Add(newUser);
            await context.SaveChangesAsync();
            return new UserDTO(){
                Username = newUser.UserName,
                token = tokenService.CreateToken(newUser)
            };
        }

        private async Task<bool> UserExists(string username){
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
        
    

    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDto){
         var user = await context.Users.FirstOrDefaultAsync(x=> x.UserName.ToLower() == loginDto.UserName.ToLower());
         if(user == null) return Unauthorized("Invalid username");

         using var hmac = new HMACSHA512(user.PasswordSalt);
         var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

         for(int i=0; i<computedHash.Length; i++){
            if(computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid password");
         }

         return new UserDTO(){
            Username = user.UserName,
            token = tokenService.CreateToken(user)
         };
    }
}
}