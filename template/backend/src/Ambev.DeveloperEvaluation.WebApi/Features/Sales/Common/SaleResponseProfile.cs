using Ambev.DeveloperEvaluation.Application.Common;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.Common
{
    public class SaleResponseProfile : Profile
    {
        public SaleResponseProfile()
        {
            CreateMap<SaleResult, SaleResponse>();
            CreateMap<SaleItemResult, SaleItemResponse>();
            CreateMap<ExternalIdentityResult, ExternalIdentityResponse>();
        }
    }
}
