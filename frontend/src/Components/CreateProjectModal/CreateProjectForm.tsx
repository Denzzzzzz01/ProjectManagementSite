import React from 'react';
import { useForm } from 'react-hook-form';
import axios from 'axios';
import { API_BASE_URL } from '../../config';

type FormData = {
  name: string;
};

const CreateProjectForm: React.FC = () => {
  const { register, handleSubmit, reset } = useForm<FormData>();

  const onSubmit = async (data: FormData) => {
    try {
      const response = await axios.post(`${API_BASE_URL}/Project/CreateProject`, { name: data.name });
      console.log('Project created:', response.data);
      reset();
    } catch (error) {
      console.error("Error creating project", error);
    }
  };

  return (
    <div className="p-8 max-w-md mx-auto bg-white rounded-lg shadow-md">
      <h2 className="text-2xl mb-4">Create New Project</h2>
      <form onSubmit={handleSubmit(onSubmit)}>
        <div className="mb-4">
          <label htmlFor="name" className="block text-sm font-medium text-gray-700">Project Name</label>
          <input
            id="name"
            type="text"
            {...register('name', { required: true })}
            className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm"
          />
        </div>
        <div className="flex justify-end">
          <button type="submit" className="px-4 py-2 bg-blue-500 text-white rounded">Create</button>
        </div>
      </form>
    </div>
  );
};

export default CreateProjectForm;
