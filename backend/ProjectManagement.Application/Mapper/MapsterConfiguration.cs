using Mapster;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Mapper;

public static class MapsterConfiguration
{
    public static void Configure()
    {
        TypeAdapterConfig<Project, ProjectDetailedVm>.NewConfig()
            .Map(dest => dest.Tasks, src => src.Tasks.Adapt<List<TaskVm>>());
    }
}
