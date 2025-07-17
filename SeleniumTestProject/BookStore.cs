using Newtonsoft.Json;
using OpenQA.Selenium.DevTools.V85.ApplicationCache;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TestProject.BookStore;
using static TestProject.BookStore.User;

namespace TestProject.BookStore;
public class User
{
    public string userName { get; set; }
    public string password { get; set; }
    public string userId { get; set; }
    public string token { get; set; }
    public class LoginData
    {
        public string userName;
        public string password;

        public LoginData(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }
    }

    public class UserCreateResponse
    {
        public string userID;
        public string userName;
        public List<Object> books;
    }

    public class GenerateTokenResponse
    {
        public string token;
        public string expires;
        public string status;
        public string result;
    }

    public class GetUserResponse
    {
        public string userId;
        public string username;
        public List<Book> books;
    }

    public static async Task<User> CreateUser()
    {
        var userName = "jednaka";
        var password = "Password!123";
        LoginData loginData = new LoginData(userName, password);

        var url = $"https://bookstore.toolsqa.com/Account/v1/User";
        using var client = new HttpClient();

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            UserCreateResponse? userResponse = JsonConvert.DeserializeObject<UserCreateResponse>(responseString);
            if (userResponse != null)
            {
                var user = new User();
                user.userName = userName;
                user.password = password;
                user.userId = userResponse.userID;               
                return user;
            }
        }
        return null;

    }

    public static async Task GenerateToken(User user)
    {
        LoginData loginData = new LoginData(user.userName, user.password);
        var url = $"https://bookstore.toolsqa.com/Account/v1/GenerateToken";
        using var client = new HttpClient();

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            var generateTokenResponse = JsonConvert.DeserializeObject<GenerateTokenResponse>(responseString);
            if (generateTokenResponse != null)
                user.token = generateTokenResponse.token;
        }
    }

    public static async Task LogIn(User user)
    {
        LoginData loginData = new LoginData(user.userName, user.password);

        var url = $"https://bookstore.toolsqa.com/Account/v1/Login";
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);

        var json = JsonConvert.SerializeObject(loginData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content);
        var responseString = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseString);
    }

    public static async Task DeleteUser(User user)
    {
        var url = $"https://bookstore.toolsqa.com/Account/v1/User/" + user.userId;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);

        var response = await client.DeleteAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
        }
    }

    public static async Task<List<Book>> GetUserBooks(User user)
    {
        var url = $"https://bookstore.toolsqa.com/Account/v1/User/" + user.userId;
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(json);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var getUserResponse = System.Text.Json.JsonSerializer.Deserialize<GetUserResponse>(json, options);
            if (getUserResponse != null)
                return getUserResponse.books;
        }

        return new List<Book>();
    }
}

public class Book
{
    public string isbn { get; set; }
    public string title { get; set; }
    public string subTitle { get; set; }
    public string author { get; set; }
    public DateTime publish_date { get; set; }
    public string publisher { get; set; }
    public int pages { get; set; }
    public string description { get; set; }
    public string website { get; set; }

    public class BookResponse
    {
        public List<Book> books { get; set; }
    }

    public class CollectionOfBooks
    {
        public string userId;
        public List<IsbnItem> collectionOfIsbns { get; set; }
    }

    public class IsbnItem
    {
        public string isbn { get; set; }
    }

    public static async Task GetBook(string isbn)
    {
        string url = "https://bookstore.toolsqa.com/BookStore/v1/Book/" + isbn;
        using HttpClient client = new HttpClient();

        HttpResponseMessage response = await client.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            string json = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(json);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var book = System.Text.Json.JsonSerializer.Deserialize<Book>(json, options);
            Console.WriteLine($"Title: {book.title}, Author: {book.author}");

        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }





    public static async Task<List<Book>> GetBooks(User user)
    {
        string url = "https://bookstore.toolsqa.com/BookStore/v1/Books";
        using HttpClient client = new HttpClient();
        BookResponse bookResponse = null;
        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                Console.WriteLine(json);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                try
                {
                    bookResponse = System.Text.Json.JsonSerializer.Deserialize<BookResponse>(json, options);
                    if (bookResponse != null)
                    {
                        foreach (var book in bookResponse.books)
                        {
                            Console.WriteLine($"Title: {book.title}, Author: {book.author}");
                        }
                        return bookResponse.books;
                    }
                    else
                    {
                        Console.WriteLine("No books found.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Deserialization error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Error: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
       
        return new List<Book>();

    }

    public static async Task AddBook(User user, Book book)
    {
        string url = "https://bookstore.toolsqa.com/BookStore/v1/Books";
        using HttpClient client = new HttpClient();
        var collectionOfBooks = new CollectionOfBooks();
        collectionOfBooks.userId = user.userId;
        collectionOfBooks.collectionOfIsbns = new List<IsbnItem>();
        var isbnItem = new IsbnItem();
        isbnItem.isbn = book.isbn;
        collectionOfBooks.collectionOfIsbns.Add(isbnItem);

        try
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.token);
            var json = JsonConvert.SerializeObject(collectionOfBooks);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine(jsonResponse);
            }
            else
            {
                Assert.Fail($"Response code: {response.StatusCode.ToString()}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }
}