// Theme Toggle Logic
document.addEventListener("DOMContentLoaded", () => {
    const themeToggleBtn = document.getElementById("theme-toggle");
    const htmlEl = document.documentElement;
    
    // Check local storage or system preference
    const savedTheme = localStorage.getItem("theme");
    const systemDark = window.matchMedia("(prefers-color-scheme: dark)").matches;

    // We default to dark theme for this gym aesthetic if no preference
    const initialTheme = savedTheme || (systemDark ? "dark" : "dark"); 
    
    setTheme(initialTheme);

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener("click", () => {
            const currentTheme = htmlEl.getAttribute("data-theme");
            const newTheme = currentTheme === "dark" ? "light" : "dark";
            setTheme(newTheme);
        });
    }

    function setTheme(theme) {
        htmlEl.setAttribute("data-theme", theme);
        localStorage.setItem("theme", theme);
        
        if (themeToggleBtn) {
            const icon = themeToggleBtn.querySelector("i");
            if (icon) {
                if (theme === "dark") {
                    icon.classList.remove("bi-moon-stars-fill");
                    icon.classList.add("bi-sun-fill");
                } else {
                    icon.classList.remove("bi-sun-fill");
                    icon.classList.add("bi-moon-stars-fill");
                }
            }
        }
    }
});
