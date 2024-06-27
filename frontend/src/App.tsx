import './App.css';
import { ToastContainer } from 'react-toastify';
import { UserProvider } from './Context/useAuth';
import { Outlet } from 'react-router';
import Navbar from './Components/Navbar/Navbar';

function App() {
  return (
    <div className="min-h-screen flex flex-col bg-custom-radial-gradient">
      <UserProvider>
        <Navbar />
        <div className="flex-grow flex justify-center items-center">
          <Outlet />
        </div>
        <ToastContainer />
      </UserProvider>
    </div>
  );
}

export default App;
