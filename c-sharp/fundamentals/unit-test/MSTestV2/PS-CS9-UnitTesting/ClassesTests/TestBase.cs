using System;
using System.IO;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassesTests
{
    public class TestBase
    {
        public TestContext? TestContext { get; set; }

        protected string? GoodFileName;
        protected void SetGoodFileName()
        {
            GoodFileName = TestContext!.Properties["GoodFileName"]!.ToString();
            if (GoodFileName != null && GoodFileName.Contains("[AppPath]"))
            {
                GoodFileName = GoodFileName.Replace("[AppPath]",
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }
        protected void EnsureGoodFileExists()
        {
            if (!string.IsNullOrEmpty(GoodFileName))
            {
                // Create the file
                TestContext?.WriteLine($"Creating file: {GoodFileName}");

                File.AppendAllText(GoodFileName, "A line in the file");
            }
        }
        protected void DeleteGoodFile()
        {
            if (File.Exists(GoodFileName))
            {
                TestContext?.WriteLine($"Deleting file: {GoodFileName}");

                File.Delete(GoodFileName);
            }
        }


        // Using Description Attribute
        protected void WriteDescription(Type type)
        {
            if (TestContext != null)
            {
                var testName = TestContext.TestName;
                if (testName != null)
                {
                    var method = type.GetMethod(testName);
                    if (method != null)
                    {
                        // Check for a description attribute
                        var descAttribute = method.GetCustomAttribute<DescriptionAttribute>();
                        if (descAttribute != null)
                        {
                            TestContext.WriteLine($"Test description: {descAttribute.Description}");
                        }
                    }
                }
            }
        }
    }
}
