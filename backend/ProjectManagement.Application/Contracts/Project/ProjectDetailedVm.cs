﻿using ProjectManagement.Application.Contracts.Task;
using ProjectManagement.Core.Enums;

namespace ProjectManagement.Application.Contracts.Project;

public class ProjectDetailedVm
{
    public Guid Id { get; set; }    
    public Guid OwnerId { get; set; }

    public DateTime CreatedTime { get; set; }
    public string Name { get; set; }
    public List<TaskVm> Tasks { get; set; }
    public Status Status { get; set; }
    public string Description { get; set; }
}
