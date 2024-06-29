using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedisCache.Model;
using StackExchange.Redis;

namespace RedisCache.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IDatabase _database;

        public StudentController(IConnectionMultiplexer mux)
        {
            _database = mux.GetDatabase();
        }

        [HttpPost]
        public async Task<Student> saveSave(Student student)
        {
           await _database.HashSetAsync(student.Id,"name",student.Name);
           await _database.HashSetAsync(student.Id, "age", student.Age);
           return student;
        }
    }
}
