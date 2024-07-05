import React, { useState } from 'react';
import { createProject } from '../../Services/ProjectService';
import { ProjectVm } from '../../Models/ProjectVm';

interface CreateProjectModalProps {
  setProjects: React.Dispatch<React.SetStateAction<ProjectVm[]>>;
}

const CreateProjectModal: React.FC<CreateProjectModalProps> = ({ setProjects }) => {
  const [showModal, setShowModal] = useState(false);
  const [projectName, setProjectName] = useState('');

  const handleCreateProject = async () => {
    try {
      const newProject = await createProject(projectName);
      setProjects(prevProjects => [...prevProjects, newProject]);
      setShowModal(false);
    } catch (error) {
      console.error('Error creating project:', error);
    }
  };

  return (
    <>
      <button
        onClick={() => setShowModal(true)}
        className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
      >
        Create Project
      </button>
      {showModal && (
        <div className="fixed inset-0 bg-gray-600 bg-opacity-50 flex justify-center items-center z-50">
          <div className="bg-white p-6 rounded shadow-lg max-w-sm w-full">
            <h2 className="text-xl mb-4">Create Project</h2>
            <input
              type="text"
              value={projectName}
              onChange={(e) => setProjectName(e.target.value)}
              placeholder="Project name"
              className="border p-2 w-full mb-4 rounded"
            />
            <div className="flex justify-end">
              <button
                onClick={handleCreateProject}
                className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 mr-2"
              >
                Create
              </button>
              <button
                onClick={() => setShowModal(false)}
                className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600"
              >
                Cancel
              </button>
            </div>
          </div>
        </div>
      )}
    </>
  );
};

export default CreateProjectModal;
