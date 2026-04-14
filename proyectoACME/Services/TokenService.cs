using System;

namespace proyectoACME.Services
{
    public class TokenService
    {
        public static string GenerarToken()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}