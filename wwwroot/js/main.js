function getWindowSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}

function getElementBoundingClientRect(element) {
    return element.getBoundingClientRect();
}
