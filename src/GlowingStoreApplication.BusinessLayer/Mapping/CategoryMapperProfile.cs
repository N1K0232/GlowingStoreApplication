using AutoMapper;
using GlowingStoreApplication.Shared.Models;
using GlowingStoreApplication.Shared.Models.Requests;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Mapping;

public class CategoryMapperProfile : Profile
{
    public CategoryMapperProfile()
    {
        CreateMap<Entities.Category, Category>();
        CreateMap<SaveCategoryRequest, Entities.Category>();
    }
}