using System;
using System.Collections.Generic;
using MediatR;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersByUserNameQuery : IRequest<List<OrdersVm>>
    {
        public string UserName { get; set; }

        public GetOrdersByUserNameQuery(string userName)
        {
            UserName = userName ?? throw new ArgumentException($"{nameof(userName)} cant be null",
                nameof(userName));
        }
    }
}
