//
function getWindowSize() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
}
//
function getElementBoundingClientRect(element) {
    return element.getBoundingClientRect();
}
//
function scrollListItemIntoView(element) {
    if (element) {
        let parent = element.parentElement

        let { top: eTop } = element.getBoundingClientRect();
        let { top: pTop } = parent.getBoundingClientRect();

        parent.parentElement.parentElement.scrollTop = eTop - pTop;
    }
}
//
function scrollDatePickerYear() {
    var items = document.getElementsByClassName("giz-calendar-year-count active");

    for (var i = 0; i < items.length; i++) {
        items[i].scrollIntoView({ block: "center" });
    }
}
//
function focusElement(element) {
    element.focus();
}
//
function setPropByElement(element, property, value) {
    element[property] = value;
}
//
function scrollItemToTop(element) {
    if (element) {
        let parent = element.parentElement

        let { top: eTop } = element.getBoundingClientRect();
        let { top: pTop } = parent.getBoundingClientRect();

        parent.scrollTop = eTop - pTop;
    }
}
//
function writeLine(message) {
    window.console.log(message);
}