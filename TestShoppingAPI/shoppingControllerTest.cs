using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NUnit.Framework;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineShopping.Collection;
using OnlineShopping.Models;
using OnlineShopping.Services;
using OnlineShoppingAPI.Collection;
using OnlineShoppingAPI.Controllers;
using System.Collections.Generic;
using System.Net;

namespace TestShoppingAPI
{
    [TestFixture]
    public class shoppingControllerTest
    {


        IOptions<MongoDBSettings> _mongoDBSettings;
        MongoDBService mongoDBService;
        private readonly IMongoCollection<Registration> _registrationCollection;
        private readonly IMongoCollection<Login> _loginCollection;
        private readonly IMongoCollection<Products> _productsCollection;
        private readonly IMongoCollection<Orders> _ordersCollection;

        [SetUp]
        public void Setup()
        {
            mongoDBService = new MongoDBService(_mongoDBSettings);
            var client = new MongoClient("mongodb+srv://testuser:pwd@cluster0.n6nchhr.mongodb.net/?retryWrites=true&w=majority");
            var db = client.GetDatabase("OnlineShopping");
            
        

        }
        [Test]
        public void Test1(MongoDBService mongoDBService)
        {

            var ls = mongoDBService.GetAllUsers();
            Console.WriteLine(ls);
            var users = ls.Result as List<Registration>;
            Console.WriteLine("hi");
            Xunit.Assert.NotNull(users);


        }
    }
}