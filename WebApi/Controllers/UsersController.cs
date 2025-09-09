using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }
        [HttpGet]
        public ActionResult<IEnumerable<User>> Get()
        {
            return _context.Users.ToList();
        }
        [HttpGet("{id}", Name = "GetUser")]
        public ActionResult<User> Get(Guid id)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("Usuário não encontrado");
            }
            return user;
        }
        [HttpPost]
        public ActionResult Post(User user)
        {
            if (user == null)
            {
                return BadRequest();
                _context.Users.Add(user);
                _context.SaveChanges();

                return new CreatedAtRouteResult("GetUser", new { id = user.Id }, user);
            }
        }

    }
}
