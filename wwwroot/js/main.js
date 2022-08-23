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
        let parent = element.parentElement;

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
function getElementScrollSize(element) {
    if (element) {
        let parent = element.parentElement;

        return {
            width: parent.scrollLeft,
            height: parent.scrollTop
        };
    }

    return {
        width: 0,
        height: 0
    };
}
//
function findElementIndexById(list, objRef) {
    var objRefIndex = -1;

    list.forEach((item, index) => {
        if (item._id == objRef._id)
            objRefIndex = index;
    });

    return objRefIndex;
}
//
var globalResizeEventListener;
var globalResizeEventListenerReferences = [];
function addWindowResizeEventListener(objRef) {
    if (!globalResizeEventListener) {
        globalResizeEventListener = window.addEventListener('resize', windowResizeHandler);
    }

    globalResizeEventListenerReferences.push(objRef);
}
function removeWindowResizeEventListener(objRef) {
    //not working var index = globalResizeEventListenerReferences.indexOf(objRef);
    var index = findElementIndexById(globalResizeEventListenerReferences, objRef);
    if (index > -1) {
        globalResizeEventListenerReferences.splice(index, 1);
    }
}
function windowResizeHandler(event) {
    globalResizeEventListenerReferences.forEach((item) => {
        item.invokeMethodAsync('OnWindowResizeEvent', { width: window.innerWidth, height: window.innerHeight });
    });
}
//
var globalMouseDownEventListener;
var globalMouseDownEventListenerReferences = [];
function addWindowMouseDownEventListener(objRef) {
    if (!globalMouseDownEventListener) {
        globalMouseDownEventListener = window.addEventListener('mousedown', windowMouseDownHandler);
    }

    globalMouseDownEventListenerReferences.push(objRef);
}
function removeWindowMouseDownEventListener(objRef) {
    //not working var index = globalMouseDownEventListenerReferences.indexOf(objRef);
    var index = findElementIndexById(globalMouseDownEventListenerReferences, objRef);
    if (index > -1) {
        globalMouseDownEventListenerReferences.splice(index, 1);
    }
}
function windowMouseDownHandler(event) {
    globalMouseDownEventListenerReferences.forEach((item) => {
        item.invokeMethodAsync('OnWindowMouseDownEvent', { clientX: event.clientX, clientY: event.clientY });
    });
}
//
function writeLine(message) {
    window.console.log(message);
}

var expansionPanelOperations = [];

function expansionPanelToggle(element) {
    window.console.log(element);
    if (element) {
        var expander = event.target.closest('.giz-expansion-panel__header').parentNode;

        if (expansionPanelOperations[expander])
            return;

        if (!expander.classList.contains('expanded')) {
            expander.classList.add('expanded');
            var body = expander.querySelector('.giz-expansion-panel__body');
            var height = body.getBoundingClientRect().height;

            body.style.setProperty('--abh', height + 'px');
            expander.classList.add('expanding');

            var expansionPanelTimeout = setTimeout(function () {
                expander.classList.remove('expanding');
                expansionPanelOperations[expander] = null;
            }, 500, expander);

            expansionPanelOperations[expander] = expansionPanelTimeout;
        }
        else {
            var body = expander.querySelector('.giz-expansion-panel__body');
            var height = body.getBoundingClientRect().height;

            body.style.setProperty('--abh', height + 'px');
            expander.classList.add('collapsing');

            var expansionPanelTimeout = setTimeout(function () {
                expander.classList.remove('expanded');
                expander.classList.remove('collapsing');
                expansionPanelOperations[expander] = null;
            }, 500, expander);
        }
    }
}