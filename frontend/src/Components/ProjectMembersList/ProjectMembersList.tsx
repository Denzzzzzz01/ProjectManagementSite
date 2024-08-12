import React, { useEffect, useState } from 'react';
import { getProjectMembers, removeUserFromProject } from '../../Services/ProjectMembersService';
import { AppUserDto } from '../../Models/AppUserDto';
import ConfirmationModal from '../../Components/ConfirmationModal/ConfirmationModal';
import { toastPromise } from '../../Utils/toastUtils'; // импортируйте toastPromise

interface ProjectMembersListProps {
  projectId: string;
}

const ProjectMembersList: React.FC<ProjectMembersListProps> = ({ projectId }) => {
  const [members, setMembers] = useState<AppUserDto[]>([]);
  const [filteredMembers, setFilteredMembers] = useState<AppUserDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUser, setSelectedUser] = useState<AppUserDto | null>(null);
  const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);

  useEffect(() => {
    const fetchMembers = async () => {
      if (projectId) {
        try {
          const members = await getProjectMembers(projectId);
          // Сортировка пользователей по именам
          const sortedMembers = members.sort((a, b) => a.userName.localeCompare(b.userName));
          setMembers(sortedMembers);
          setFilteredMembers(sortedMembers);
        } catch (error) {
          console.error('Error fetching project members:', error);
        }
      }
    };

    fetchMembers();
  }, [projectId]);

  useEffect(() => {
    // Фильтрация пользователей по поисковому запросу
    const searchFilter = searchTerm.toLowerCase();
    const filtered = members.filter(member =>
      member.userName.toLowerCase().includes(searchFilter)
    );
    setFilteredMembers(filtered);
  }, [searchTerm, members]);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const handleRemoveMember = (user: AppUserDto) => {
    setSelectedUser(user);
    setIsConfirmationOpen(true);
  };

  const confirmRemoveMember = async () => {
    if (selectedUser && projectId) {
      try {
        await toastPromise(
          removeUserFromProject(projectId, selectedUser.id),
          'Removing user from project',
          'User removed from project successfully!',
          'Failed to remove user from project'
        );
        setMembers((prevMembers) => prevMembers.filter((member) => member.id !== selectedUser.id));
        setFilteredMembers((prevMembers) => prevMembers.filter((member) => member.id !== selectedUser.id));
        setSelectedUser(null);
        setIsConfirmationOpen(false);
      } catch (error) {
        console.error('Error removing member from project:', error);
      }
    }
  };

  const cancelRemoveMember = () => {
    setSelectedUser(null);
    setIsConfirmationOpen(false);
  };

  return (
    <div className="mt-8">
      <h3 className="text-2xl font-bold mb-4">Project Members</h3>
      <input
        type="text"
        placeholder="Search members"
        value={searchTerm}
        onChange={handleSearchChange}
        className="border p-2 w-full mb-4 rounded"
      />
      {filteredMembers.length === 0 ? (
        <p>No members found.</p>
      ) : (
        <ul>
          {filteredMembers.map((member) => (
            <li key={member.id} className="flex justify-between items-center mb-2 p-2 bg-gray-800 rounded">
              <span>{member.userName}</span>
              <button
                onClick={() => handleRemoveMember(member)}
                className="bg-red-500 text-white px-2 py-1 rounded hover:bg-red-700 transition"
              >
                Remove
              </button>
            </li>
          ))}
        </ul>
      )}
      {selectedUser && (
        <ConfirmationModal
          isOpen={isConfirmationOpen}
          onClose={cancelRemoveMember}
          onConfirm={confirmRemoveMember}
          title="Confirm Removal"
          message={`Are you sure you want to remove ${selectedUser.userName} from the project?`}
        />
      )}
    </div>
  );
};

export default ProjectMembersList;
