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
    var items = document.getElementsByClassName("giz-date-picker-year-count active");

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
function findElementIndexById(list, elementRef) {
    var objRefIndex = -1;

    list.forEach((item, index) => {
        if (item._id == elementRef._id)
            objRefIndex = index;
    });

    return objRefIndex;
}
//
var globalResizeEventListener;
var globalResizeEventListenerReferences = [];
function addWindowResizeEventListener(elementRef) {
    if (!globalResizeEventListener) {
        globalResizeEventListener = window.addEventListener('resize', windowResizeHandler);
    }

    globalResizeEventListenerReferences.push(elementRef);
}
function removeWindowResizeEventListener(elementRef) {
    //not working var index = globalResizeEventListenerReferences.indexOf(elementRef);
    var index = findElementIndexById(globalResizeEventListenerReferences, elementRef);
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
function addWindowMouseDownEventListener(elementRef) {
    if (!globalMouseDownEventListener) {
        globalMouseDownEventListener = window.addEventListener('mousedown', windowMouseDownHandler);
    }

    globalMouseDownEventListenerReferences.push(elementRef);
}
function removeWindowMouseDownEventListener(elementRef) {
    //not working var index = globalMouseDownEventListenerReferences.indexOf(elementRef);
    var index = findElementIndexById(globalMouseDownEventListenerReferences, elementRef);
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

function expansionPanelToggle(elementRef) {
    if (elementRef) {
        var expander = elementRef;

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

                expansionPanelEventListenerReferences.forEach((item) => {
                    item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: false });
                });
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

                expansionPanelEventListenerReferences.forEach((item) => {
                    item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: true });
                });
            }, 500, expander);
        }
    }
}
//
function dropSelection(element) {
    if (element) {
        element.setSelectionRange(100, 100);
    }
}
//
function getInputSelectionRange(element) {
    if (element) {
        return {
            selectionStart: element.selectionStart,
            selectionEnd: element.selectionEnd
        };
    }
}
//
function setInputCaretIndex(element, index) {
    if (element) {
        if (element.createTextRange) {
            var range = element.createTextRange();
            range.move('character', index);
            range.select();
        } else {
            element.focus();
            element.setSelectionRange(index, index);
        }
    }
}
//
const focusableElementsSelector = "input:not([disabled]), textarea:not([disabled]), select:not([disabled]), button:not([disabled]), a[href]:not([disabled]), [tabindex = \"0\"]";
//
function focusPrevious(element) {
    if (element) {
        var inputs = document.querySelectorAll(focusableElementsSelector);
        if (inputs.length > 0) {
            for (var i = 1; i < inputs.length; i++) {
                if (inputs[i] == element) {
                    //TODO: A
                    inputs[i - 1].focus();
                    return;
                }
            }
            //If not found already and the selector is correct then the element is the first in the page. Start over again.
            inputs[inputs.length - 1].focus();
        }
    }
}
//
function focusNext(element) {
    //console.log(element);
    if (element) {
        var inputs = document.querySelectorAll(focusableElementsSelector);
        if (inputs.length > 0) {
            for (var i = 0; i < inputs.length - 1; i++) {
                if (inputs[i] == element) {
                    //TODO: A
                    inputs[i + 1].focus();
                    //console.log(inputs[i + 1]);
                    return;
                }
            }
            //If not found already and the selector is correct then the element is the last in the page. Start over again.
            inputs[0].focus();
            //console.log("start");
        }
    }
}
//
function focusTrapCheck(e) {
    if (!(e.key == 'Tab' || e.keyCode == 9))
        return;

    var dialog = e.target.closest('.giz-dialog');

    if (dialog) {
        var inputs = dialog.querySelectorAll(focusableElementsSelector);

        if (e.shiftKey) {
            if (document.activeElement == inputs[0]) {
                inputs[inputs.length - 1].focus();
                e.preventDefault();
            }
        } else {
            if (document.activeElement == inputs[inputs.length - 1]) {
                inputs[0].focus();
                e.preventDefault();
            }
        }
    }
}
//
function focusTrap(element) {
    if (element) {
        element.addEventListener('keydown', focusTrapCheck);
        element.focus();
    }
}
//
function focusUntrap(element) {
    if (element) {
        element.removeEventListener('keydown', focusTrapCheck);
    }
}

var registeredExpansionPanels = [];

function registerExpansionPanel(elementRef, isCollapsed) {
    registeredExpansionPanels.push({
        element: elementRef,
        isCollapsed: isCollapsed
    });
}

function unregisterExpansionPanel(elementRef) {
    var objRefIndex = -1;

    registeredExpansionPanels.forEach((item, index) => {
        if (item.element.id == elementRef.id)
            objRefIndex = index;
    });

    if (objRefIndex > -1) {
        expansionPanelEventListenerReferences.splice(objRefIndex, 1);
    }
}

var expansionPanelEventListenerReferences = [];

function addExpansionPanelEventListener(elementRef) {
    expansionPanelEventListenerReferences.push(elementRef);
}

function removeExpansionPanelEventListener(elementRef) {
    var index = findElementIndexById(expansionPanelEventListenerReferences, elementRef);
    if (index > -1) {
        expansionPanelEventListenerReferences.splice(index, 1);
    }
}