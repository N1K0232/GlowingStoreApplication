using AutoMapper;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Mapping;

public class ProductMapperProfile : Profile
{
    public ProductMapperProfile()
    {
        CreateMap<Entities.Product, Product>().ForMember(p => p.Category, options => options.MapFrom(p => p.Category.Name));
        CreateMap<SaveProductRequest, Entities.Product>();
    }
}