window.removeSidebarActiveOnResize = function () {
    window.addEventListener("resize", function () {
        if (window.innerWidth <= 991) {
            document.getElementById("sidebar").classList.remove("active");
            document.getElementById("content").classList.remove("active");
        }
    });
};


function toggleSidebar() {
    $('#sidebar').toggleClass('active');
    $('#content').toggleClass('active');
}

function toggleMoreMenu() {
    $('#sidebar, .body-overlay').toggleClass('show-nav');
}



