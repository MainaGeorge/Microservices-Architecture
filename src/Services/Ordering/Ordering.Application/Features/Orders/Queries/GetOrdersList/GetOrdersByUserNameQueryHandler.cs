using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistence;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList
{
    public class GetOrdersByUserNameQueryHandler : IRequestHandler<GetOrdersByUserNameQuery, List<OrdersVm>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _repo;

        public GetOrdersByUserNameQueryHandler(IMapper mapper, IOrderRepository repo)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        }
        public async Task<List<OrdersVm>> Handle(GetOrdersByUserNameQuery request, CancellationToken cancellationToken)
        {
            var orders = await _repo.GetOrdersByUserName(request.UserName);

            return _mapper.Map<List<OrdersVm>>(orders);
        }
    }
}
