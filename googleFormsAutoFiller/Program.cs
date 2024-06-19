using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Collections.Generic;

class Program
{
    static void Main(string[] args)
    {
        IWebDriver driver = null;
        try
        {
            var options = new ChromeOptions();

            string chromeDriverPath = AppDomain.CurrentDomain.BaseDirectory;

            driver = new ChromeDriver(chromeDriverPath, options);

            string formUrl = "https://forms.gle/LCCAmPhzFD5hfKd37";
            driver.Navigate().GoToUrl(formUrl);

            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));

            wait.Until(drv => drv.FindElement(By.CssSelector("form")));

            Random random = new Random();

            var multipleChoiceQuestions = driver.FindElements(By.CssSelector("div[role='radiogroup']"));
            foreach (var question in multipleChoiceQuestions)
            {
                var radioOptions = question.FindElements(By.CssSelector("div[role='radio']"));
                if (radioOptions.Count > 1)
                {
                    int randomChoice = random.Next(radioOptions.Count);
                    wait.Until(drv => radioOptions[randomChoice].Displayed && radioOptions[randomChoice].Enabled);
                    ScrollIntoView(driver, radioOptions[randomChoice]);
                    radioOptions[randomChoice].Click();
                }
            }

            var checkboxQuestions = driver.FindElements(By.CssSelector("div[role='list']"));
            foreach (var question in checkboxQuestions)
            {
                var checkboxOptions = question.FindElements(By.CssSelector("div[role='checkbox']"));
                if (checkboxOptions.Count > 1)
                {
                    int randomChoicesCount = random.Next(1, checkboxOptions.Count + 1);
                    HashSet<int> chosenIndexes = new HashSet<int>();
                    while (chosenIndexes.Count < randomChoicesCount)
                    {
                        int randomChoice = random.Next(checkboxOptions.Count);
                        chosenIndexes.Add(randomChoice);
                    }
                    foreach (int index in chosenIndexes)
                    {
                        wait.Until(drv => checkboxOptions[index].Displayed && checkboxOptions[index].Enabled);
                        ScrollIntoView(driver, checkboxOptions[index]);
                        checkboxOptions[index].Click();
                    }
                }
            }

            var submitButton = wait.Until(drv => drv.FindElement(By.XPath("//span[contains(text(), 'გაგზავნა')]"))); // button to click is georgian in my chrome
            ScrollIntoView(driver, submitButton);
            submitButton.Click();

            wait.Until(drv => drv.FindElement(By.CssSelector("div[class*='freebirdFormviewerViewResponseConfirmationMessage']")));


        }
        catch (Exception ex)
        {
            Console.WriteLine($"{ex.Message}");
        }
        finally
        {
            if (driver != null)
            {
                try
                {
                    driver.Quit();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($" {ex.Message}");
                }
                finally
                {
                    driver.Dispose();
                }
            }
        }
    }

    static void ScrollIntoView(IWebDriver driver, IWebElement element)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        js.ExecuteScript("arguments[0].scrollIntoView({ block: 'center', inline: 'nearest' });", element);
    }
}