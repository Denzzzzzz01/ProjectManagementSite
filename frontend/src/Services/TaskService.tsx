import axios from 'axios';
import { API_BASE_URL } from '../config';
import { AddTaskDto } from '../Models/AddTaskDto';
import { UpdateTaskDto } from '../Models/UpdateTaskDto';

export const addTask = async (task: AddTaskDto) => {
  const response = await axios.post(`${API_BASE_URL}/Task/AddTask`, task);
  return response.data;
};

export const removeTask = async (taskId: string, projectId: string) => {
    const response = await axios.delete(`${API_BASE_URL}/Task/RemoveTask`, { data: { projectId, taskId } });
    return response.data;
};

export const markTaskDone = async (taskId: string, projectId: string, isDone: boolean) => {
    const response = await axios.put(`${API_BASE_URL}/Task/DoTask`, {
        taskId,
        projectId,
        isDone
    });
    return response.data;
};

export const updateTask = async (taskId: string, projectId: string, updatedTask: UpdateTaskDto) => {
    const response = await axios.put(`${API_BASE_URL}/Task/UpdateTask`, {
      taskId,
      projectId,
      ...updatedTask,
    });
    return response.data;
};