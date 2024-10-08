﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ProjectManagement.Application.Contracts.Account;
using ProjectManagement.Application.Interfaces;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AccountController> _logger;
    private readonly ICacheService _cache;

    public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenService tokenService, ILogger<AccountController> logger, ICacheService cache)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _logger = logger;
        _cache = cache;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto, CancellationToken ct)
    {
        try
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var cacheKey = $"User_{registerDto.Email}";
            var existingUser = await _cache.GetAsync<AppUser>(cacheKey);
            if (existingUser != null)
            {
                _logger.LogInformation("User {Email} already exists in cache", registerDto.Email);
                return BadRequest("User already exists");
            }

            var user = new AppUser
            {
                UserName = registerDto.Username,
                Email = registerDto.Email,
            };
            var createdUser = await _userManager.CreateAsync(user, registerDto.Password);

            if (createdUser.Succeeded)
            {
                var roleResult = await _userManager.AddToRoleAsync(user, "User");
                if (roleResult.Succeeded)
                {
                    var response = new UserResponseDto
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        Token = _tokenService.CreateToken(user)
                    };
                    _logger.LogInformation("User registered: {Username}, {Email}", user.UserName, user.Email);
                    await _cache.SetAsync(cacheKey, user, new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
                    }, ct);
                    return Ok(response);
                }
                else
                {
                    _logger.LogError("Error occurred while adding role to user {Username}: {Error}", user.UserName, roleResult.Errors);
                    return StatusCode(500, roleResult.Errors);
                }
            }
            else
            {
                _logger.LogError("Error occurred while creating user: {Username}, {Email}", user.UserName, user.Email);
                return StatusCode(500, createdUser.Errors);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while registering user");
            return StatusCode(500, ex);
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userManager.Users.FirstOrDefaultAsync(x => x.Email.ToLower() == loginDto.Email.ToLower());
        if (user is null)
            return Unauthorized("Invalid Email!");

        var cacheKey = $"User_{loginDto.Email}";
        var existingUser = await _cache.GetAsync<AppUser>(cacheKey);
        if (existingUser != null && existingUser.Email.ToLower() == loginDto.Email.ToLower())
        {
            _logger.LogInformation("User {Email} found in cache", loginDto.Email);
            if (!string.IsNullOrEmpty(loginDto.Password))
            {
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
                if (result.Succeeded)
                {
                    var response = new UserResponseDto
                    {
                        Username = user.UserName,
                        Email = user.Email,
                        Token = _tokenService.CreateToken(user)
                    };
                    _logger.LogInformation("User logged in: {Username}, {Email}", user.UserName, user.Email);
                    return Ok(response);
                }
                else
                {
                    _logger.LogError("Incorrect password for user {Username}, {Email}", user.UserName, user.Email);
                    return Unauthorized("Incorrect password");
                }
            }
            else
            {
                _logger.LogError("Password not provided for user {Username}, {Email}", user.UserName, user.Email);
                return Unauthorized("Password not provided");
            }
        }

        var resultDb = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);
        if (!resultDb.Succeeded)
            return Unauthorized("User not found and/or incorrect password");

        var responseDb = new UserResponseDto
        {
            Username = user.UserName,
            Email = user.Email,
            Token = _tokenService.CreateToken(user)
        };
        _logger.LogInformation("User logged in: {Username}, {Email}", user.UserName, user.Email);
        await _cache.SetAsync(cacheKey, user, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        }, ct);
        return Ok(responseDb);
    }
}
