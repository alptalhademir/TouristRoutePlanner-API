using AutoMapper;
using TouristRoutePlanner.API.DTOs;
using TouristRoutePlanner.API.Models;

namespace TouristRoutePlanner.API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Place, PlaceDto>()
                .ForMember(dest => dest.Types, opt => opt.MapFrom(src =>
                src.PlaceTypes.Select(pt => pt.Type.Name)))
                .ReverseMap();

            CreateMap<AddPlaceRequestDto, Place>()
                .ForMember(dest => dest.PlaceTypes, opt => opt.MapFrom(src =>
                    src.Types.Select(typeName => new PlaceType
                    {
                        Type = new Models.Type
                        {
                            Name = typeName
                        }
                    })))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.TravelPlaces, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<UpdatePlaceRequestDto, Place>()
                .ForMember(dest => dest.PlaceTypes, opt => opt.MapFrom(src =>
                    src.Types.Select(typeName => new PlaceType
                    {
                        Type = new Models.Type
                        {
                            Name = typeName
                        }
                    })))
                .ForMember(dest => dest.TravelPlaces, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Models.Type, TypeDto>().ReverseMap();
            CreateMap<AddTypeRequestDto, Models.Type>().ReverseMap();
            CreateMap<UpdateTypeRequestDto, Models.Type>().ReverseMap();
            CreateMap<Distance, DistanceDto>().ReverseMap();
            CreateMap<AddDistanceRequestDto, Distance>().ReverseMap();
            CreateMap<UpdateDistanceRequestDto, Distance>().ReverseMap();

            CreateMap<Travel, TravelDto>()
                .ForMember(dest => dest.SelectedTypes, opt => opt.MapFrom(src =>
                    src.TravelTypes.Select(tt => tt.Type)))
                .ForMember(dest => dest.SelectedPlaces, opt => opt.MapFrom(src =>
                    src.TravelPlaces.Select(tp => tp.Place)))
                .ReverseMap();

            CreateMap<AddTravelRequestDto, Travel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.TravelTypes, opt => opt.MapFrom(src =>
                    src.TypeIds.Select(typeId => new TravelType
                    {
                        TypeId = typeId
                    })))
                .ForMember(dest => dest.TravelPlaces, opt => opt.MapFrom(src =>
                    src.PlaceIds.Select(placeId => new TravelPlace
                    {
                        PlaceId = placeId
                    })));

            CreateMap<UpdateTravelRequestDto, Travel>()
                .ForMember(dest => dest.TravelTypes, opt => opt.MapFrom(src =>
                    src.TypeIds.Select(typeId => new TravelType
                    {
                        TypeId = typeId
                    })))
                .ForMember(dest => dest.TravelPlaces, opt => opt.MapFrom(src =>
                    src.PlaceIds.Select(placeId => new TravelPlace
                    {
                        PlaceId = placeId
                    })));

        }
    }
}
