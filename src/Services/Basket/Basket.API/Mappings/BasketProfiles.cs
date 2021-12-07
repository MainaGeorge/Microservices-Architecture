using AutoMapper;
using Basket.API.Entities;
using EventBus.Messages.Events;

namespace Basket.API.Mappings
{
    public class BasketProfiles : Profile
    {
        public BasketProfiles()
        {
            CreateMap<BasketCheckout, BasketCheckoutEvent>()
                .ReverseMap();
        }
    }
}
