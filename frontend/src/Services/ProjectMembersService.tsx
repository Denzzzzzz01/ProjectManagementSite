import axios from 'axios';
import { API_BASE_URL } from '../config';
import { AppUserDto } from '../Models/AppUserDto';

export const searchUsers = async (term: string): Promise<AppUserDto[]> => {
  const response = await axios.get(`${API_BASE_URL}/ProjectMembers/search`, {
    params: { term }
  });
  return response.data;
};

export const addUserToProject = async (projectId: string, userId: string): Promise<void> => {
  await axios.post(`${API_BASE_URL}/ProjectMembers/${projectId}/add`, null, {
    params: { userId }
  });
};

export const getProjectMembers = async (projectId: string): Promise<AppUserDto[]> => {
  const response = await axios.get(`${API_BASE_URL}/ProjectMembers/${projectId}/members`);
  return response.data;
};

export const removeUserFromProject = async (projectId: string, userId: string): Promise<void> => {
  await axios.delete(`${API_BASE_URL}/ProjectMembers/${projectId}/members/${userId}`);
};
