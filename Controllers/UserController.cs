using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    [Route("api/[controller]")]
    public class UserController(DataContext context) : ControllerBase
    {
       [HttpGet]
       public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers(){
           var users = await context.Users.ToListAsync();
           if(users == null) return NotFound();
           return users;
       }
       
       [HttpGet("{id:int}")]
       public ActionResult<AppUser> GetUser(int id){
           var user = context.Users.Find(id);
           if(user == null) return NotFound();
           return user;
       }

       
      
        
    }
}