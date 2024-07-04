/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      backgroundImage: {
        'gray-gradient': 'linear-gradient(90deg, #2d3130 0%, #424848 60%, #515756 100%)' ,
        'gray-radial-gradient': 'radial-gradient(circle at 70% 30%, #515756 0%, #424848 30%, #2d3130 85%)',
      },
      colors: {
        darkGray: {
          light: '#515756',  
          DEFAULT: '#424848',  
          dark: '#2d3130',  
        },
        beige: {
          light: '#dab27b',
          DEFAULT: '#d0af8b',  
          dark: '#b49270',
        }
      },
    },
  },
  plugins: [],
}