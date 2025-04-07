using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WebApplication2.Models;
using WebApplication2.Data;

namespace WebApplication2.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly AppDbContext _context;
        public DashBoardController (AppDbContext context)
        {
            _context = context;
        }


    }
}
