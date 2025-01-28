using Microsoft.AspNetCore.Mvc;
using RuleConfigService.Models;

namespace RuleConfigService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RulesController(RuleConfigDbContext context) : ControllerBase
    {
        [HttpGet]
        public IActionResult GetRules()
        {
            return Ok(context.Rules.ToList());
        }

        [HttpPost]
        public IActionResult AddRule([FromBody] Rule rule)
        {
            context.Rules.Add(rule);
            context.SaveChanges();
            return Ok(rule);
        }
    }
}