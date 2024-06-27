import * as Yup from "yup";
import { yupResolver } from "@hookform/resolvers/yup";
import { useAuth } from '../../Context/useAuth';
import { useForm } from 'react-hook-form';

type Props = {};

type LoginFormsInputs = {
  email: string;
  password: string;
};

const validation = Yup.object().shape({
  email: Yup.string().required("Email is required").email("Invalid email address"),
  password: Yup.string().required("Password is required"),
});

const LoginPage = (props: Props) => {
  const { loginUser } = useAuth();
  const { register, handleSubmit, formState: { errors } } = useForm<LoginFormsInputs>({ resolver: yupResolver(validation) });

  const handleLogin = (form: LoginFormsInputs) => {
    loginUser(form.email, form.password);
  };

  return (
    <section className="flex items-center justify-center w-full h-full">
      <div className="bg-white p-8 rounded-lg shadow-lg max-w-md w-full">
        <h1 className="text-2xl font-bold mb-6 text-center">Sign In</h1>
        <form onSubmit={handleSubmit(handleLogin)} className="">
          <div className="mt-4">
            <label htmlFor="email" className="block text-sm font-medium text-gray-700">Email</label>
            <input
              type="text"
              id="email"
              placeholder="Email"
              {...register("email")}
              className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
            {errors.email && <p className="mt-2 text-sm text-red-600">{errors.email.message}</p>}
          </div>
          <div className="mt-4 ">
            <label htmlFor="password" className="block text-sm font-medium text-gray-700">Password</label>
            <input
              type="password"
              id="password"
              placeholder="••••••••"
              {...register("password")}
              className="mt-1 block w-full p-2 border border-gray-300 rounded-md shadow-sm focus:ring-indigo-500 focus:border-indigo-500 sm:text-sm"
            />
            {errors.password && <p className="mt-2 text-sm text-red-600">{errors.password.message}</p>}
          </div>
          <button 
            type="submit" 
            className="w-full flex justify-center mt-10 py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-beige hover:bg-beige-dark focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-beige-light ">
              Sign in
          </button>
          <p className="mt-2 text-center text-sm text-gray-600">
            Don’t have an account yet? <a href="#" className="text-indigo-600 hover:text-indigo-500">Sign up</a>
          </p>
        </form>
      </div>
    </section>
  );
}

export default LoginPage;
