using DocumentApi.Data;
using DocumentApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DocumentApi.Models.DTOs;

namespace DocumentApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly AppDbContext _context;
    public UserController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAllUser()
    {
        var users = await _context.Users.ToListAsync();

        return Ok(users);
    }

    [HttpPost("create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.UserName,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok(user);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<ActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

}