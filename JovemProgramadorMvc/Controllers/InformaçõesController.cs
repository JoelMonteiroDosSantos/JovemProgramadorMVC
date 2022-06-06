using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JovemProgramadorMvc.Controllers
{
    public class InformaçõesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
