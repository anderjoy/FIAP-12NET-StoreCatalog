using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StoreCatalog.WebAPI.Controllers
{
    public class StoreCatalogController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("statusServer/")]
        public IActionResult GetStatusServer()
        {
            return Ok();
        }
    }
}