using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BrightBoostApplication.Data;
using BrightBoostApplication.Models;

namespace BrightBoostApplication.Controllers
{
    public class ExpertisesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpertisesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Expertises
        public async Task<IActionResult> Index()
        {
            return View();
        }

    }
}
