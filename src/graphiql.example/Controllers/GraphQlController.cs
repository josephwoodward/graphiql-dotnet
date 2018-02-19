using System.Threading.Tasks;
using graphiql.example.GraphQl;
using graphiql.example.GraphQl.Models;
using GraphQL;
using GraphQL.Types;
using Microsoft.AspNetCore.Mvc;

namespace graphiql.example.Controllers
{
    [Route(Startup.GraphQLPath)]
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
