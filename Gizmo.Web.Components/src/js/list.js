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
    let draggedElement = null;
    function getVisibleDraggableElements(container) {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && (el.draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        });
    }
    function getTargetElement(container, clientY) {
        const children = getVisibleDraggableElements(container);
        let closest = {
            offset: Number.NEGATIVE_INFINITY,
            element: null
        };
        children.forEach(element => {
            const rect = element.getBoundingClientRect();
            const offset = clientY - (rect.top + rect.height / 2);
            if (offset < 0 && offset > closest.offset) {
                closest = { offset, element };
            }
        });
        if (!closest.element && children.length > 0) {
            const lastElement = children[children.length - 1];
            const lastRect = lastElement.getBoundingClientRect();
            if (clientY > lastRect.bottom) {
                return null;
            }
        }
        return closest.element;
    }
    draggableList.addEventListener('dragstart', (e) => {
        const targetItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (targetItem) {
            draggedElement = targetItem;
            draggedElement.classList.add('dragging');
            requestAnimationFrame(() => (draggedElement.style.opacity = '0.5'));
        }
    });
    draggableList.addEventListener('dragend', () => {
        if (draggedElement) {
            draggedElement.classList.remove('dragging');
            draggedElement.style.opacity = '';
            draggedElement = null;
        }
    });
    draggableList.addEventListener('dragover', (e) => {
        if (draggedElement) {
            e.preventDefault();
            const targetElement = getTargetElement(draggableList, e.clientY);
            if (targetElement && targetElement !== draggedElement) {
                draggableList.insertBefore(draggedElement, targetElement);
            }
            else {
                draggableList.appendChild(draggedElement);
            }
        }
    });
    draggableList.addEventListener('drop', (e) => __awaiter(this, void 0, void 0, function* () {
        var _a;
        if (draggedElement) {
            e.preventDefault();
            const targetElement = getTargetElement(draggableList, e.clientY);
            const targetElementId = (_a = targetElement === null || targetElement === void 0 ? void 0 : targetElement.getAttribute('id')) !== null && _a !== void 0 ? _a : null;
            const draggedElementId = draggedElement.getAttribute('id');
            draggedElement.style.opacity = '';
            draggedElement.classList.remove('dragging');
            draggedElement = null;
            if (draggedElementId) {
                try {
                    yield dotnetObject.invokeMethodAsync('HandleDragDrop', draggedElementId, targetElementId);
                }
                catch (error) {
                    console.error('Error invoking HandleDragDrop:', error);
                }
            }
        }
    }));
}
window.initDraggable = initDraggable;
