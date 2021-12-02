using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Contracts.Infrastructure;
using Ordering.Application.Contracts.Persistence;
using Ordering.Application.Models;
using Ordering.Domain.Common;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Commands.CheckoutOrder
{
    public class CheckoutCommandHandler : IRequestHandler<CheckoutOrderCommand, int>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly ILogger<CheckoutCommandHandler> _logger;

        public CheckoutCommandHandler(IOrderRepository orderRepository, IMapper mapper,
            IEmailService emailService, ILogger<CheckoutCommandHandler> logger)
        {
            _orderRepository = orderRepository;
            _mapper = mapper;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<int> Handle(CheckoutOrderCommand request, CancellationToken cancellationToken)
        {
            var orderEntity = _mapper.Map<Order>(request);

            var newOrder = await _orderRepository.AddAsync(orderEntity);

            _logger.LogInformation($"new order with id {newOrder.Id} has successfully been created");

            await SendMail(newOrder);

            return newOrder.Id;
        }

        private async Task SendMail(EntityBase order)
        {
            var email = new Email
            {
                To = "nuriszka@gmail.com", Body = "An order was created", Subject = "New order"
            };

            try
            {
                await _emailService.SendEmail(email);
            }
            catch (Exception e)
            {
                _logger.LogError($"Order {order.Id} failed due to an error with the mail service : {e.Message}");
                throw;
            }
        }
    }
}
