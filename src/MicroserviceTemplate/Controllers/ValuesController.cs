using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MicroserviceTemplate.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Gets all value items
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Gets one value item
        /// </summary>
        /// <param name="id">The required item id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        /// <summary>
        /// Creates a new value item
        /// </summary>
        /// <param name="value">value to be created</param>
        /// <response code="200">If the item is created successfully</response>
        /// <response code="400">If the item is null</response>   
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public void Post([FromBody] string value)
        {
        }

        /// <summary>
        /// Updates an item
        /// </summary>
        /// <param name="id">The item key to update</param>
        /// <param name="value">The new value item</param>
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        /// <summary>
        /// Deletes an item
        /// </summary>
        /// <param name="id">The item key to delete</param>
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
