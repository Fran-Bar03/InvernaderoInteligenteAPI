using InvernaderoInteligente.Data.DTOs;
using InvernaderoInteligente.Data.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        private readonly IMemoryCache _memorycache;
        private readonly IEmailService _emailservice;
        private readonly IUsuarioService _usuarioservice; 


        public RecuperarContrasenaService(IConfiguration configuration, IMemoryCache memorycache, IEmailService emailservice, IUsuarioService usuarioservice)
        {
            _secret = configuration["Jwt:Secret"];
            _issuer = configuration["Jwt:Secret"];
            _audience = configuration["Jwt:Secret"];
            _memorycache = memorycache;
            _emailservice = emailservice;
            _usuarioservice = usuarioservice;
        }



        public async Task EnviarCodigoRecuperacion(RecuperarContrasenaEmailDTO dto)
        {
            try
            {
                var codigo = new Random().Next(100000, 999999).ToString();

                var Token = GenerarToken(dto.Email);

                var data = new Dictionary<string, object>
            {
                {"Codigo", codigo },
                {"HoraGeneracion", DateTime.UtcNow },
                {"Token", Token }
            };

                _memorycache.Set(dto.Email, data, TimeSpan.FromMinutes(6));
                await _emailservice.EnviarCorreoAsync(dto.Email, "Codigo de recuperacion", $"Tu codigo de recuperacion es : {codigo}");
            }
            catch (Exception ex)
            {
                throw new Exception($"Hubo un error al enviar el código de recuperación: {ex.Message}", ex);
            }
           

        }


        public string ValidarCodigo(ValidarCodigoDTO dto)
        {
            if (_memorycache.TryGetValue(dto.Email, out Dictionary<string, object> data))
            {
                var codigoGuardado = data["Codigo"] as string;
                var horaGeneracion = data["HoraGeneracion"] as DateTime?;
                var tokenGuardado = data["Token"] as string; // Recuperamos el token

                if (codigoGuardado == dto.Codigo && horaGeneracion.HasValue &&
                    DateTime.UtcNow.Subtract(horaGeneracion.Value).TotalMinutes <= 6)
                {
                    _memorycache.Remove(dto.Email);
                    return tokenGuardado;
                }
            }

            throw new Exception("Código inválido o expirado.");
        }


        public string GenerarToken(string Email)
        {
            var Claims = new List<Claim>
            {
                new Claim("Email", Email)
            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
            var Credential = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);

            var Token = new JwtSecurityToken(
                issuer : _issuer,
                audience: _audience,
                claims : Claims,
                expires : DateTime.UtcNow.AddMinutes(6),
                signingCredentials : Credential);

            return new JwtSecurityTokenHandler().WriteToken(Token);
        }

        public string ValidarToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    var emailClaim = jsonToken?.Claims.FirstOrDefault(c => c.Type == "Email");
                    if (emailClaim != null)
                    {
                        return emailClaim.Value; // Retorna el correo del claim
                    }
                }
            }
            catch (Exception)
            {
                throw new UnauthorizedAccessException("Token inválido.");
            }

            return null;
        }

    }
}
