using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Users.Collections;
using Users.Collections.Impl;
using Users.Data;
using Users.Models;

namespace Users.Controllers
{
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageCollection imageDb;

        public ImageController(UserDb hdb)
        {
            imageDb = new ImageCollection(hdb);
        }

        [HttpPost("new")]
        public async Task<IActionResult> NewImage([FromBody] Image image)
        {
            if (image == null)
            {
                return BadRequest();
            }
            await imageDb.NewImage(image);
            return Created("Image created", true);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage([FromBody] Image image, string id)
        {
            if (image == null)
            {
                return BadRequest();
            }
            image.Id = id;
            await imageDb.UpdateImage(image);
            return Created("Modified", image);
        }


    }
}