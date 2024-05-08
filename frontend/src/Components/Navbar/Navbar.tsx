import { Link } from 'react-router-dom';
import { useAuth } from '../../Context/useAuth';

type Props = {}

const Navbar = (props: Props) => {
    const { isLoggedIn, user, logout } = useAuth();

    return (
      <nav >
        <div >
          <div >
            <Link to="/"><h1>ProjectManagementSite</h1></Link>
          </div>
          {isLoggedIn() ? (
            <div>
              <div>Welcome, {user?.username}</div>
              <a
                onClick={logout}
              >
                Logout
              </a>
            </div>
          ) : (
            <div>
              <Link to="/login">
                Login
              </Link>
              <Link
                to="/register"
              >
                Signup
              </Link>
            </div>
          )}
        </div>
      </nav>
    );
}

export default Navbar