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

        //public MongoDBService _service;
        //public shoppingController _controller;
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
            //var c1 = db.GetCollection<Registration>(mongoDBSettings.Value.FirstCollectionName);
            //var c2 = db.GetCollection<Login>(mongoDBSettings.Value.SecondCollectionName);
            //var c3 = db.GetCollection<Products>(mongoDBSettings.Value.ThirdCollectionName);
            //var c4 = db.GetCollection<Orders>(mongoDBSettings.Value.FourthCollectionName);
            
        

        }
        [Test]
        public void Test1(MongoDBService mongoDBService)
        {
            //var response1 = _httpClient.GetAsync("api/v1.0/shopping/login");
            //var result = _controller.GetLogin();
            //Assert
            //Xunit.Assert.Equal(System.Net.HttpStatusCode.OK, response1.Status);
            //Xunit.Assert.True(response1.ToString().Equals(HttpStatusCode.OK));
            var ls = mongoDBService.GetAllUsers();
            Console.WriteLine(ls);
            var users = ls.Result as List<Registration>;
            Console.WriteLine("hi");
            Xunit.Assert.NotNull(users);

            //var list = result.Result;
            //Xunit.Assert.Equal(3,list.Count);

        }
    }
}