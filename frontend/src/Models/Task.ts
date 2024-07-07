import { Priority } from "../Enums/PriorityEnum";

export interface Task {
    id: string;
    title: string;
    addedTime: string;
    isDone: boolean;
    priority: Priority;
  }