using AutoMapper;
using GlowingStoreApplication.Shared.Models;
using Entities = GlowingStoreApplication.DataAccessLayer.Entities;

namespace GlowingStoreApplication.BusinessLayer.Mapping;

public class ImageMapperProfile : Profile
{
    public ImageMapperProfile()
    {
        CreateMap<Entities.Image, Image>();
    }
}