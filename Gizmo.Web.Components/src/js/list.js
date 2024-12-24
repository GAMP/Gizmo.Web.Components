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
    function getVisibleDraggableChildren(container) {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && (el.draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        });
    }
    function getDragAfterElement(container, clientY) {
        const children = getVisibleDraggableChildren(container);
        let closest = { offset: Number.NEGATIVE_INFINITY, element: null };
        for (const child of children) {
            const rect = child.getBoundingClientRect();
            const offset = clientY - (rect.top + rect.height * 2);
            if (offset < 0 && offset > closest.offset) {
                closest = { offset, element: child };
            }
        }
        return closest.element;
    }
    draggableList.addEventListener('dragstart', (e) => {
        const targetItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (targetItem) {
            draggedItem = targetItem;
            draggedItem.classList.add('dragging');
            requestAnimationFrame(() => (draggedItem.style.opacity = '0.5'));
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
            const afterElement = getDragAfterElement(draggableList, e.clientY);
            const draggedItemId = draggedItem.getAttribute('id');
            const targetItemId = (_a = afterElement === null || afterElement === void 0 ? void 0 : afterElement.getAttribute('id')) !== null && _a !== void 0 ? _a : null;
            draggedItem.style.opacity = '';
            draggedItem.classList.remove('dragging');
            draggedItem = null;
            if (draggedItemId) {
                try {
                    yield dotnetObject.invokeMethodAsync('HandleDragDrop', draggedItemId, targetItemId);
                }
                catch (error) {
                    console.error('Error invoking HandleDragDrop:', error);
                }
            }
        }
    }));
}
window.initDraggable = initDraggable;
