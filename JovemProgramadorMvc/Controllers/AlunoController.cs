using JovemProgramadorMvc.Data.Repositorio.Interfaces;
using JovemProgramadorMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JovemProgramadorMvc.Controllers
{
    public class AlunoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IAlunoRepositorio _alunorepositorio;
        public AlunoController(IConfiguration configuration, IAlunoRepositorio alunoRepositorio)
        {
            _configuration = configuration;
            _alunorepositorio = alunoRepositorio;
        }
        public IActionResult Index(AlunoModel filtroaluno)
        {
            List<AlunoModel> aluno = new();

            if (filtroaluno.Idade > 0)
            {
                return View(_alunorepositorio.FiltroIdade(filtroaluno.Idade, filtroaluno.Operacao));
            }
            if (filtroaluno.Nome != null)
            {
                return View(_alunorepositorio.FiltroNome(filtroaluno.Nome));
            }
            if (filtroaluno.Contato != null)
            {
                return View(_alunorepositorio.FiltroContato(filtroaluno.Contato));
            }
            if (filtroaluno.Id == 0)
            {
                return View(_alunorepositorio.BuscarAlunos());
            }
            return View(aluno);
            
        }

        public IActionResult Adicionar()
        {
            return View();
        }
        public IActionResult Mensagens()
        {
            return View();
        }
        public IActionResult Filtros()
        {
            return View();
        }

        public async Task<IActionResult> BuscarEndereco(AlunoModel aluno)
        {
            EnderecoModel enderecoModel = new();
            try
            {
                var dados = _alunorepositorio.BuscarId(aluno.Id);

                var cep = dados.Cep.Replace("-", "");
                using var client = new HttpClient();
                var result = await client.GetAsync(_configuration.GetSection("ApiCep")["BaseUrl"] + cep + "/json");
                if (result.IsSuccessStatusCode)
                {
                    enderecoModel = JsonSerializer.Deserialize<EnderecoModel>(
                        await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { });
                    if (enderecoModel.complemento == "")
                    {
                        enderecoModel.complemento = "NÃO POSSUÍ";
                    }

                }
                else
                {
                    TempData["Mensagem"] = "NÃO FOI POSSÍVEL ACHAR O ENDEREÇO!";
                    return View("Index");
                }

            }
            catch (Exception e)
            {
                TempData["Mensagem"] = "FALHA NA REQUISIÇÃO!";
                return View("Index");
            }
            return View("BuscarEndereco", enderecoModel);
        }

        public IActionResult Inserir(AlunoModel aluno)
        {
            var retorno = _alunorepositorio.Inserir(aluno);
            if(retorno != null)
            {
                TempData["Mensagem2"] = "DADOS ADICIONADOS COM SUCESSO!";
            }
            return RedirectToAction("Index");
        }

        public IActionResult Editar(int id)
        {
            var aluno = _alunorepositorio.BuscarId(id);
            return View("Editar", aluno);
        }

        public IActionResult Atualizar(AlunoModel aluno)
        {
            var retorno = _alunorepositorio.Atualizar(aluno);

            return RedirectToAction("Index");
        }

        public IActionResult Excluir(int id)
        {
            var retorno = _alunorepositorio.Excluir(id);
            if (retorno == true)
            {
                TempData["Mensagem3"] = "ALUNO EXCLUÍDO COM SUCESSO!";
            }
            else
            {
                TempData["Mensagem3"] = "ALUNO NÃO FOI EXCLUÍDO!";
            }
            return RedirectToAction("Index");
        }
    }
}