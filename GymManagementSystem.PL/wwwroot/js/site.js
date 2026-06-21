(function() {
    var btn = document.getElementById('theme-toggle');
    var html = document.documentElement;
    if (!btn) return;
    btn.addEventListener('click', function() {
        var cur = html.getAttribute('data-theme');
        var next = cur === 'dark' ? 'light' : 'dark';
        html.setAttribute('data-theme', next);
        localStorage.setItem('gympro-theme', next);
        var icon = btn.querySelector('i');
        if (icon) icon.className = next === 'dark' ? 'bi bi-moon-stars-fill' : 'bi bi-sun-fill';
    });
})();
