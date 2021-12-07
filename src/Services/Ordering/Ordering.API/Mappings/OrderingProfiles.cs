using AutoMapper;
using EventBus.Messages.Events;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;

namespace Ordering.API.Mappings
{
    public class OrderingProfiles : Profile
    {
        public OrderingProfiles()
        {
            CreateMap<CheckoutOrderCommand, BasketCheckoutEvent>()
                .ReverseMap();
        }
    }
}
