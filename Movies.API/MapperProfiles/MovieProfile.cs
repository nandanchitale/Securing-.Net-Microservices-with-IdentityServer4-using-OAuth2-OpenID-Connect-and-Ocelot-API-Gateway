using AutoMapper;
using Movies.API.DTO;
using Movie = Movies.Models.Movies;

namespace Movies.API.MapperProfiles;

public class MovieProfile : Profile
{
    public MovieProfile()
    {
        // source --> destination
        CreateMap<Movie, MovieReadDto>();
        CreateMap<MovieDetailDto, Movie>();
    }
}