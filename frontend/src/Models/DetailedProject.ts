import { Status } from "../Enums/StatusEnum";
import { Task } from "./Task";

export interface DetailedProject {
  id: string;
  ownerId: string;
  name: string;
  createdTime: string;
  status: Status;
  tasks: Task[];
  description: string; 
}
