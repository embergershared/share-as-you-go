using System.Collections.Generic;

namespace ClassesLibrary.Person
{
    public class Supervisor : Person
    {
        public List<Employee>? Employees { get; set; }
    }
}
