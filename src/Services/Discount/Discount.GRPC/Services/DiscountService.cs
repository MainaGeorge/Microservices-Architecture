using AutoMapper;
using Discount.GRPC.Entities;
using Discount.GRPC.Protos;
using Discount.GRPC.Repository;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.GRPC.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase    
    {
        private readonly ILogger<DiscountService> _logger;
        private readonly IDiscountRepository _repo;
        private readonly IMapper _mapper;

        public DiscountService(ILogger<DiscountService> logger, IDiscountRepository repo,
            IMapper mapper)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override async Task<CouponModel> 
            GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repo.GetDiscount(request.ProductName);

            if(coupon is null)
            {
                _logger.LogError($"no discount found for the products {request.ProductName}");
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with the product name {request.ProductName} was not found"));
            }

            _logger.LogInformation($"discount retrieved for products {request.ProductName} in the amount of {coupon.Amount}");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.CouponModel);

            var success = await _repo.CreateDiscount(coupon); 

            if(!success)
                throw new RpcException(new Status(StatusCode.NotFound, $"Failed to create the discount for {request.CouponModel.ProductName}"));

            _logger.LogInformation($"Successfully created the discount for {request.CouponModel.ProductName} ");
            return _mapper.Map<CouponModel>(coupon);

        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var coupon = _mapper.Map<Coupon>(request.CouponModel);

            if (!await _repo.UpdateDiscount(coupon))
                throw new RpcException(new Status(StatusCode.NotFound, $"Failed to update the discount for {request.CouponModel.ProductName}"));

            _logger.LogInformation($"Successfully updated the discount for {request.CouponModel.ProductName} ");
            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            if(!await _repo.DeleteDiscount(request.ProductName))
                throw new RpcException(new Status(StatusCode.NotFound, $"Failed to delete the discount for {request.ProductName}"));

            _logger.LogInformation($"Successfully deleted the discount for {request.ProductName}");
            return new DeleteDiscountResponse { Success = true };

        }
    }
}
