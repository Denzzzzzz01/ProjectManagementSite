import * as Yup from "yup"
import { yupResolver } from "@hookform/resolvers/yup"
import { useAuth } from '../../Context/useAuth';
import { useForm } from 'react-hook-form';

type Props = {};

type LoginFormsInputs = {
  username: string;
  password: string;
};

const validation = Yup.object().shape({
  username: Yup.string().required("Username is required"),
  password: Yup.string().required("Password is required"),
});

const LoginPage = (props: Props) => {
  const { loginUser } = useAuth();
  const { register, handleSubmit, formState: {errors} } = useForm<LoginFormsInputs>({resolver: yupResolver(validation)});
  
  const handleLogin = (form: LoginFormsInputs) => {
    loginUser(form.username, form.password);
  };

  return (
    <section >
      <div >
        <h1> Sign in to your account </h1>
        <form onSubmit={handleSubmit(handleLogin)}>
          <div>
            <label htmlFor="username" > Username </label>
            <input
              type="text"
              id="username"
              placeholder="username"
              {...register("username")}
            />
            {errors.username ? (
              <p>{errors.username.message}</p>
            ) : (
              ""
            )}
          </div>
          <div>
            <label htmlFor="password" > Password </label>
            <input
              type="password"
              id="password"
              placeholder="••••••••"
              {...register("password")}
            />
            {errors.password ? (
              <p>{errors.password.message}</p>
            ) : (
              ""
            )} 
          </div>
          <div>
            <a href="#" > Forgot password? </a>
          </div>
          <button type="submit"> Sign in </button>
          <p>
            Don’t have an account yet?{" "}
            <a href="#" > Sign up </a>
          </p>
        </form>
      </div>
    </section>
  );
}

export default LoginPage