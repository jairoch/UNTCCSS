function toggleCollapse(id) {
    var element = document.getElementById("collapse-" + id);
    if (element.classList.contains("show")) {
        element.classList.remove("show");
    } else {
        element.classList.add("show");
    }
}