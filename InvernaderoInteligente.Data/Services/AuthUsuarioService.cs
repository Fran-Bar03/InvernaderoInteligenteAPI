using InvernaderoInteligente.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Services 
{
  public class AuthUsuarioService 
  {
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience = "https://localhost:44360/";
    private readonly int _expirationminutes;


    public AuthUsuarioService(IConfiguration configuration) 
    {
      _secret = configuration["Jwt.Secret"]!;
      _issuer = configuration["Jwt.issuer"]!;
      _expirationminutes = configuration.GetValue<int> ("Jwt.expirationminutes", 30);
    }




    public string GenerarToken (UsuarioModel usuario) 
    {
      var Claims = new[] 
      {
        new Claim(ClaimTypes.NameIdentifier, usuario.UsuarioId!),
        new Claim(ClaimTypes.Name, usuario.Email!),
        new Claim(ClaimTypes.Role, usuario.Rol.ToString())
      };



      //Aqui creamos la clave de seguridad para firmar el token

      var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

      var Credential = new SigningCredentials (Key, SecurityAlgorithms.HmacSha256);




      // Aqui se esta creando el token, 

      var Token = new JwtSecurityToken (
        issuer: _issuer,
        audience: _audience,
        claims: Claims,
        expires: DateTime.UtcNow.AddMinutes (_expirationminutes),
        signingCredentials: Credential
        );


      //Aqui creamos el token como string

      var TokenHandler = new JwtSecurityTokenHandler ();
      return TokenHandler.WriteToken (Token);

    }
    

  }
}
