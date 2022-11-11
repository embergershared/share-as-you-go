using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassesTests
{
    [TestClass]
    public class ClassesTestsInitialization
    {
        [AssemblyInitialize()]
        public static void AssemblyInitialize(TestContext context)
        {
            // TODO: Initialize for all tests in the assembly
            context.WriteLine("In AssemblyInitialize() method");

            // Can be adding data, a db, a file, a registry key
        }

        [AssemblyCleanup()]
        public static void AssemblyCleanup()
        {
            // TODO: Cleanup the data, db, etc created
        }
    }
}
