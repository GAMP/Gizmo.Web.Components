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
    let targetElement = null;
    function getVisibleDraggableElements(container) {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && (el.draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        });
    }
    function getTargetItem(container, clientY) {
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
        const draggedItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (draggedItem) {
            draggedElement = draggedItem;
            draggedElement.classList.add('dragging');
            requestAnimationFrame(() => (draggedElement.style.opacity = '0.5'));
        }
        console.log(Array.from(draggableList.children).map(child => child.id).join('\n'));
    });
    draggableList.addEventListener('dragend', () => {
        if (draggedElement) {
            draggedElement.classList.remove('dragging');
            draggedElement.style.opacity = '';
            draggedElement = null;
        }
        console.log(Array.from(draggableList.children).map(child => child.id).join('\n'));
    });
    draggableList.addEventListener('dragover', (e) => {
        var _a;
        if (draggedElement) {
            e.preventDefault();
            const targetItem = getTargetItem(draggableList, e.clientY);
            if (targetItem && targetItem !== draggedElement) {
                let element = draggableList.insertBefore(draggedElement, targetItem);
                targetElement = ((_a = element.previousElementSibling) !== null && _a !== void 0 ? _a : element.nextElementSibling);
            }
            else {
                const element = draggableList.appendChild(draggedElement);
                targetElement = element.previousElementSibling;
            }
        }
    });
    draggableList.addEventListener('drop', (e) => __awaiter(this, void 0, void 0, function* () {
        if (draggedElement) {
            e.preventDefault();
            draggedElement.style.opacity = '';
            draggedElement.classList.remove('dragging');
            try {
                yield dotnetObject.invokeMethodAsync('HandleDragDrop', draggedElement.id, targetElement === null || targetElement === void 0 ? void 0 : targetElement.id);
            }
            catch (error) {
                console.error('Error invoking HandleDragDrop:', error);
            }
            draggedElement = null;
        }
    }));
}
window.initDraggable = initDraggable;
