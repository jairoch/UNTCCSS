using UNTCCSS.Helper;
using UNTCCSS.Models;
using NuGet.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using UNTCCSS.Servicios.JCASoftware.JCAReponses;

namespace UNTCCSS.Servicios.JCASoftware
{
    public class JCARepositorio : IJCARepositorio
    {
        private readonly HttpClient httpClient;
        public JCARepositorio(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        public async Task<ResponseConsultaDni> ObtenerPersona(string Dni)
        {
            try
            {
                string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjMiLCJub21icmUiOiJKYWlybyIsImVtYWlsIjoiamFpcm9jaGluZ28xOUBnbWFpbC5jb20iLCJkbmkiOiI3MzcwMjQ5OCIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6ImNsaWVudGVzIiwiZXhwIjoxNzcwODU1NDYxLCJpc3MiOiJKQ0EgU29mdHdhcmUiLCJhdWQiOiJDbGllbnRlcyJ9.s5aySyEbxTud4mlsZfeXDBuVqnlCObTQnuYpKSHxteo";
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync($"reniec/terceros/dni/{Dni.Trim()}");
                if (response.IsSuccessStatusCode)
                {
                    var persona = await response.Content.ReadFromJsonAsync<ResponseConsultaDni>();
                    return persona;
                }
                Console.WriteLine($"❌ Error en la API: Código {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción general: {ex.Message}");
            }
            return new ResponseConsultaDni { Success = false };
        }
    }
}
