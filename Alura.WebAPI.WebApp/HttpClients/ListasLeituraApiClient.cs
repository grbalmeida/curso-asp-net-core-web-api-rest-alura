using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Lista = Alura.ListaLeitura.Modelos.ListaLeitura;

namespace Alura.ListaLeitura.HttpClients
{
    public class ListasLeituraApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiClient _auth;

        public ListasLeituraApiClient(HttpClient httpClient, AuthApiClient auth)
        {
            _httpClient = httpClient;
            _auth = auth;
        }

        public async Task<Lista> GetListaLeituraAsync(TipoListaLeitura tipo)
        {
            var token = await _auth.PostLoginAsync(new LoginModel { Login = "Lucas", Password = "123" });

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resposta = await _httpClient.GetAsync($"{tipo}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<Lista>();
        }
    }
}
