using System.Threading.Tasks;
using GraphiQl.Demo.GraphQl;
using GraphiQl.Demo.GraphQl.Models;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace GraphiQl.Demo.Controllers
{
    [Route(Startup.GraphQlPath)]
    [Route(Startup.CustomGraphQlPath)]
    public class GraphQlController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] GraphQlQuery query)
        {
            var schema = new Schema {Query = new StarWarsQuery()};
            
            var result = await new DocumentExecuter().ExecuteAsync(x =>
            {
                x.Schema = schema;
                x.Query = query.Query;
                x.Inputs = query.Variables;
            });
    
            if (result.Errors?.Count > 0)
            {
                return BadRequest();
            }
    
            return Ok(result);
        }
    }
}
