using Mapster;
using Microsoft.AspNetCore.Identity.Data;
using Voting_System.Contracts.Questions;
using Voting_System.Contracts.Votes;


namespace Voting_System.Mapping;

public class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Vote, VoteResponse>()
           .Map(dest => dest.Summary, sorc => sorc.Summary);
        config.NewConfig<RegisterRequest, ApplicatonUser>()
            .Map(dest => dest.UserName, src => src.Email);

        config.NewConfig<QuestionRequest, Question>()
            .Map(dest => dest.Answers, src => src.Answers.Select(answer => new Answer { Content = answer }));
    }
}
