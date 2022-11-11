using System.Collections.Generic;

namespace ClassesLibrary.Person
{
    public class PersonManager
    {
        public Person? CreatePerson(string first, string last, bool isSupervisor)
        {
            Person? ret = null;

            if (string.IsNullOrEmpty(first)) return ret;
            if (isSupervisor)
            {
                ret = new Supervisor();
            }
            else
            {
                ret = new Employee();
            }

            ret.FirstName = first;
            ret.LastName = last;

            return ret;
        }

        public List<Person> GetPeople()
        {
            return new List<Person>
            {
                new() { FirstName = "Paul", LastName = "McCartney" },
                new() { FirstName = "Michael", LastName = "Jackson" },
                new() { FirstName = "Whitney", LastName = "Houston" },
                new() { FirstName = "David", LastName = "Bowie" }
            };
        }

#pragma warning disable CS8604 // Possible null reference argument.
        public List<Person> GetSupervisors() => new()
        {
                CreatePerson("Bruce", "Springsteen", true),
                CreatePerson("Freddy", "Mercury", true)
            };
#pragma warning restore CS8604 // Possible null reference argument.

        public List<Person> GetEmployees()
        {
#pragma warning disable CS8604 // Possible null reference argument.
            return new List<Person>
            {
                CreatePerson("Devon", "Burch", false),
                CreatePerson("Bobbi", "Malone", false),
                CreatePerson("Virgil", "Mullins", false),
                CreatePerson("Miriam", "Reid", false)
            };
#pragma warning restore CS8604 // Possible null reference argument.

        }

        public List<Person> GetSupervisorsAndEmployees()
        {
            var people = new List<Person>();

            people.AddRange(GetEmployees());
            people.AddRange(GetSupervisors());

            return people;
        }
    }
}
