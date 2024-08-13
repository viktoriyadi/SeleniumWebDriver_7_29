using System.Collections.ObjectModel;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace TestProject2
{
    [TestFixture]
    public class WorkingWithWebTable
    {
        IWebDriver driver;

        [SetUp]
        public void SetUp()
        {
            // Create object of ChromeDriver
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArguments("headless");
            chromeOptions.AddArguments("no-sandbox");
            chromeOptions.AddArguments("disable-dev-shm-usage");
            chromeOptions.AddArguments("disable-gpu");
            chromeOptions.AddArguments("window-size=1920x1080");
            chromeOptions.AddArguments("disable-extensions");
            chromeOptions.AddArguments("remote-debugging-port=9222");

            chromeOptions.AddUserProfilePreference("profile.password_manager_enabled", false);
            chromeOptions.AddArgument("--disable-search-engine-choice-screen");

            driver = new ChromeDriver(chromeOptions);

            // Add implicit wait
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void TestExtractProductInformation()
        {
            // Launch Chrome browser with the given URL
            driver.Url = "http://practice.bpbonline.com/";

            // Identify the web table
            IWebElement productTable = driver.FindElement(By.XPath("//*[@id='bodyContent']/div/div[2]/table"));

            // Find the number of rows
            ReadOnlyCollection<IWebElement> tableRows = productTable.FindElements(By.XPath("//tbody/tr"));

            // Path to save the CSV file
            string path = System.IO.Directory.GetCurrentDirectory() + "/productinformation.csv";

            // If the file exists in the location, delete it
                        if (File.Exists(path))
                File.Delete(path);

            // Traverse through table rows to find the table columns
               foreach (IWebElement trow in tableRows)
            {
                ReadOnlyCollection<IWebElement> tableCols = trow.FindElements(By.XPath("td"));
                foreach (IWebElement tcol in tableCols)
                {
                    // Extract product name and cost
                    String data = tcol.Text;
                    String[] productinfo = data.Split('\n');
                    String printProductinfo = productinfo[0].Trim() + "," + productinfo[1].Trim() + "\n";

                    // Write product information extracted to the file
                    File.AppendAllText(path, printProductinfo);
                }
            }

            // Verify the file was created and has content
            Assert.IsTrue(File.Exists(path), "CSV file was not created");
            Assert.IsTrue(new FileInfo(path).Length > 0, "CSV file is empty");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}