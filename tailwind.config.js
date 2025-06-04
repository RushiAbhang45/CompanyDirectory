/** @type {import('tailwindcss').Config} */
module.exports = {
    content: ["./**/*.cshtml"], // Search all .cshtml files in all subdirectories
    theme: {
        extend: {},
    },
    plugins: [
        require('@tailwindcss/line-clamp'),
    ],
}

