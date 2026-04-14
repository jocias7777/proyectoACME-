using System.Web;

namespace proyectoACME.Services
{
    public class AuthService
    {
        public static bool EstaLogueado(HttpSessionStateBase session)
        {
            return session["UsuarioId"] != null;
        }

        public static int GetUsuarioId(HttpSessionStateBase session)
        {
            return (int)session["UsuarioId"];
        }

        public static string GetUsername(HttpSessionStateBase session)
        {
            return session["Username"]?.ToString();
        }
    }
}