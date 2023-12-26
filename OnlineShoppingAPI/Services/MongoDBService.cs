using OnlineShopping.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Bson;
using OnlineShopping.Collection;
using DnsClient;
using OnlineShoppingAPI.Collection;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Hosting;
using System;
using System.Text.Json;
using System.Diagnostics;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.VisualBasic;
using NLog.Fluent;
using static Confluent.Kafka.ConfigPropertyNames;

namespace OnlineShopping.Services
{
    public class MongoDBService
    {
        private readonly IMongoCollection<Registration> _registrationCollection;
        private readonly IMongoCollection<Login> _loginCollection;
        private readonly IMongoCollection<Products> _productsCollection;
        private readonly IMongoCollection<Orders> _ordersCollection;

        public MongoDBService(IOptions<MongoDBSettings> mongoDBSettings)
        {
            MongoClient client = new MongoClient(mongoDBSettings.Value.ConnectionString);
            IMongoDatabase database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _registrationCollection = database.GetCollection<Registration>(mongoDBSettings.Value.FirstCollectionName);
            _loginCollection = database.GetCollection<Login>(mongoDBSettings.Value.SecondCollectionName);
            _productsCollection = database.GetCollection<Products>(mongoDBSettings.Value.ThirdCollectionName);
            _ordersCollection = database.GetCollection<Orders>(mongoDBSettings.Value.FourthCollectionName);
        }
        public async Task<List<Registration>> GetAllUsers() {
            return await _registrationCollection.Find(new BsonDocument()).ToListAsync();

        }
        private bool IsLoginidExists(string loginid)
        {
            bool exists = _registrationCollection.Find(e => e.Loginid == loginid).Any();
            return exists;
        }
        private bool IsEmaildExists(string email)
        {
            bool exists = _registrationCollection.Find(e => e.Email == email).Any();
            return exists;
        }

        public async Task<string> CreateUser(Registration registration, Login login)
        {
            //await _registrationCollection.InsertOneAsync(registration);
            if ((!IsLoginidExists(registration.Loginid)))
            {
                if (!IsEmaildExists(registration.Email))
                {
                    if (registration.Password == registration.Confirmpassword && registration.Password==login.Password)
                    {
                        await _registrationCollection.InsertOneAsync(registration);
                        await _loginCollection.InsertOneAsync(login);
                        return "User Created Successfully!";
                    }
                    else
                    {
                        return "Passwords must match!";
                    }
                }
                else
                {
                    return "Email id already exists!";
                }
            }
            else
            {
                return "Login id already exists!";

            }
        }

        public async Task<List<Login>> GetLoginUsers()
        {
            return await _loginCollection.Find(new BsonDocument()).ToListAsync();

        }
        public async Task CreateUserLogin(Login login)
        {
            await _loginCollection.InsertOneAsync(login);
            return;
        }
        
        public async Task<string> SearchPassword(string fname, Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);

            if (IsAdminTrue)
            {
                var filter = Builders<Registration>.Filter.Eq("Fname", fname);
                var result = await _registrationCollection.Find(filter).FirstOrDefaultAsync();
                if (result != null)
                {
                    string password = result.Password;
                    return password;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return "User not allowed!";
            }          
        }


        public async Task<List<Products>> GetProductAll()
        {
            return await _productsCollection.Find(new BsonDocument()).ToListAsync();

        }
        public async Task<List<Products>> SearchProduct(string searchProd)
        {
            var queryExpr = new BsonRegularExpression(new Regex(searchProd, RegexOptions.IgnoreCase));
            var filter = Builders<Products>.Filter.Regex("ProductName", queryExpr);
            return await _productsCollection.Find(filter).ToListAsync();

        }


        public async Task<string> CreateProduct(Products products,Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);

            if (IsAdminTrue)
            {
                await _productsCollection.InsertOneAsync(products);
                return "Product Created!";
            }
            else
            {
                return "User not allowed!";
            }
        }

        public async Task<string> UpdateProduct(Products product,Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid,login.Password);
            if (IsAdminTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("ProductId", product.ProductId);
                UpdateDefinition<Products> updateStockCount = Builders<Products>.Update.Set("StockCount", product.StockCount);
                var res="";
                if (product.StockCount == 0)
                {
                    UpdateDefinition<Products> updateProductStatus = Builders<Products>.Update.Set("ProductStatus", "Out of Stock");
                    await _productsCollection.UpdateOneAsync(filter, updateProductStatus);
                    await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                    res = (product.ProductId +" : "+ product.ProductName +" status is "+"Out of Stock");
                }
                else if ((product.StockCount > 0) && (product.StockCount <= 5))
                {
                    UpdateDefinition<Products> updateProductStatus = Builders<Products>.Update.Set("ProductStatus", "Hurry up tp Purchase");
                    await _productsCollection.UpdateOneAsync(filter, updateProductStatus);
                    await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                    res = (product.ProductId +" : "+ product.ProductName +" status is "+"Hurry up tp Purchase");
                }
                else
                {
                    UpdateDefinition<Products> updateProductStatus = Builders<Products>.Update.Set("ProductStatus", "Available");
                    await _productsCollection.UpdateOneAsync(filter, updateProductStatus);
                    await _productsCollection.UpdateOneAsync(filter, updateStockCount);
                    res = (product.ProductId +" : "+ product.ProductName +" status is "+"Available");
                }
                return (res);

            }
            else
            {
                return null;
            }
           
        }
        public async Task<string> DeleteProduct(int ProductId, Login login) 
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("ProductId", ProductId);
                await _productsCollection.DeleteOneAsync(filter);
                return "Product with ID " + ProductId + " deleted successfully";
            }
            else
            {
                return "Product with ID " +ProductId+" deletion failed";
            }

            
        }
        

        private async Task<bool> IsUserAdmin(string loginid,string password)

        {

            var filter1 = Builders<Login>.Filter.Where(x => x.Loginid == loginid && x.Role == "Admin" && x.Password==password);


            var loginCheck = await _loginCollection.Find(filter1).FirstOrDefaultAsync();



            if (loginCheck != null)

            {

                return true;

            }

            else

            {

                return false;

            }

        }

        public async Task<bool> ProductExists(int pid, Orders order)
        {
            var filter1 = Builders<Products>.Filter.Eq(x => x.ProductId, pid);
            var orderIdCheck = _productsCollection.Find(p => p.ProductId == order.ProductId).Any();
            //var filter2 = Builders<Orders>.Filter.Eq(x => x.ProductId,pid);
            //bool orderCheck =  _ordersCollection.Find(filter2).Equals(filter1);
            //var ProductPid = await _productsCollection.Find(filter1).FirstOrDefaultAsync();           
            return orderIdCheck;
        }
        private async Task<bool> IsUserExist(string loginid,string password)
        {
            var filter1 = Builders<Login>.Filter.Where(x => x.Loginid == loginid && x.Role == "User" && x.Password==password);
            var loginCheck = await _loginCollection.Find(filter1).FirstOrDefaultAsync();



            if (loginCheck != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<string> CreateOrder(Orders orders, Products product, Login login)
        {
            bool IsUserTrue = await IsUserExist(login.Loginid,login.Password);
            bool IsProdTrue = await ProductExists(product.ProductId, orders);
            if (IsProdTrue && IsUserTrue)
            {
                FilterDefinition<Products> filter = Builders<Products>.Filter.Eq("StockCount", product.StockCount);
                var dec = product.StockCount -1;
                UpdateDefinition<Products> update = Builders<Products>.Update.Set("StockCount", dec);
                await _productsCollection.UpdateOneAsync(filter, update);
                await _ordersCollection.InsertOneAsync(orders);
                return "Order created";
            }
            else
            {
                return "Failed";
            }
        }
        public async Task<List<Orders>> GetOrdersPlaced(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid,login.Password);
            if (IsAdminTrue)
            {
                return await _ordersCollection.Find(new BsonDocument()).ToListAsync();

            }
            else
            {
                return null;
            }


        }

        public async Task<string> GetOrderCount(Login login,string productName)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                //var queryExpr = new BsonRegularExpression(new Regex(productName, RegexOptions.IgnoreCase));
                var filter = Builders<Orders>.Filter.Eq("ProductName", productName);
                var count = _ordersCollection.Find(filter).Count();
                return "Number of "+ productName +":" + Convert.ToInt32(count);

            }
            else
            {
                return null;
            }


        }
        public async Task<List<Products>> GetAvailable(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                var filter1 = Builders<Products>.Filter.Where(x => x.ProductStatus== "Hurry up to purchase");
                return await _productsCollection.Find(filter1).ToListAsync();
            }
            else
            {
                return null;
            }
        }
        public async Task<List<string>> GetStock(Login login)
        {
            bool IsAdminTrue = await IsUserAdmin(login.Loginid, login.Password);
            if (IsAdminTrue)
            {
                var filter = Builders<Products>.Filter.Exists(x => x.StockCount);
                var list = await _productsCollection.Find(filter).ToListAsync();
                List<string> plist = new List<string>();
                foreach (var prd in list)
                {
                    plist.Add(prd.ProductName +" : "+prd.StockCount);
                }
                return plist;
            }
            else
            {
                return null;
            }
        }


    }
}
