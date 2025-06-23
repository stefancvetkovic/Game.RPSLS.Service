using AutoMapper;
using RpslsGameService.Application.DTOs;
using RpslsGameService.Domain;
using RpslsGameService.Domain.Enums;
using RpslsGameService.Domain.Models;

namespace RpslsGameService.Application.Mappings;

public class GameMappingProfile : Profile
{
    public GameMappingProfile()
    {
        CreateMap<Choice, ChoiceDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name));

        CreateMap<GameResult, GameResultResponse>()
            .ForMember(dest => dest.Results, opt => opt.MapFrom(src => MapOutcomeToString(src.Outcome)))
            .ForMember(dest => dest.Player, opt => opt.MapFrom(src => src.PlayerChoice.Id))
            .ForMember(dest => dest.Computer, opt => opt.MapFrom(src => src.ComputerChoice.Id))
            .ForMember(dest => dest.PlayerChoice, opt => opt.MapFrom(src => src.PlayerChoice.Name))
            .ForMember(dest => dest.ComputerChoice, opt => opt.MapFrom(src => src.ComputerChoice.Name))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => src.ResultMessage))
            .ForMember(dest => dest.PlayedAt, opt => opt.MapFrom(src => src.PlayedAt));
    }

    private static string MapOutcomeToString(GameOutcome outcome)
    {
        return outcome switch
        {
            GameOutcome.Win => Constants.GameOutcomes.Win,
            GameOutcome.Lose => Constants.GameOutcomes.Lose,
            GameOutcome.Tie => Constants.GameOutcomes.Tie,
            _ => Constants.GameOutcomes.Unknown
        };
    }
}