import React, { useState, useEffect } from 'react';
import Modal from '../../Components/Modal/Modal';

interface ProjectModalProps {
  isOpen: boolean;
  onClose: () => void;
  onSave: (project: { name: string; description: string }) => void;
  initialProject?: { name: string; description: string };
  title: string;
}

const ProjectModal: React.FC<ProjectModalProps> = ({
  isOpen,
  onClose,
  onSave,
  initialProject,
  title,
}) => {
  const [projectName, setProjectName] = useState('');
  const [projectDescription, setProjectDescription] = useState('');
  const [errors, setErrors] = useState<{ name?: string; description?: string }>({});

  useEffect(() => {
    if (initialProject) {
      setProjectName(initialProject.name);
      setProjectDescription(initialProject.description || '');
    } else {
      setProjectName('');
      setProjectDescription('');
    }
  }, [isOpen, initialProject]);

  const validate = () => {
    const errors: { name?: string; description?: string } = {};

    if (projectName.length < 3 || projectName.length > 36) {
      errors.name = 'Name must be between 3 and 36 characters.';
    }

    if (projectDescription.length > 500) {
      errors.description = 'Description must be 500 characters or less.';
    }

    setErrors(errors);

    return Object.keys(errors).length === 0;
  };

  const handleSave = () => {
    if (validate()) {
      onSave({ name: projectName, description: projectDescription });
      onClose();
    }
  };

  return (
    <Modal title={title} isOpen={isOpen} onClose={onClose}>
      <input
        type="text"
        placeholder="Project name"
        value={projectName}
        onChange={(e) => setProjectName(e.target.value)}
        className={`border p-2 w-full mb-4 rounded ${errors.name ? 'border-red-500' : ''}`}
      />
      {errors.name && <p className="text-red-500">{errors.name}</p>}

      <textarea
        placeholder="Project description"
        value={projectDescription}
        onChange={(e) => setProjectDescription(e.target.value)}
        className={`border p-2 w-full mb-4 rounded ${errors.description ? 'border-red-500' : ''}`}
      />
      {errors.description && <p className="text-red-500">{errors.description}</p>}

      <div className="flex justify-end">
        <button
          onClick={handleSave}
          className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600 mr-2"
        >
          Save
        </button>
      </div>
    </Modal>
  );
};

export default ProjectModal;
