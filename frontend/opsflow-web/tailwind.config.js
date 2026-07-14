/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{ts,tsx}'],
  theme: {
    extend: {
      boxShadow: {
        glow: '0 20px 60px rgba(15, 23, 42, 0.24)',
      },
      colors: {
        canvas: {
          50: '#f8fbff',
          100: '#eef5ff',
          200: '#d9e8ff',
          300: '#b8d1ff',
          400: '#87b2ff',
          500: '#4f8cff',
          600: '#2d64e8',
          700: '#224bc2',
          800: '#1e3d97',
          900: '#1a3378',
        },
      },
      backgroundImage: {
        'opsflow-grid':
          'linear-gradient(rgba(148, 163, 184, 0.18) 1px, transparent 1px), linear-gradient(90deg, rgba(148, 163, 184, 0.18) 1px, transparent 1px)',
      },
    },
  },
  plugins: [],
};
