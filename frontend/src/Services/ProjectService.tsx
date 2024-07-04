import axios from "axios";
import { ProjectVm } from "../Models/ProjectVm";
import { API_BASE_URL } from "../config";

export const getUserProjects = async (): Promise<ProjectVm[]> => {
  const response = await axios.get<ProjectVm[]>(API_BASE_URL + "Project/GetUserProjects");
  return response.data;
};
