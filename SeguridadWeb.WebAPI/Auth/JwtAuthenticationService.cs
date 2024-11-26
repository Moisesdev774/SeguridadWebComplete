using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// Librerías necesarias para el manejo de tokens JWT y la seguridad.
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SeguridadWeb.EntidadesDeNegocio;

namespace SeguridadWeb.WebAPI.Auth
{
    // Clase que implementa el servicio de autenticación con JWT.
    public class JwtAuthenticationService : IJwtAuthenticationService
    {
        // Campo privado que almacena la clave secreta utilizada para firmar el token.
        private readonly string _key;

        // Constructor que inicializa la clave secreta.
        public JwtAuthenticationService(string key)
        {
            _key = key; // La clave se pasa como parámetro al crear la instancia.
        }

        // Método para autenticar un usuario y generar un token JWT.
        public string Authenticate(Usuario pUsuario)
        {
            // Crea un manejador para trabajar con tokens JWT.
            var tokenHandler = new JwtSecurityTokenHandler();

            // Convierte la clave secreta a un arreglo de bytes.
            var tokenKey = Encoding.ASCII.GetBytes(_key);

            // Define las propiedades del token, como las reclamaciones (claims), la expiración y las credenciales de firma.
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                // Configura la identidad del token con una lista de reclamaciones.
                Subject = new ClaimsIdentity(new Claim[]
                {
                    // Agrega un reclamo que identifica al usuario por su nombre de inicio de sesión (Login).
                    new Claim(ClaimTypes.Name, pUsuario.Login)
                }),

                // Configura la expiración del token para 8 horas desde el momento actual.
                Expires = DateTime.UtcNow.AddHours(8),

                // Configura las credenciales de firma, utilizando la clave secreta y el algoritmo HMAC-SHA256.
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            // Crea el token usando el descriptor configurado.
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Devuelve el token como una cadena codificada.
            return tokenHandler.WriteToken(token);
        }
    }
}
