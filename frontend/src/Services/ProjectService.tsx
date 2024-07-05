import axios from 'axios';
import { API_BASE_URL } from '../config';
import { ProjectVm } from '../Models/ProjectVm';

export const getUserProjects = async (): Promise<ProjectVm[]> => {
  const response = await axios.get(`${API_BASE_URL}/project/GetUserProjects`);
  return response.data;
};

export const createProject = async (projectName: string): Promise<ProjectVm> => {
  const response = await axios.post(`${API_BASE_URL}/project/CreateProject`, { name: projectName });
  return response.data;
};

export const deleteProject = async (projectId: string): Promise<void> => {
  await axios.delete(`${API_BASE_URL}/project/DeleteProject`, {
    params: { ProjectId: projectId }
  });
};

