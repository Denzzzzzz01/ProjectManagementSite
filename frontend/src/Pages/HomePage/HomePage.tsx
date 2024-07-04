import React from "react";
import Projects from "../../Components/Projects/Projects";

type Props = {};

const HomePage = (props: Props) => {
  return (
    <>
      <h1 className="text-3xl font-medium
      hover:text-green-500 transition-color duration-300 ease-in-out">
      TEST HOME PAGE
      <Projects />
      </h1>
    </>
  );
};

export default HomePage;