using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassesLibrary.Person;

namespace ClassesTests
{
    [TestClass]
    public class PersonShould : TestBase
    {
        [TestInitialize]
        public void TestInitialize()
        {
            TestContext?.WriteLine("In TestInitialize() method");

            WriteDescription(GetType());
        }


        [TestMethod]
        [Description("This test creates a Person with the attribute supervisor and checks it got a Supervisor type object")]
        public void IsInstanceOfTypeSupervisor_Test()
        {
            // Arrange
            var sut = new PersonManager();

            // Act
            var actual = sut.CreatePerson("Piotr", "Knott", true);

            // Assert
            Assert.IsInstanceOfType(actual, typeof(Supervisor));
        }

        [TestMethod]
        [Description("This test creates a Person with parameters that will return a Null object")]
        public void IsNull_Test()
        {
            // Arrange
            var sut = new PersonManager();

            // Act
            var actual = sut.CreatePerson("", "Joel", true);

            // Assert
            Assert.IsNull(actual);
        }

        [TestMethod]
        [Description("This test creates a ICollection<Person> and compares it to GetPeople()")]
        public void AreCollectionsEqual_Test()
        {
            // Arrange
            var sut = new PersonManager();
            var peopleExpected = new List<Person>
            {
                new() { FirstName = "Paul", LastName = "McCartney" },
                new() { FirstName = "Michael", LastName = "Jackson" },
                new() { FirstName = "Whitney", LastName = "Houston" },
                new() { FirstName = "David", LastName = "Bowie" }
            };

            // Act
            var peopleActual = sut.GetPeople();

            // Assert
            CollectionAssert.AreNotEqual(peopleExpected, peopleActual);
        }

        [TestMethod]
        [Description("This test checks GetSupervisors() return Supervisor objects' type")]
        public void IsCollectionOfType_Test()
        {
            // Arrange
            var sut = new PersonManager();

            // Act
            var peopleActual = sut.GetSupervisors();

            // Assert
            CollectionAssert.AllItemsAreInstancesOfType(peopleActual, typeof(Supervisor));
        }

        [TestMethod]
        [Description("This test checks if 2 collections are equivalent")]
        public void AreCollectionEquivalent_Test()
        {
            // Arrange
            var sut = new PersonManager();
            var peopleExpected = new List<Person>();

            // Act
            var peopleActual = sut.GetPeople();
            peopleExpected.Add(peopleActual[1]);
            peopleExpected.Add(peopleActual[2]);
            peopleExpected.Add(peopleActual[3]);
            peopleExpected.Add(peopleActual[0]);

            // Assert
            CollectionAssert.AreEquivalent(peopleExpected, peopleActual);
        }

        [TestMethod]
        [Description("This test creates a ICollection<Person> and compares it to GetPeople(), with a custom Comparer")]
        public void AreCollectionsEqualWithComparer_Test()
        {
            // Arrange
            var sut = new PersonManager();
            var peopleExpected = new List<Person>
            {
                new() { FirstName = "Paul", LastName = "McCartney" },
                new() { FirstName = "Michael", LastName = "Jackson" },
                new() { FirstName = "Whitney", LastName = "Houston" },
                new() { FirstName = "David", LastName = "Bowie" }
            };

            // Act
            var peopleActual = sut.GetPeople();

            // Assert
            CollectionAssert.AreEqual(peopleExpected, peopleActual,
                Comparer<Person>.Create((x,y) => 
                    x.FirstName == y.FirstName && x.LastName == y.LastName ? 0 : 1 ));
        }
    }
}
