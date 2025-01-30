using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RuleConfigService.Models;

namespace RuleConfigService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RulesController : ControllerBase
    {
        private readonly RuleConfigDbContext _context;

        public RulesController(RuleConfigDbContext context)
        {
            _context = context;
            context.Database.Migrate();
        }
        
        
        [HttpGet]
        public IActionResult GetRules()
        {
            return Ok(_context.Rules.ToList());
        }

        [HttpPost]
        public IActionResult AddRule([FromBody] Rule rule)
        {
            _context.Rules.Add(rule);
            _context.SaveChanges();
            return Ok(rule);
        }
    }
}