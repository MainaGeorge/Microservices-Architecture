using System.Net.Http;
using System.Threading.Tasks;
using Shopping.Aggregator.Models;
using Shopping.Aggregator.Services.Interfaces;

namespace Shopping.Aggregator.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _client;

        public BasketService(HttpClient client)
        {
            _client = client;
        }
        public Task<BasketModel> GetBasketByUserName(string userName)
        {
            throw new System.NotImplementedException();
        }
    }
}
