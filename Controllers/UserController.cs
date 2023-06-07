using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Repositories;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private IUserCollection db = new UserCollection();

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            return Ok(await db.GetAllUsers());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails(string id)
        {
            return Ok(await db.GetUserById(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Name == null)
            {
                ModelState.AddModelError("Name", "Nombre de usuario no encontrado");
            }
            await db.NewUser(user);
            return Created("Created", true);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser([FromBody] User user, string id)
        {
            if (user == null)
            {
                return BadRequest();
            }
            if (user.Name == string.Empty)
            {
                ModelState.AddModelError("Name", "Nombre de usuario no encontrado");
            }
            user.Id=new MongoDB.Bson.ObjectId(id);
            await db.UpdateUser(user);
            return Created("Modified", true);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id){
            await db.DeleteUser(id);
            return NoContent(); // success
        }
    }
}