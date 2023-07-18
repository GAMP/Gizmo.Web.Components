/** @format */

//
window.getWindowSize = function getWindowSize() {
  return {
    width: window.innerWidth,
    height: window.innerHeight,
  };
};
//
window.getElementBoundingClientRect = function getElementBoundingClientRect(element) {
    if (element) {
        return element.getBoundingClientRect();
    } else {
        console.log("Cannot read getBoundingClientRect of null element.");
        return {
            "x": 0,
            "y": 0,
            "width": 0,
            "height": 0,
            "top": 0,
            "right": 0,
            "bottom": 0,
            "left": 0
        };
    }
};
//
window.scrollListItemIntoView = function scrollListItemIntoView(element) {
  if (element) {
    let parent = element.parentElement;

    let { top: eTop } = element.getBoundingClientRect();
    let { top: pTop } = parent.getBoundingClientRect();

    parent.parentElement.parentElement.scrollTop = eTop - pTop;
  }
};
//
window.scrollDatePickerYear = function scrollDatePickerYear() {
  var items = document.getElementsByClassName('giz-date-picker-year-count active');

  for (var i = 0; i < items.length; i++) {
    items[i].scrollIntoView({ block: 'center' });
  }
};
//
window.focusElement = function focusElement(element) {
  element.focus();
};
//
window.setPropByElement = function setPropByElement(element, property, value) {
  element[property] = value;
};
//
window.scrollItemToTop = function scrollItemToTop(element) {
  if (element) {
    let parent = element.parentElement;

    let { top: eTop } = element.getBoundingClientRect();
    let { top: pTop } = parent.getBoundingClientRect();

    parent.scrollTop = eTop - pTop;
  }
};
//
window.getElementScrollSize = function getElementScrollSize(element) {
  if (element) {
    let parent = element.parentElement;

    return {
      width: parent.scrollLeft,
      height: parent.scrollTop,
    };
  }

  return {
    width: 0,
    height: 0,
  };
};
//
window.findElementIndexById = function findElementIndexById(list, elementRef) {
  var objRefIndex = -1;

  list.forEach((item, index) => {
    if (item._id == elementRef._id) objRefIndex = index;
  });

  return objRefIndex;
};
//
var globalResizeEventListener;
var globalResizeEventListenerReferences = [];
window.addWindowResizeEventListener = function addWindowResizeEventListener(elementRef) {
  if (!globalResizeEventListener) {
    globalResizeEventListener = window.addEventListener('resize', windowResizeHandler);
  }

  globalResizeEventListenerReferences.push(elementRef);
};
window.removeWindowResizeEventListener = function removeWindowResizeEventListener(elementRef) {
  //not working var index = globalResizeEventListenerReferences.indexOf(elementRef);
  var index = findElementIndexById(globalResizeEventListenerReferences, elementRef);
  if (index > -1) {
    globalResizeEventListenerReferences.splice(index, 1);
  }
};
window.windowResizeHandler = function windowResizeHandler(event) {
  globalResizeEventListenerReferences.forEach(item => {
    item.invokeMethodAsync('OnWindowResizeEvent', { width: window.innerWidth, height: window.innerHeight });
  });
};
//
var globalMouseDownEventListener;
var globalMouseDownEventListenerReferences = [];
window.addWindowMouseDownEventListener = function addWindowMouseDownEventListener(elementRef) {
  if (!globalMouseDownEventListener) {
    globalMouseDownEventListener = window.addEventListener('mousedown', windowMouseDownHandler);
  }

  globalMouseDownEventListenerReferences.push(elementRef);
};
window.removeWindowMouseDownEventListener = function removeWindowMouseDownEventListener(elementRef) {
  //not working var index = globalMouseDownEventListenerReferences.indexOf(elementRef);
  var index = findElementIndexById(globalMouseDownEventListenerReferences, elementRef);
  if (index > -1) {
    globalMouseDownEventListenerReferences.splice(index, 1);
  }
};
window.windowMouseDownHandler = function windowMouseDownHandler(event) {
  globalMouseDownEventListenerReferences.forEach(item => {
    item.invokeMethodAsync('OnWindowMouseDownEvent', { clientX: event.clientX, clientY: event.clientY });
  });
};
//
window.writeLine = function writeLine(message) {
  window.console.log(message);
};

var expansionPanelOperations = [];

window.expansionPanelToggle = function expansionPanelToggle(elementRef) {
  if (elementRef) {
    var expander = elementRef;

    if (expansionPanelOperations[expander]) return;

    if (!expander.classList.contains('expanded')) {
      expander.classList.add('expanded');
      var body = expander.querySelector('.giz-expansion-panel__body');
      var height = body.getBoundingClientRect().height;

      body.style.setProperty('--abh', height + 'px');
      expander.classList.add('expanding');

      var expansionPanelTimeout = setTimeout(
        function () {
          expander.classList.remove('expanding');
          expansionPanelOperations[expander] = null;

          expansionPanelEventListenerReferences.forEach(item => {
            item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: false });
          });
        },
        500,
        expander,
      );

      expansionPanelOperations[expander] = expansionPanelTimeout;
    } else {
      var body = expander.querySelector('.giz-expansion-panel__body');
      var height = body.getBoundingClientRect().height;

      body.style.setProperty('--abh', height + 'px');
      expander.classList.add('collapsing');

      var expansionPanelTimeout = setTimeout(
        function () {
          expander.classList.remove('expanded');
          expander.classList.remove('collapsing');
          expansionPanelOperations[expander] = null;

          expansionPanelEventListenerReferences.forEach(item => {
            item.invokeMethodAsync('OnExpansionPanelEvent', { Id: expander.id, IsCollapsed: true });
          });
        },
        500,
        expander,
      );
    }
  }
};
//
window.dropSelection = function dropSelection(element) {
  if (element) {
    element.setSelectionRange(100, 100);
  }
};
//
window.getInputSelectionRange = function getInputSelectionRange(element) {
  if (element) {
    return {
      selectionStart: element.selectionStart,
      selectionEnd: element.selectionEnd,
    };
  }
};
//
window.setInputCaretIndex = function setInputCaretIndex(element, index) {
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
};
//
const focusableElementsSelector = 'input:not([disabled]), textarea:not([disabled]), select:not([disabled]), button:not([disabled]), a[href]:not([disabled]), [tabindex = "0"]';
//
window.focusPrevious = function focusPrevious(element) {
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
};
//
window.focusNext = function focusNext(element) {
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
};
//
window.focusTrapCheck = function focusTrapCheck(e) {
  if (!(e.key == 'Tab' || e.keyCode == 9)) return;

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
};
//
window.focusTrap = function focusTrap(element) {
  if (element) {
    element.addEventListener('keydown', focusTrapCheck);
    element.focus();
  }
};
//
window.focusUntrap = function focusUntrap(element) {
  if (element) {
    element.removeEventListener('keydown', focusTrapCheck);
  }
};

var registeredExpansionPanels = [];

window.registerExpansionPanel = function registerExpansionPanel(elementRef, isCollapsed) {
  registeredExpansionPanels.push({
    element: elementRef,
    isCollapsed: isCollapsed,
  });
};

window.unregisterExpansionPanel = function unregisterExpansionPanel(elementRef) {
  var objRefIndex = -1;

  registeredExpansionPanels.forEach((item, index) => {
    if (item.element.id == elementRef.id) objRefIndex = index;
  });

  if (objRefIndex > -1) {
    expansionPanelEventListenerReferences.splice(objRefIndex, 1);
  }
};

var expansionPanelEventListenerReferences = [];

window.addExpansionPanelEventListener = function addExpansionPanelEventListener(elementRef) {
  expansionPanelEventListenerReferences.push(elementRef);
};

window.removeExpansionPanelEventListener = function removeExpansionPanelEventListener(elementRef) {
  var index = findElementIndexById(expansionPanelEventListenerReferences, elementRef);
  if (index > -1) {
    expansionPanelEventListenerReferences.splice(index, 1);
  }
};
