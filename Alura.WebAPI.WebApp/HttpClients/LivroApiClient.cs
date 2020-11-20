using Alura.ListaLeitura.Modelos;
using Alura.ListaLeitura.Seguranca;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.HttpClients
{
    public class LivroApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly AuthApiClient _auth;

        public LivroApiClient(HttpClient httpClient, AuthApiClient auth)
        {
            _httpClient = httpClient;
            _auth = auth;
        }

        public async Task<LivroApi> GetLivroAsync(int id)
        {
            var resposta = await _httpClient.GetAsync($"{id}");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsAsync<LivroApi>();
        }

        public async Task<byte[]> GetCapaLivroAsync(int id)
        {
            var token = await _auth.PostLoginAsync(new LoginModel { Login = "Lucas", Password = "123" });

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var resposta = await _httpClient.GetAsync($"{id}/capa");
            resposta.EnsureSuccessStatusCode();

            return await resposta.Content.ReadAsByteArrayAsync();
        }

        public async Task DeleteLivroAsync(int id)
        {
            var resposta = await _httpClient.DeleteAsync($"{id}");
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PostLivroAsync(LivroUpload model)
        {
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PostAsync("", content);
            resposta.EnsureSuccessStatusCode();
        }

        public async Task PutLivroAsync(LivroUpload model)
        {
            var content = CreateMultipartFormDataContent(model);
            var resposta = await _httpClient.PutAsync("", content);
            resposta.EnsureSuccessStatusCode();
        }

        private HttpContent CreateMultipartFormDataContent([FromForm] LivroUpload model)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(model.Titulo ?? ""), EnvolveComAspasDuplas("titulo"));
            content.Add(new StringContent(model.Subtitulo ?? ""), EnvolveComAspasDuplas("subtitulo"));
            content.Add(new StringContent(model.Resumo ?? ""), EnvolveComAspasDuplas("resumo"));
            content.Add(new StringContent(model.Autor ?? ""), EnvolveComAspasDuplas("autor"));
            content.Add(new StringContent(model.Lista.ParaString()), EnvolveComAspasDuplas("lista"));

            if (model.Id > 0)
            {
                content.Add(new StringContent(model.Id.ToString()), EnvolveComAspasDuplas("id"));
            }

            if (model.Capa != null)
            {
                var imagemContent = new ByteArrayContent(model.Capa.ConvertToBytes());
                imagemContent.Headers.Add("content-type", "image/png");
                content.Add(
                    imagemContent,
                    EnvolveComAspasDuplas("capa"),
                    EnvolveComAspasDuplas("capa.png")
                );
            }

            return content;
        }

        private string EnvolveComAspasDuplas(string valor)
        {
            return $"\"{valor}\"";
        }
    }
}
