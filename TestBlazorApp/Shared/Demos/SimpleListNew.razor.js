"use strict";
/** @format */
// Get the list container
const sortableList = document.getElementById('sortable-list');
// Store the dragged element
let draggedItem = null;
function getDragAfterElement(container, y) {
    const draggableElements = Array.from(container.querySelectorAll('li:not(.dragging)'));
    return draggableElements.reduce((closest, child) => {
        var _a;
        const box = child.getBoundingClientRect();
        const offset = y - box.top - box.height / 2; // Distance from center
        if (offset < 0 && offset > ((_a = closest === null || closest === void 0 ? void 0 : closest.getBoundingClientRect().top) !== null && _a !== void 0 ? _a : Number.NEGATIVE_INFINITY)) {
            return child;
        }
        else {
            return closest;
        }
    }, null);
}
// Event listeners for drag-and-drop
sortableList.addEventListener('dragstart', (e) => {
    if (e.target && e.target instanceof HTMLLIElement) {
        draggedItem = e.target;
        e.target.classList.add('dragging');
    }
});
sortableList.addEventListener('dragend', (e) => {
    if (draggedItem) {
        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
});
sortableList.addEventListener('dragover', (e) => {
    e.preventDefault();
    const afterElement = getDragAfterElement(sortableList, e.clientY);
    if (draggedItem && afterElement && afterElement !== draggedItem) {
        sortableList.querySelectorAll('li:not(.dragging)').forEach(item => {
            const li = item;
            li.style.transform = '';
        });
        const draggedIndex = Array.from(sortableList.children).indexOf(draggedItem);
        const afterIndex = Array.from(sortableList.children).indexOf(afterElement);
        if (draggedIndex > afterIndex) {
            afterElement.style.transform = 'translateY(20px)';
        }
        else {
            afterElement.style.transform = 'translateY(-20px)';
        }
    }
});
sortableList.addEventListener('dragleave', () => {
    sortableList.querySelectorAll('li').forEach(item => {
        const li = item;
        li.style.transform = '';
    });
});
sortableList.addEventListener('drop', (e) => {
    e.preventDefault();
    const afterElement = getDragAfterElement(sortableList, e.clientY);
    if (draggedItem) {
        if (afterElement == null) {
            sortableList.appendChild(draggedItem);
        }
        else {
            sortableList.insertBefore(draggedItem, afterElement);
        }
        // Reset transforms
        sortableList.querySelectorAll('li').forEach(item => {
            const li = item;
            li.style.transform = '';
        });
        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
});
