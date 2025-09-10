using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Context;
using WebApi.Models;

namespace WebApi.Controllers;

// TODO: usar Tasks (await/async)
// TODO: aplicar CQRS com usecases, mediator com mediatr, repository, DI e clean arc

// TODO: colocar tag de versionamento, autenticações e para swagger
[ApiController]
[Route("api/v1/users")]
public class UsersController(AppDbContext context) : ControllerBase
{
    // TODO: autenticação admin
    [HttpGet]
    public ActionResult<IEnumerable<object>> GetAll()
    {
        var users = context.Users.Select(x => new { x.Id, x.CreatedAt }).ToList();
        return Ok(users);
    }

    // TODO: autenticação admin
    [HttpGet("{id}")]
    public ActionResult<object> GetById(Guid id)
    {
        var user = context.Users.SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound("User not found.");
        }

        return Ok(new
        {
            user.Name,
            user.Email,
            user.Preferences,
            user.CreatedAt,
            user.UpdatedAt
        });
    }

    // TODO: autenticação admin
    [HttpPost]
    public ActionResult Create(User user)
    {
        if (user is null)
        {
            return BadRequest();
        }

        var userWithEmail = context.Users.SingleOrDefault(x => x.Email == user.Email);

        if (userWithEmail is not null)
        {
            return Conflict("There is already an user using this email.");
        }

        context.Users.Add(user);
        context.SaveChanges();

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { user.Id });
    }

    // TODO: autenticação admin
    [HttpPut("{id}")]
    public ActionResult Update(Guid id, User userData)
    {
        if (userData is null)
        {
            return BadRequest();
        }

        var user = context.Users.SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound("User not found.");
        }

        user.SetName(userData.Name);
        user.SetUpdateAt(DateTime.UtcNow);
        context.SaveChanges();

        // trocar pra createdaction com um getMe
        return NoContent();
    }

    // TODO: autenticação com step register 
    [HttpPost("register")]
    public ActionResult Register(User user)
    {
        if (user is null)
        {
            return BadRequest();
        }

        var userWithEmail = context.Users.SingleOrDefault(x => x.Email == user.Email);

        if (userWithEmail is not null)
        {
            return Conflict("There is already an user using this email.");
        }

        context.Users.Add(user);
        context.SaveChanges();

        return Created();
    }

    // TODO: autenticação usuario
    [HttpPut("me/preferences")]
    public ActionResult SetPreferences(UserPreference preferences)
    {
        if (preferences is null)
        {
            return BadRequest();
        }

        // TODO: id mockado deve ser pego com base no token jwt de autenticação
        var id = Guid.Empty;
        var user = context.Users.Include(x => x.Preferences).SingleOrDefault(x => x.Id == id);

        if (user is null)
        {
            return NotFound("User not found.");
        }

        user.SetPreferences(preferences);
        context.SaveChanges();

        return NoContent();
    }
}
