using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace CV_Chatbot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> PostAuthentication()
        {
            var secretToken = "Zhvaz353c5U.dVW7Jrsnk5BhMrmQvFd9SB2fco_-EwdGork4ws1s0Zs";// _configuration.GetValue<string>("SecretToken");
            HttpClient client = new HttpClient();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //Realiza un Get con el secret token para generar uno temporal de sesion
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://webchat.botframework.com/api/tokens");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", secretToken);
            var response = await client.SendAsync(request);
            string token = String.Empty;
            string jsonresult = String.Empty;
            if (response.IsSuccessStatusCode)
            {
                token = await response.Content.ReadAsStringAsync();
                token = token.Replace("\"", "");
                var tokenJWT = handler.ReadJwtToken(token) as JwtSecurityToken;
                var idconversacion = tokenJWT.Claims.FirstOrDefault(c => c.Type == "conv").Value;
                jsonresult = await Task.Run(() => JsonSerializer.Serialize(new { idConversation = idconversacion, token = token }));
                return Ok(jsonresult);
            }
            else
            {
                var notSuccessMesagge = $"Respuesta sin exito \nFecha: {DateTime.Today} \nCodigo: {response.StatusCode} \nMensaje: {response.RequestMessage}";
                //_logger.LogInformation(notSuccessMesagge);                    
                return Forbid();
            }

        }
    }
}
