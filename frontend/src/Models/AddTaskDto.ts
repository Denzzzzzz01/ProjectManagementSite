import { Priority } from "../Enums/PriorityEnum";

export interface AddTaskDto {
    projectId: string;
    title: string;
    description: string;
    priority: Priority;
  }