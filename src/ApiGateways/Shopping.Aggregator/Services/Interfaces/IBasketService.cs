using System.Threading.Tasks;
using Shopping.Aggregator.Models;

namespace Shopping.Aggregator.Services.Interfaces
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasketByUserName(string userName);
    }
}
