/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./**/*.{razor,html,cshtml}",
    "./Pages/**/*.{razor,html}",
    "./Layout/**/*.{razor,html}",
    "./wwwroot/**/*.html",
    "./Components/**/*.{razor,html}"
  ],
  theme: {
    extend: {},
  },
  plugins: [
    require('@tailwindcss/forms'),
    require('@tailwindcss/typography'),
  ],
}
