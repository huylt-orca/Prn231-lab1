using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using ODataBookStore.Models;
using System.Drawing.Printing;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ODataBookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private MyDBContext db ;
        // GET: api/Books

        public BookController (MyDBContext db)
        {
            this.db = db;
        }

        [EnableQuery(PageSize = 3)]
        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetBooks()
        {
            try
            {
                return Ok(db.Books);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpGet("total")]
        [Authorize]
        public async Task<ActionResult> GetTotalBooks(string? value ="")
        {
            try
            {
                value = value ?? "";
                return Ok(db.Books.Where( b=> 
                    b.ISBN.Contains(value) || b.Title.Contains(value) || b.Author.Contains(value)
                    ).Count());
            }
            catch
            {
                return BadRequest();
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetBook(int id)
        {
            try
            {
                var book = await db.Books.Include(b => b.Press).FirstOrDefaultAsync(c => c.Id == id);
                book.Press.Books = null;
                if (book == null)
                {
                    return NotFound();
                }
                return Ok(book);
            }
            catch
            {
                return BadRequest();
            }
        }


        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles="admin")]
        public async Task<IActionResult> PutBook(int id, Book bookModel)
        {
            try
            {
                //Book book = db.Books.FirstOrDefault(c => c.Id == id);
                //if (book == null)
                //{
                //    return NotFound();
                //}
                db.Books.Update(bookModel);
                db.SaveChanges();
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult> PostBook(Book bookModel)
        {
            try
            {
                db.Books.Add(bookModel);
                db.SaveChanges();
                return Ok(bookModel);
            }
            catch
            {
                return BadRequest();
            }
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                Book book = db.Books.FirstOrDefault(c => c.Id == id);
                if(book == null)
                {
                    return NotFound();
                }

                db.Books.Remove(book);
                db.SaveChanges(true);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}
