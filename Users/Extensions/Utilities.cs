using RestSharp;
using Users.Models;

namespace Users.Extensions
{
    public static class Utilities
    {
        public static void SendWelcomeEmail(User user)
        {
            try
            {
                var client = new RestClient("http://localhost:3030/api/email/setpassword");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    Name = user.Firstname,
                    ToEmail = user.Email,
                    Subject = "Activar cuenta",
                    Message = user.UserId,
                });
                var response = client.Execute(request);
                Console.WriteLine($"Content response: {response.Content}");
                Console.WriteLine($"Status: {response.StatusCode}");
                if (response.Headers != null)
                {
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine(header);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void SendResetEmail(User user)
        {
            try
            {
                var client = new RestClient("http://localhost:3030/api/email/resendpassword");
                var request = new RestRequest
                {
                    Method = Method.Get
                };
                request.AddHeader("Content-Type", "application/json");
                request.AddJsonBody(new
                {
                    Name = user.Firstname,
                    ToEmail = user.Email,
                    Subject = "Cambiar contraseña",
                    Message = user.UserId,
                });
                var response = client.Execute(request);
                Console.WriteLine($"Content response: {response.Content}");
                Console.WriteLine($"Status: {response.StatusCode}");
                if (response.Headers != null)
                {
                    foreach (var header in response.Headers)
                    {
                        Console.WriteLine(header);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
