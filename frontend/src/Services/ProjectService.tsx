import axios from 'axios';
import { API_BASE_URL } from '../config';
import { ProjectVm } from '../Models/ProjectVm';
import { Status } from '../Enums/StatusEnum';
import { DetailedProject } from '../Models/DetailedProject';

export const getUserProjects = async (): Promise<ProjectVm[]> => {
  const response = await axios.get(`${API_BASE_URL}/Project/GetUserProjects`);
  console.log(response.data);
  return response.data;
};

export const getProjectById = async (projectId: string): Promise<DetailedProject> => {
  const response = await axios.get(`${API_BASE_URL}/Project/GetProjectById?projectId=${projectId}`);
  return response.data;
};

export const createProject = async (projectName: string, projectDescription: string): Promise<ProjectVm> => {
  const response = await axios.post(`${API_BASE_URL}/Project/CreateProject`, { name: projectName, description: projectDescription });
  return response.data;
};

export const deleteProject = async (projectId: string): Promise<void> => {
  await axios.delete(`${API_BASE_URL}/Project/DeleteProject`, {
    params: { ProjectId: projectId }
  });
};

export const updateProject = async (projectId: string, projectName: string, projectDescription: string): Promise<ProjectVm> => {
  const response = await axios.put(`${API_BASE_URL}/Project/UpdateProject`, { id: projectId, name: projectName, description: projectDescription });
  return response.data;
};

export const updateProjectStatus = async (projectId: string, newStatus: Status): Promise<void> => {
  await axios.put(`${API_BASE_URL}/Project/UpdateProjectStatus`, { id: projectId, status: newStatus });
};
