/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    './Views/**/*.cshtml',
    './wwwroot/js/**/*.js',
  ],
  darkMode: 'class', // Allows explicit dark mode control if needed, though we default to dark
  theme: {
    extend: {
      colors: {
        gym: {
          dark: '#09090b', // zinc-950
          darker: '#020617', // slate-950
          card: '#18181b', // zinc-900
          border: '#27272a', // zinc-800
        },
        neon: {
          green: '#39ff14',
          cyan: '#0ff',
          orange: '#ff5e00',
        }
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
        display: ['Outfit', 'sans-serif'],
      },
      boxShadow: {
        'neon-green': '0 0 10px rgba(57, 255, 20, 0.4), 0 0 20px rgba(57, 255, 20, 0.2)',
        'neon-cyan': '0 0 10px rgba(0, 255, 255, 0.4), 0 0 20px rgba(0, 255, 255, 0.2)',
        'neon-orange': '0 0 10px rgba(255, 94, 0, 0.4), 0 0 20px rgba(255, 94, 0, 0.2)',
        'glass': '0 8px 32px 0 rgba(0, 0, 0, 0.37)',
      }
    },
  },
  plugins: [],
}
