using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public  class SearchPage
    {
        public void PerformSearchAndValidate(IWebDriver driver)
        {
            IWebElement webElementSearch = driver.FindElement(By.Id("store_nav_search_term"));
            webElementSearch.SendKeys("FIFA");
            webElementSearch.SendKeys(Keys.Return);
            System.Threading.Thread.Sleep(1000);

            var webElementSearchList = driver.FindElements(By.ClassName("responsive_search_name_combined"));
            Assert.AreEqual(webElementSearchList[0].FindElement(By.XPath(".//span")).Text, "EA SPORTS FC™ 25");
            Assert.AreEqual(webElementSearchList[1].FindElement(By.XPath(".//span")).Text, "FIFA 22");

            webElementSearchList.ElementAt(0).Click();
            System.Threading.Thread.Sleep(1000);
            Assert.IsTrue(driver.Title.Substring(0).StartsWith("EA SPORTS FC™ 25"));
            var webElementGameName = driver.FindElement(By.ClassName("apphub_AppName"));
            Assert.AreEqual(webElementGameName.Text, "EA SPORTS FC™ 25");

        }

        public void ClickDownloadAndValidate(IWebDriver driver)
        {
            var webElementDownload = driver.FindElement(By.XPath("//div[@id='demoGameBtn']/a"));
            webElementDownload.Click();
            System.Threading.Thread.Sleep(1000);
            var webElementNoINeedSteam = driver.FindElements(By.XPath(".//div[@class='gotsteam_buttons']/a"))[1];
            webElementNoINeedSteam.Click();
            System.Threading.Thread.Sleep(1000);
            var webElementInstallSteam = driver.FindElement(By.ClassName("about_install_steam_link"));
            Assert.IsTrue(webElementInstallSteam.Enabled);
        }

    }

    public class SteamPage
    {

        public void ValidateSteam(IWebDriver driver)
        {

            var onlineGamers = driver.FindElements(By.ClassName("online_stat"))[0].Text;
            var onlineNumberString = onlineGamers.Split('\n')[1].Split(',');
            string onlineNumber = ""; ;
            for (int i = 0; i < onlineNumberString.Length; i++)
            {
                onlineNumber = onlineNumber + onlineNumberString[i];
            }
            var intOnlineNumber = int.Parse(onlineNumber);

            var playingNowGamers = driver.FindElements(By.ClassName("online_stat"))[1].Text;
            var playingNumberString = playingNowGamers.Split('\n')[1].Split(',');
            string playingNumber = ""; ;
            for (int i = 0; i < playingNumberString.Length; i++)
            {
                playingNumber = playingNumber + playingNumberString[i];
            }
            var intPlayingNumber = int.Parse(playingNumber);

            Assert.IsTrue(intPlayingNumber < intOnlineNumber);
        }
    }
}
