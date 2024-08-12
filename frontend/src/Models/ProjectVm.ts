import { Status } from "../Enums/StatusEnum";

export interface ProjectVm {
  id: string;
  name: string;
  status: Status;
  description: string;
}
