﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebAPIAutores.DTOs;

namespace WebAPIAutores.Controllers
{
    [Route("api/cuentas")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly SignInManager<IdentityUser> _signInManager;

        public CuentasController(UserManager<IdentityUser> userManager, IConfiguration configuration, SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(CredencialesUsuario credencialesUsuario)
        {
            var usuario = new IdentityUser { UserName = credencialesUsuario.Email, Email = credencialesUsuario.Email };
            var resultado = await _userManager.CreateAsync(usuario, credencialesUsuario.Password);

            if (resultado.Succeeded)
                return ConstruirToken(credencialesUsuario);
            else
                return BadRequest(resultado.Errors);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login (CredencialesUsuario credencialesUsuario)
        {
            var resultado = await _signInManager.PasswordSignInAsync(credencialesUsuario.Email, credencialesUsuario.Password, isPersistent: false, lockoutOnFailure: false);

            if (resultado.Succeeded)
                return ConstruirToken(credencialesUsuario);
            else
                return BadRequest("login incorrecto");
        }

        private RespuestaAutenticacion ConstruirToken(CredencialesUsuario credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("email", credencialesUsuario.Email),
            };

            var llave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["llavejwt"]));

            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion,
            };
        }
    }
}
