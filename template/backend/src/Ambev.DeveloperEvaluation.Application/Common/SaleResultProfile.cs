using Ambev.DeveloperEvaluation.Domain.Entities;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Common
{
    public class SaleResultProfile : Profile
    {
        public SaleResultProfile()
        {
            CreateMap<Sale, SaleResult>()
                .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new ExternalIdentityResult
                {
                    Id = src.CustomerId,
                    Name = src.CustomerName
                }))
                .ForMember(dest => dest.Branch, opt => opt.MapFrom(src => new ExternalIdentityResult
                {
                    Id = src.BranchId,
                    Name = src.BranchName
                }));

            CreateMap<SaleItem, SaleItemResult>()
                .ForMember(dest => dest.Product, opt => opt.MapFrom(src => new ExternalIdentityResult
                {
                    Id = src.ProductId,
                    Name = src.ProductName
                }));
        }
    }
}
