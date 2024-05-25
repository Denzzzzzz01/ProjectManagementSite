using Mapster;
using ProjectManagement.Application.Contracts.Project;
using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Application.Common.Mapping;

public static class MappingConfiguration
{
    public static void Configure()
    {
        TypeAdapterConfig<Project, ProjectDetailedVm>.NewConfig()
            .Map(dest => dest.Tasks, src => src.Tasks.Adapt<List<TaskVm>>());
    }
}
