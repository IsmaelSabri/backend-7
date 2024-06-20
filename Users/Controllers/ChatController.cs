using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Collections;
using Users.Collections.Impl;
using Users.Dto;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        /*private readonly IChatCollection xd = new ChatCollection();

        public ChatController(IChatCollection chatCollection)
        {
            xd = chatCollection;
        }

        [HttpPost("register-user")]
        public IActionResult RegisterUser(UserDto model)
        {
            if (!string.IsNullOrEmpty(model.Firstname))
            {
                if (xd.AddUserToList(model.Firstname))
                {
                    return NoContent();
                }
            }
            return BadRequest("Escoja otro nombre");
        }*/
    }
}