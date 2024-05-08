import * as Yup from "yup"
import { yupResolver } from "@hookform/resolvers/yup"
import { useAuth } from "../../Context/useAuth";
import { useForm } from "react-hook-form";

type Props = {}

type RegisterFormsInputs = {
    username: string;  
    email: string;
    password: string;
};

const validation = Yup.object().shape({
    username: Yup.string().required("Username is required"),
    email: Yup.string().required("Email is required"),
    password: Yup.string().required("Password is required"),
});

const RegisterPage = (props: Props) => {
    const { registerUser } = useAuth();
    const { register, handleSubmit, formState: {errors} } = useForm<RegisterFormsInputs>({resolver: yupResolver(validation)});

    const handleRegister = (form: RegisterFormsInputs) => {
        registerUser(form.username, form.email, form.password);
    };

  return (
    <section >
      <div >
        <h1> Sign up to your account </h1>
        <form onSubmit={handleSubmit(handleRegister)}>
        <div>
            <label htmlFor="username" > Username </label>
            <input
              type="text"
              id="username"
              placeholder="Username"
              {...register("username")}
            />
            {errors.username ? (
              <p>{errors.username.message}</p>
            ) : (
              ""
            )}
          </div>
          
          <div>
            <label htmlFor="email" > Email </label>
            <input
              type="text"
              id="email"
              placeholder="Email"
              {...register("email")}
            />
            {errors.email ? (
              <p>{errors.email.message}</p>
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
  )
}

export default RegisterPage