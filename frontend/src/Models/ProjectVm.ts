import { Status } from "../Enums/StatusEnum";

export interface ProjectVm {
  id: string;
  createdTime: string;
  name: string;
  status: Status;
  description: string;
}
