using AutoMapper;
using Movies.API.DTO;
using Movies.API.Model;

namespace Movies.API.MapperProfiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        // source --> destination
        CreateMap<Movie, MovieReadDto>();
        CreateMap<MovieCreateDto, Movie>();
    }
}