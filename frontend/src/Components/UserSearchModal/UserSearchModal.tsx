import React, { useState } from 'react';
import Modal from '../../Components/Modal/Modal';
import { searchUsers, addUserToProject } from '../../Services/ProjectMembersService';
import { AppUserDto } from '../../Models/AppUserDto';
import { toastPromise } from '../../Utils/toastUtils'; // импортируйте toastPromise

interface UserSearchModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
}

const UserSearchModal: React.FC<UserSearchModalProps> = ({ isOpen, onClose, projectId }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [users, setUsers] = useState<AppUserDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  const handleSearchChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(e.target.value);
  };

  const handleSearchClick = async () => {
    if (searchTerm.length < 3) {
      setErrorMessage('Please enter at least 3 characters.');
      return;
    }

    setLoading(true);
    setErrorMessage(null);

    try {
      const result = await searchUsers(searchTerm);
      setUsers(result);
    } catch (error) {
      console.error('Error fetching users:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddUser = async (userId: string) => {
    try {
      await toastPromise(
        addUserToProject(projectId, userId),
        'Adding user to project',
        'User added to project successfully!',
        'Failed to add user to project'
      );
      setUsers((prevUsers) => prevUsers.filter((user) => user.id !== userId));
    } catch (error) {
      console.error('Error adding user to project:', error);
    }
  };

  return (
    <Modal title="Search Users" isOpen={isOpen} onClose={onClose}>
      <input
        type="text"
        placeholder="Search users"
        value={searchTerm}
        onChange={handleSearchChange}
        className="border p-2 w-full mb-4 rounded"
      />
      <button
        onClick={handleSearchClick}
        className="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600"
      >
        Search
      </button>
      {errorMessage && <p className="text-red-500 mt-2">{errorMessage}</p>}
      {loading ? (
        <p className="mt-4">Loading...</p>
      ) : (
        <ul className="list-none p-0 mt-4">
          {users.map((user) => (
            <li key={user.id} className="flex items-center justify-between mb-2 p-2 bg-gray-200 rounded">
              <span>{user.userName}</span>
              <button
                onClick={() => handleAddUser(user.id)}
                className="bg-green-500 text-white px-4 py-2 rounded hover:bg-green-600"
              >
                Add
              </button>
            </li>
          ))}
        </ul>
      )}
    </Modal>
  );
};

export default UserSearchModal;
