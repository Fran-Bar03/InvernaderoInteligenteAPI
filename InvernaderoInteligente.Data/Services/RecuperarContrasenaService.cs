using InvernaderoInteligente.Data.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InvernaderoInteligente.Data.Services
{
    public class RecuperarContrasenaService
    {
        private readonly string _secret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationMinutes;


        public RecuperarContrasenaService(IConfiguration configuration)
        {
            _secret = configuration["Jwt:Secret"];
            _issuer = configuration["Jwt:Secret"];
            _audience = configuration["Jwt:Secret"];
            _expirationMinutes = 15;
        }

       
        public string GenerarCodigoDeRecuperacion(string email)
        {
            var codigo = new Random().Next(100000, 999999).ToString(); // Código aleatorio de 6 dígitos

            
            var claims = new[]
            {
                new Claim("email", email),    
                new Claim("codigo", codigo), 
                //new Claim(JwtRegisteredClaimNames.Exp, DateTime.UtcNow.AddMinutes(_expirationMinutes).ToString()) // Expiración del token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(8),
                signingCredentials: credentials
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);
        }


        public bool ValidarCodigo(ValidarCodigoDTO dto)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {

                var principal = tokenHandler.ReadToken(dto.Token) as JwtSecurityToken;

                
                var codigoToken = principal?.Claims.FirstOrDefault(c => c.Type == "codigo")?.Value;

                
                return codigoToken == dto.Codigo;
            }
            catch
            {
                
                return false;
            }
        }


    }
}
