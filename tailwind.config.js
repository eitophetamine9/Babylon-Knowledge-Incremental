/** @type {import('tailwindcss').Config} */
module.exports = {
  darkMode: ['selector', '[data-theme="dark"]'],
  content: [
    "./**/*.{razor,html,cshtml}",
    "./Components/**/*.{razor,html}",
    "./Pages/**/*.{razor,html}",
    "./wwwroot/**/*.html"
  ],
  theme: {
    extend: {
      colors: {
        gold: {
          DEFAULT: '#D4AF37',
          dark: '#B8960C',
          light: 'rgba(212, 175, 55, 0.15)',
          glow: 'rgba(212, 175, 55, 0.4)',
        },
        lapis: {
          DEFAULT: '#1B4965',
          dark: '#0F3A5D',
          light: 'rgba(27, 73, 101, 0.08)',
        },
        sandstone: {
          DEFAULT: '#F4EEDD',
          alt: '#EBE3CD',
          deep: '#E2D8C0',
        },
        marble: {
          DEFAULT: '#F9F8F6',
          hover: '#FFFFFF',
        }
      },
      fontFamily: {
        heading: ['Cinzel', 'serif'],
        body: ['Inter', 'sans-serif'],
      },
      boxShadow: {
        'gold': '0 0 15px rgba(212, 175, 55, 0.35)',
        'card': '0 4px 16px rgba(44, 34, 15, 0.06)',
      }
    },
  },
  plugins: [],
}
