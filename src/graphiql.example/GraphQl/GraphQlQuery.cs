using GraphQL;

namespace graphiql.example.GraphQl
{
    public class GraphQlQuery
    {
        public string OperationName { get; set; }
        public string NamedQuery { get; set; }
        public string Query { get; set; }
        public Inputs Variables { get; set; }
    }
}