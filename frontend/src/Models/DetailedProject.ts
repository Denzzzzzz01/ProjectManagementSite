import { Status } from "../Enums/StatusEnum";
import { Task } from "./Task";

export interface DetailedProject {
  id: string;
  name: string;
  createdTime: string;
  status: Status;
  tasks: Task[];
}