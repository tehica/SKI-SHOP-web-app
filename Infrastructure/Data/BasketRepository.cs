using Core.Entities;
using Core.Interfaces;
using System.Text.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient.Server;
using System.Collections.Immutable;

namespace Infrastructure.Data
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer redis)
        {
            // now we have got a connection to our database available for use
            _database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<CustomerBasket> GetBasketAsync(string basketId)
        {
            // baskets are gonna to stored as strings in Redis database
            var data = await _database.StringGetAsync(basketId);

            // if data is null or empty then return null or if we have data
            // then Deserialize that insto our Customer Basket
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data);
        }

        // this method is used for Create or Update basket
        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            // keep basket from someone user 30days in database -> ( TimeSpan.FromDays(30) )
            var created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), 
                TimeSpan.FromDays(30));

            // if basket is not already created
            if (!created)
                return null;

            return await GetBasketAsync(basket.Id);
        }
    }
}
