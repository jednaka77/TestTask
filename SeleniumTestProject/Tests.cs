using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools.V126.FedCm;
using Reqnroll;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using TestProject.BookStore;
using static TestProject.BookStore.Book;

namespace TestProject
{

    public class Tests
    {

        [SetUp]
        public void Setup()
        {
            
        }


        [Test]
        public void TestCase1()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("--incognito");
            options.AddArgument(@"--start-maximized");
            IWebDriver driver = new ChromeDriver(options);
            NavigateToPage(driver);

            SearchPage searchPage = new SearchPage();
            searchPage.PerformSearchAndValidate(driver);
            searchPage.ClickDownloadAndValidate(driver);

            SteamPage steamPage = new SteamPage();            
            steamPage.ValidateSteam(driver);

            driver.Close();
            driver.Quit();
        }

        public IWebDriver NavigateToPage(IWebDriver driver)
        {
            
            driver.Navigate().GoToUrl("https://store.steampowered.com/");
            return driver;
        }

        [Test]
        public async Task TestCase2()
        {
            User user = await User.CreateUser();
            await User.GenerateToken(user);
            await User.LogIn(user);


            List<Book> allBooks = await Book.GetBooks(user);
            Assert.IsTrue(allBooks.Count > 0);

            //await Book.GetBook("9781449325862");
            await Book.AddBook(user, allBooks[0]);
            List<Book> userBooks = await User.GetUserBooks(user);
            Assert.IsTrue(userBooks.Count == 1);
            Assert.IsTrue(allBooks[0].isbn ==  userBooks[0].isbn);

            await Book.AddBook(user, allBooks[1]);
            userBooks = await User.GetUserBooks(user);
            Assert.IsTrue(userBooks.Count == 1);
            Assert.IsTrue(allBooks[1].isbn == userBooks[0].isbn);

            await User.DeleteUser(user);
        }

        //public void TestCase2()
        //{
        //    {
        //        var bookResponse = Book.GetBooks();
        //    }
        //}
    }
}

    

