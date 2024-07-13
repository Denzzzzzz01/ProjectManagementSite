import { Priority } from "../Enums/PriorityEnum";

export interface UpdateTaskDto {
    title: string;
    description: string;
    priority: Priority;
  }