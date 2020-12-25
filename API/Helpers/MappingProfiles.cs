using API.Dtos;
using AutoMapper;
using Core.Entities;
using Core.Entities.Identity;
using Core.Entities.OrderAggregate;

namespace API.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.ProductBrand, o => o.MapFrom(s => s.ProductBrand.Name))
                .ForMember(d => d.ProductType, o => o.MapFrom(s => s.ProductType.Name))
                .ForMember(d => d.PictureUrl, o => o.MapFrom<ProductUrlResolver>());

            // create map for Address and AddressDto classes
            // it is enought for map this tho classes because they have properties with the same names
            // ReverseMap() map at the other way
            CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();

            CreateMap<CustomerBasketDto, CustomerBasket>();

            CreateMap<BasketItemDto, BasketItem>();

            // Core.Entities.OrderAggregate.Address
            // because i have Address class with the same name in Core.Entities.Identity
            CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();


            CreateMap<Order, OrderToReturnDto>()
                     .ForMember(d => d.DeleveryMethod, o => o.MapFrom(s => s.DeleveryMethod.ShortName))
                     .ForMember(d => d.ShippingPrice, o => o.MapFrom(s => s.DeleveryMethod.Price));

            CreateMap<OrderItem, OrderItemDto>()
                     .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ItemOrdered.ProductItemId))
                     .ForMember(d => d.ProductName, o => o.MapFrom(s => s.ItemOrdered.ProductName))
                     .ForMember(d => d.PictureUrl, o => o.MapFrom(s => s.ItemOrdered.PictureUrl))
                     .ForMember(d => d.PictureUrl, o => o.MapFrom<OrderItemUrlResolver>());
        }
    }
}
