using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenCapture
{
    class Program
    {
        private static ChromeDriver _chrome;
        static void Main(string[] args)
        {
            try
            {
                _chrome = new ChromeDriver(@"../../artifacts");
                _chrome.Manage().Window.Maximize();
                _chrome.Navigate().GoToUrl("https://login.xfinity.com/login");
                var username = _chrome.FindElementById("user");
                username.SendKeys(ConfigurationManager.AppSettings["username"]);
                var password = _chrome.FindElementById("passwd");
                password.SendKeys(ConfigurationManager.AppSettings["password"]);
                var submit = _chrome.FindElementById("sign_in");
                submit.Click();
                Console.WriteLine("Waiting on page to load");
                var wifiButton = WaitUntilElementExists("xfinity xfi");
                Console.WriteLine("navigating to devices");
                _chrome.Navigate().GoToUrl("https://internet.xfinity.com/devices");
                System.Threading.Thread.Sleep(10000);
                try
                {
                    Console.WriteLine("try to close tour popup");
                    _chrome.ExecuteScript("document.getElementById('tour').style.display = 'none';");
                }
                catch(Exception ex) {
                    Console.WriteLine("failed during js exectuion for closing tour popup");
                }
                System.Threading.Thread.Sleep(2000);
                try
                {
                    Console.WriteLine("try to close tour overlay");
                    _chrome.ExecuteScript("document.getElementsByClassName('ReactModal__Overlay')[0].style.display = 'none';");
                }
                catch (Exception ex) {
                    Console.WriteLine("failed during js exectuion for closing tour overlay");
                }

                System.Threading.Thread.Sleep(3000);
                Console.WriteLine("zoom out");
                _chrome.ExecuteScript("document.body.style.zoom='67%'");
                System.Threading.Thread.Sleep(1000);
                Console.WriteLine("take screen shot");
                TakeScreenShot();
                Console.WriteLine("finished");
            }
            catch (NoSuchElementException e) {

            }
            catch (Exception e) {

            }
        }

        public static IWebElement WaitUntilElementExists(string elementName, int timeout = 100)
        {
            try
            {
                var wait = new WebDriverWait(_chrome, TimeSpan.FromSeconds(timeout));
                Func<IWebDriver, IWebElement> waitForElement = new Func<IWebDriver, IWebElement>((IWebDriver Web) =>
                {
                    return Web.FindElement(By.Name(elementName));
                });
                return wait.Until(waitForElement);
            }
            catch (NoSuchElementException ex)
            {
                throw new Exception("Element with locator: '" + elementName + "' was not found in current context page.", ex);
            }
        }

        private static void TakeScreenShot()
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
                bmp.Save(@"../../captures/screenshot.png");  // saves the image
            }
        }
    }
}
