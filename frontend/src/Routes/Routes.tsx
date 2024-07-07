import { createBrowserRouter } from "react-router-dom";
import App from "../App";
import HomePage from "../Pages/HomePage/HomePage";
import LoginPage from "../Pages/LoginPage/LoginPage";
import RegisterPage from "../Pages/RegisterPage/RegisterPage";
import ProtectedRoute from "./ProtectedRoute";
import ProjectDetailPage from "../Pages/ProjectDetailPage/ProjectDetailPage";

export const router = createBrowserRouter([
    {
      path: "/",
      element: <App />,
      children: [
        { path: "", element: <ProtectedRoute><HomePage /></ProtectedRoute> },
        { path: "project/:projectId", element: <ProtectedRoute><ProjectDetailPage /></ProtectedRoute> },
        { path: "login", element: <LoginPage /> },
        { path: "register", element: <RegisterPage /> },
      ],
    },
  ]);