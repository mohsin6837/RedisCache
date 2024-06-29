namespace RedisCache.Model
{
    public class Student
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }

        public Student(string name, int age)
        {
            Id=Guid.NewGuid().ToString();
            Age = age;
            Name = name;
        }
    }
}
