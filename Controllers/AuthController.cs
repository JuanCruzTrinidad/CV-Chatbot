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
            var secretToken = "0HoFjkIP43I.Yba-TOFzrSJyX3WoSIVl-6em34SdSwZ1jM6CkkXRNhs";// _configuration.GetValue<string>("SecretToken");
            HttpClient client = new HttpClient();
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            //Realiza un Get con el secret token para generar uno temporal de sesion
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"https://webchat.botframework.com/api/tokens");
            request.Headers.Authorization = new AuthenticationHeaderValue("BotConnector", secretToken);
            var response = await client.SendAsync(request);
            string token = String.Empty;
            string jsonresult = String.Empty;
            if (response.IsSuccessStatusCode)
            {
                token = await response.Content.ReadAsStringAsync();
                var tokenConvert = JsonSerializer.Deserialize<DirectLineToken>(token);
                //var tokenJWT = handler.ReadJwtToken(token) as JwtSecurityToken;
                //var idconversacion = tokenJWT.Claims.FirstOrDefault(c => c.Type == "conv").Value;
                //jsonresult = await Task.Run(() => JsonSerializer.Serialize(new { idConversation = idconversacion, token = token }));
                return Ok(tokenConvert.token);
            }
            else
            {
                var notSuccessMesagge = $"Respuesta sin exito \nFecha: {DateTime.Today} \nCodigo: {response.StatusCode} \nMensaje: {response.RequestMessage}";
                //_logger.LogInformation(notSuccessMesagge);                    
                return Forbid();
            }

        }
    }

    public class DirectLineToken
    {
        public string token { get; set; }
        public string conversationId { get; set; }
        public int expires_in { get; set; }
    }
}
