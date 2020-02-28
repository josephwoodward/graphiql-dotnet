using System;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;

namespace GraphiQl.tests
{
    public abstract class BaseTest
    {
        protected ChromeDriver Driver { get; }

        protected BaseTest(bool runHeadless)
        {
            var options = new ChromeOptions();
            if (runHeadless)
                options.AddArgument("--headless");
            
            Driver = new ChromeDriver(options);
        }

        protected async Task RunTest(Func<ChromeDriver, Task> execute)
        {
            try
            {
                await execute(Driver);
            }
            catch (Exception e)
            {
                throw;
            }
            finally
            {
                Driver.Close();
            }
        }
    }
}