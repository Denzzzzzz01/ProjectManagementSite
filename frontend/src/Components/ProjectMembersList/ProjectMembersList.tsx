import React, { useEffect, useState } from 'react';
import { getProjectMembers, removeUserFromProject, addUserToProject } from '../../Services/ProjectMembersService';
import { AppUserDto } from '../../Models/AppUserDto';
import ConfirmationModal from '../../Components/ConfirmationModal/ConfirmationModal';
import UserSearchModal from '../../Components/UserSearchModal/UserSearchModal';
import { toastPromise } from '../../Utils/toastUtils';

interface ProjectMembersListProps {
  projectId: string;
}

const ProjectMembersList: React.FC<ProjectMembersListProps> = ({ projectId }) => {
  const [members, setMembers] = useState<AppUserDto[]>([]);
  const [filteredMembers, setFilteredMembers] = useState<AppUserDto[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [selectedUser, setSelectedUser] = useState<AppUserDto | null>(null);
  const [isConfirmationOpen, setIsConfirmationOpen] = useState(false);
  const [isUserSearchModalOpen, setIsUserSearchModalOpen] = useState(false);

  useEffect(() => {
    const fetchMembers = async () => {
      if (projectId) {
        try {
          const members = await getProjectMembers(projectId);
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

  const handleAddUser = async (userId: string) => {
    try {
      await toastPromise(
        addUserToProject(projectId, userId),
        'Adding user to project',
        'User added to project successfully!',
        'Failed to add user to project'
      );
      const updatedMembers = await getProjectMembers(projectId);
      const sortedMembers = updatedMembers.sort((a, b) => a.userName.localeCompare(b.userName));
      setMembers(sortedMembers);
      setFilteredMembers(sortedMembers);
    } catch (error) {
      console.error('Error adding user to project:', error);
    }
  };

  const closeUserSearchModal = () => {
    setIsUserSearchModalOpen(false);
  };

  return (
    <div className="mt-8 p-4 rounded shadow-md shadow-black bg-white">
      <h3 className="text-2xl font-bold mb-4 flex justify-center items-center text-beige">
        <button
          onClick={() => setIsUserSearchModalOpen(true)}
          className="bg-beige mr-1 shadow-sm shadow-black rounded transform transition-transform duration-200 
          hover:bg-beige-dark active:shadow-inner active:shadow-black "
        >
          <img src="https://www.flaticon.com/svg/vstatic/svg/3917/3917555.svg?token=exp=1723965753~hmac=d0d53a851ae0fe8316c71ec5008473e0" draggable="false" width="32" height="32" className='p-1.5 active:pt-2 active:pb-1 filter invert'/>
        </button>
        Project Members
      </h3>

      <div className="relative w-full">
        <i className="fi fi-br-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
        <input
          type="text"
          value={searchTerm}
          onChange={handleSearchChange}
          placeholder="Search members..."
          className="w-full pl-10 px-4 py-2 rounded shadow-inner"
        />
      </div>

      {filteredMembers.length === 0 && (
            <p className="text-gray-500 mt-4">No members found.</p>
          )}
      <ul className="mt-4 max-h-[16vh] overflow-y-auto">
        {filteredMembers.map((member) => (
          <li key={member.id} className="flex justify-between items-center mb-2 mx-1 p-1 rounded-sm shadow-sm shadow-black bg-beige hover:bg-beige-dark">
            <span>{member.userName}</span>
            <button
              onClick={() => handleRemoveMember(member)}
              className="bg-red-500 text-white px-2 pt-1.5 pb-0.5 rounded hover:bg-red-600 text-center "
            >
              <i className="fi fi-bs-delete-user hover:scale-125"></i>
            </button>
          </li>
        ))}
      </ul>

      <UserSearchModal
        isOpen={isUserSearchModalOpen}
        onClose={closeUserSearchModal}
        projectId={projectId}
        onAddUser={handleAddUser}
      />

      <ConfirmationModal
        isOpen={isConfirmationOpen}
        onClose={cancelRemoveMember}
        onConfirm={confirmRemoveMember}
        title="Confirm Member Removal"
        message={`Are you sure you want to remove ${selectedUser?.userName} from the project?`}
      />
    </div>
  );
};

export default ProjectMembersList;
