﻿using System.Linq;
using Alura.ListaLeitura.Persistencia;
using Alura.ListaLeitura.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System;
using System.Threading.Tasks;

namespace Alura.ListaLeitura.WebApp.Controllers
{
    [Authorize]
    public class LivroController : Controller
    {
        private readonly IRepository<Livro> _repo;
        private readonly HttpClient _httpClient;

        public LivroController(IRepository<Livro> repository)
        {
            _repo = repository;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:7000/api/");
        }

        [HttpGet]
        public IActionResult Novo()
        {
            return View(new LivroUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Novo(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                _repo.Incluir(model.ToLivro());
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ImagemCapa(int id)
        {
            byte[] img = _repo.All
                .Where(l => l.Id == id)
                .Select(l => l.ImagemCapa)
                .FirstOrDefault();
            if (img != null)
            {
                return File(img, "image/png");
            }
            return File("~/images/capas/capa-vazia.png", "image/png");
        }

        [HttpGet]
        public async Task<IActionResult> Detalhes(int id)
        {
            var resposta = await _httpClient.GetAsync($"Livros/{id}");
            resposta.EnsureSuccessStatusCode();

            var livro = await resposta.Content.ReadAsAsync<LivroApi>();

            if (livro == null)
            {
                return NotFound();
            }

            return View(livro.ToUpload());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Detalhes(LivroUpload model)
        {
            if (ModelState.IsValid)
            {
                var livro = model.ToLivro();
                if (model.Capa == null)
                {
                    livro.ImagemCapa = _repo.All
                        .Where(l => l.Id == livro.Id)
                        .Select(l => l.ImagemCapa)
                        .FirstOrDefault();
                }
                _repo.Alterar(livro);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Remover(int id)
        {
            var model = _repo.Find(id);
            if (model == null)
            {
                return NotFound();
            }
            _repo.Excluir(model);
            return RedirectToAction("Index", "Home");
        }
    }
}