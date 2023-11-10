using System.Security.Authentication;
using api.Filters;
using api.TransferModels;
using Microsoft.AspNetCore.Mvc;
using service;
using service.Models.Command;
using service.Services;

namespace api.Controllers;

[ValidateModel]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountService _service;
    private readonly JwtService _jwtService;
    private readonly BlobService _blobService;

    public AccountController(AccountService service, JwtService jwtService, BlobService blobService)
    {
        _service = service;
        _jwtService = jwtService;
        _blobService = blobService;
    }

    [HttpPost]
    [Route("/api/account/login")]
    public IActionResult Login([FromBody] LoginCommandModel model)
    {
        var user = _service.Authenticate(model);
        if (user == null) return Unauthorized();
        var token = _jwtService.IssueToken(SessionData.FromUser(user!));
        return Ok(new { token });
    }

    [HttpPost]
    [Route("/api/account/register")]
    public IActionResult Register([FromBody] RegisterCommandModel model)
    {
        var user = _service.Register(model);
        return Created();
    }

    [RequireAuthentication]
    [HttpGet]
    [Route("/api/account/whoami")]
    public IActionResult WhoAmI()
    {
        var data = HttpContext.GetSessionData();
        var user = _service.Get(data);
        return Ok(user);
    }

    [RequireAuthentication]
    [HttpPut]
    [Route("/api/account/update")]
    public IActionResult Update([FromForm] UpdateAccountCommandModel model, IFormFile? avatar)
    {
        var session = HttpContext.GetSessionData()!;
        string? avatarUrl = null;
        if (avatar != null)
        {
            avatarUrl = _service.Get(session)?.AvatarUrl;
            using var avatarStream = avatar.OpenReadStream();
            _blobService.Save("avatar", avatarStream, avatarUrl);
        }
        
        avatarUrl = _service.Get(session)?.AvatarUrl;
        var user = _service.Update(session, model, avatarUrl);
        return Ok(user);
    }
}