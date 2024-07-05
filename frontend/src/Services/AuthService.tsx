import axios from "axios";
import { handleError } from "../Helpers/ErrorHandler";
import { UserProfileToken } from "../Models/User";
import { API_BASE_URL } from "../config";

export const loginAPI = async (email: string, password: string) => {
  try {
    const data = await axios.post<UserProfileToken>(API_BASE_URL + "/account/login", {
      email: email,
      password: password,
    });
    return data; 
  } catch (error) {
    handleError(error);
  }
};

export const registerAPI = async ( username: string, email: string, password: string) => {
  try {
    const data = await axios.post<UserProfileToken>(API_BASE_URL + "/account/register", {
      username: username,
      email: email,
      password: password,
    });
    return data;
  } catch (error) {
    handleError(error);
  }
};