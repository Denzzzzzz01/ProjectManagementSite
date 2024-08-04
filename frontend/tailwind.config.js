/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  plugins: [
    require('tailwind-scrollbar'),
    require('@tailwindcss/forms'),
    function({ addUtilities }) {
      addUtilities({
        '.scrollbar-hide': {
          '-ms-overflow-style': 'none',  // IE и Edge
          'scrollbar-width': 'none',  // Firefox
          '&::-webkit-scrollbar': {
            display: 'none',  // Chrome, Safari, Opera
          },
        },
      });
    },
  ],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Nunito', 'sans-serif'],
      },
      backgroundImage: {
        'gray-gradient': 'linear-gradient(90deg, #2d3130 0%, #424848 60%, #515756 100%)' ,
        'gray-radial-gradient': 'radial-gradient(circle at 70% 30%, #515756 0%, #424848 30%, #2d3130 85%)',
        'beige-gradient': 'linear-gradient(90deg, #dab27b 0%, #d0af8b 60%, #b49270 100%)' ,
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