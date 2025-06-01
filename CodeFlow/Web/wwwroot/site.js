window.toggleMenu = function () {
    const navMenu = document.getElementById('navMenu');
    const btn = document.getElementById('mobileMenuBtn');

    if (!navMenu || !btn) return;

    navMenu.classList.toggle('active');
    btn.innerHTML = navMenu.classList.contains('active')
        ? '<i class="fas fa-times"></i>'
        : '<i class="fas fa-bars"></i>';
}
