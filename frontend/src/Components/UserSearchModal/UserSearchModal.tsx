import React, { useState } from 'react';
import Modal from '../../Components/Modal/Modal';
import { searchUsers } from '../../Services/ProjectMembersService';
import { AppUserDto } from '../../Models/AppUserDto';

interface UserSearchModalProps {
  isOpen: boolean;
  onClose: () => void;
  projectId: string;
  onAddUser: (userId: string) => Promise<void>;
}

const UserSearchModal: React.FC<UserSearchModalProps> = ({ isOpen, onClose, projectId, onAddUser }) => {
  const [searchTerm, setSearchTerm] = useState('');
  const [users, setUsers] = useState<AppUserDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);
  const [searchClicked, setSearchClicked] = useState(false); // Новое состояние

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
    setSearchClicked(true); // Устанавливаем состояние в true при нажатии на кнопку поиска

    try {
      const result = await searchUsers(searchTerm);
      setUsers(result);
    } catch (error) {
      console.error('Error fetching users:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleAddUserClick = async (userId: string) => {
    await onAddUser(userId);
    setUsers((prevUsers) => prevUsers.filter((user) => user.id !== userId));
  };

  return (
    <Modal title="Search Users" isOpen={isOpen} onClose={onClose} className='max-h-[50%]'>
      
      <div className="relative w-full">
        <i className="fi fi-br-search absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400"></i>
        <input
          type="text"
          value={searchTerm}
          onChange={handleSearchChange}
          placeholder="Search users..."
          className="w-full pl-10 px-4 py-2 rounded shadow-inner shadow-gray-400"
        />
      </div>
      <button
        onClick={handleSearchClick}
        className="bg-beige shadow-sm shadow-black text-white px-4 py-2 rounded mt-2
        hover:bg-beige-dark active:pt-3 active:pb-1 active:shadow-inner active:shadow-black"
      >
        Search
      </button>
      {errorMessage && <p className="text-red-500 mt-2">{errorMessage}</p>}
      {loading ? (
        <p className="mt-4">Loading...</p>
      ) : (
        <>
          {users.length === 0 && searchClicked && !errorMessage && (
            <p className="text-gray-500 mt-4">No users found.</p>
          )}
          <ul className="list-none pb-2 px-1 mt-4 max-h-[24vh] overflow-auto">
            {users.map((user) => (
              <li key={user.id} className="flex items-center justify-between mt-2 px-2 py-1 bg-gray-300 rounded shadow-sm shadow-black">
                <span>{user.userName}</span>
                <button
                  onClick={() => handleAddUserClick(user.id)}
                  className="bg-beige shadow-sm shadow-black text-white px-2 pt-1.5 pb-0.5 rounded
                  hover:bg-beige-dark active:pt-2 active:pb-0 active:shadow-inner active:shadow-black"
                >
                  <i className="fi fi-br-plus-small text-2xl"></i>
                </button>
              </li>
            ))}
          </ul>
        </>
      )}
    </Modal>
  );
};

export default UserSearchModal;
