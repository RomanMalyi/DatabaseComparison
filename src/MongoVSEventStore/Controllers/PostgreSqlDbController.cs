using Microsoft.AspNetCore.Mvc;
using MongoVSEventStore.Dto.Commands;
using Npgsql;

namespace MongoVSEventStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostgreSqlDbController : ControllerBase
    {
        [HttpPost("users")]
        public async Task<IActionResult> Post(CreateUser command)
        {
            //pgAdmin ->PGADMIN_DEFAULT_EMAIL=myemail@example.com + PGADMIN_DEFAULT_PASSWORD=SuperSecret

            var cs = "Host=localhost;Username=testuser;Password=testpass;Database=testdb";
            await using var con = new NpgsqlConnection(cs);
            con.Open();

            await InsertUser(con, command);

            return Ok();
        }

        [HttpPost("users/{userId}/stock")]
        public async Task<IActionResult> AddStock([FromRoute] Guid userId, [FromBody] AddStockInfo command)
        {
            return Ok();
        }

        private async Task InsertUser(NpgsqlConnection con, CreateUser command)
        {
            await using var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            cmd.CommandText = "INSERT INTO users (UserId, CreatedDate) VALUES (@userId, @createdDate)";
            cmd.Parameters.AddWithValue("userId", command.Id);
            //TODO: add name
            cmd.Parameters.AddWithValue("createdDate", DateTime.UtcNow);
            cmd.ExecuteNonQuery();
        }
    }
}
