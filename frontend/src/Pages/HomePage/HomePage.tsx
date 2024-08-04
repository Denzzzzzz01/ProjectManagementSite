import React from "react";
import Projects from "../../Components/Projects/Projects";

type Props = {};

const HomePage = (props: Props) => {
  return (
    <div className="flex flex-col justify-center items-center">
      <Projects />
    </div>
  );
};

export default HomePage;