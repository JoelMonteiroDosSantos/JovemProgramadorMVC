using JovemProgramadorMvc.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace JovemProgramadorMvc.Controllers
{
    public class ProfessorController : Controller
    {
        private readonly IConfiguration _configuration;
        private IConfiguration configuration;

        public ProfessorController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> BuscarEnderecoProfessorAsync(string cep)
        {
            EnderecoModel enderecoModel = new();

            try
            {
                cep = cep.Replace("-", "");

                using var client = new HttpClient();
                var result = await client.GetAsync(_configuration.GetSection("ApiCep")["BaseUrl"] + cep + "/json");

                if (result.IsSuccessStatusCode)
                {
                    enderecoModel = JsonSerializer.Deserialize<EnderecoModel>(
                        await result.Content.ReadAsStringAsync(), new JsonSerializerOptions() { });

                    if (enderecoModel.complemento == "")
                    {
                        enderecoModel.complemento = "Não possuí";
                    }
                }
                else
                {
                    ViewData["Mensagem"] = "ERRO AO BUSCAR O ENDEREÇO!";
                    return View("Index");
                }
            }

            catch (Exception e)
            {
                ViewData["Mensagem"] = "ERRO AO BUSCAR O ENDEREÇO!";
                return View("Index");
            }
            return View("BuscarEnderecoProfessor", enderecoModel);
        }
    }
}