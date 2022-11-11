using System;
using System.Data;
using System.Data.SqlClient;
using ClassesLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClassesTests
{
    [TestClass]
    public class FileProcessShouldDataDriven : TestBase
    {
        [TestMethod]
        public void FileExistsTestFromDb()
        {
            // Arrange
            var testFailed = false;
            var fp = new FileProcess();
            const string sqlQuery = "SELECT * FROM tests.FileProcessTest";
            var sqlConnString = TestContext!.Properties["SqlConnectionString"]!.ToString();
            if (sqlConnString != null)
            {
                var dataTable = LoadTestDataFromSql(sqlConnString, sqlQuery);
            
                if (dataTable.Rows.Count > 0)
                {
                    foreach (DataRow row in dataTable.Rows)
                    {
                        // Get the test values
                        var actual = false;
                        var fileName = row["FileName"].ToString();
                        var expected = Convert.ToBoolean(row["ExpectedValue"]);
                        var causesException = Convert.ToBoolean(row["CausesException"]);

                        try
                        {
                            // Act
                            actual = fp.FileExists(fileName);
                        }
                        catch (ArgumentNullException)
                        {
                            if (!causesException)
                            {
                                testFailed = true;
                            }
                        }
                        catch (Exception)
                        {
                            testFailed = true;
                        }

                        TestContext.WriteLine("Testing file: '{0}', Expected value: '{1}', Actual value: '{2}', Result: '{3}'", fileName, expected, actual, (expected == actual ? "Success" : "FAILED"));

                        if (expected != actual)
                        {
                            testFailed = true;
                        }
                    }
                }
            }

            // Assert
            if (testFailed)
            {
                Assert.Fail("Data driven tests have failed.");
            }
        }

        private DataTable LoadTestDataFromSql(string connection, string query)
        {
            var dataTable = new DataTable();

            try
            {
                using var conn = new SqlConnection(connection);
                using var cmd = new SqlCommand(query, conn);
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dataTable);
            }
            catch (SqlException ex)
            {
                TestContext?.WriteLine("Exception occurred in LoadTestDataFromSql: {0}", ex);
            }

            return dataTable;
        }
    }
}
