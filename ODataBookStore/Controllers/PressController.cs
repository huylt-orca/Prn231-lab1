using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using ODataBookStore.Models;
using System.Drawing.Printing;

namespace ODataBookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PressController : ControllerBase
    {
        private MyDBContext db;
        public PressController(MyDBContext db)
        {
            this.db = db;
        }

        [EnableQuery(PageSize = 10)]
        [HttpGet]
        public async Task<ActionResult> GetPress()
        {
            try
            {
                return Ok(db.Presss);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
