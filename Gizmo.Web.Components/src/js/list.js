"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
function initDraggable(listId, dotnetObject) {
    const draggableList = document.getElementById(listId);
    if (!draggableList) {
        console.error(`Element with id ${listId} not found`);
        return;
    }
    let draggedItem = null;
    function getDragAfterElement(container, clientY) {
        const draggableElements = [...container.querySelectorAll('div:not(.dragging)')];
        return draggableElements.reduce((closest, child) => {
            const { top, height } = child.getBoundingClientRect();
            const offset = clientY - top - height * 2;
            return offset < 0 && (!closest || offset > closest.offsetTop) ? child : closest;
        }, null);
    }
    draggableList.addEventListener('dragstart', (e) => {
        const target = e.target;
        if (target) {
            draggedItem = target;
            target.classList.add('dragging');
            requestAnimationFrame(() => target.style.opacity = '0.5');
        }
    });
    draggableList.addEventListener('dragend', () => {
        if (draggedItem) {
            draggedItem.classList.remove('dragging');
            draggedItem.style.opacity = '';
            draggedItem = null;
        }
    });
    draggableList.addEventListener('dragover', (e) => {
        if (draggedItem) {
            e.preventDefault();
            const afterElement = getDragAfterElement(draggableList, e.clientY);
            if (afterElement && afterElement !== draggedItem) {
                draggableList.insertBefore(draggedItem, afterElement.nextSibling);
            }
        }
    });
    draggableList.addEventListener('drop', (e) => __awaiter(this, void 0, void 0, function* () {
        var _a;
        if (draggedItem) {
            e.preventDefault();
            draggedItem.style.opacity = '';
            const afterElement = getDragAfterElement(draggableList, e.clientY);
            const draggedItemId = draggedItem.getAttribute('id');
            const targetItemId = (_a = afterElement === null || afterElement === void 0 ? void 0 : afterElement.getAttribute('id')) !== null && _a !== void 0 ? _a : null;
            if (draggedItemId) {
                try {
                    yield dotnetObject.invokeMethodAsync('HandleDragDrop', draggedItemId, targetItemId);
                }
                catch (error) {
                    console.error('Error invoking HandleDragDrop:', error);
                }
            }
            if (draggedItem) {
                draggedItem.classList.remove('dragging');
                draggedItem = null;
            }
        }
    }));
}
window.initDraggable = initDraggable;
