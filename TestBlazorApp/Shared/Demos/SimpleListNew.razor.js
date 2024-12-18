"use strict";
/** @format */
// Get the list container
const sortableList = document.getElementById('sortable-list');
// Store the dragged element
let draggedItem = null;
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
    if (draggedItem && afterElement.element && afterElement.element !== draggedItem) {
        sortableList.querySelectorAll('li:not(.dragging)').forEach(item => {
            const li = item;
            li.style.transform = '';
        });
        const draggedIndex = Array.from(sortableList.children).indexOf(draggedItem);
        const afterIndex = Array.from(sortableList.children).indexOf(afterElement.element);
        if (draggedIndex > afterIndex) {
            afterElement.element.style.transform = 'translateY(20px)';
        }
        else {
            afterElement.element.style.transform = 'translateY(-20px)';
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
        if (afterElement.element == null) {
            sortableList.appendChild(draggedItem);
        }
        else {
            sortableList.insertBefore(draggedItem, afterElement.element);
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
function getDragAfterElement(container, y) {
    const draggableElements = Array.from(container.querySelectorAll('li:not(.dragging)'));
    return draggableElements.reduce((closest, child) => {
        const box = child.getBoundingClientRect();
        const offset = y - box.top - box.height / 2; // Distance from center
        if (offset < 0 && offset > closest.offset) {
            return { offset: offset, element: child };
        }
        else {
            return closest;
        }
    }, { offset: Number.NEGATIVE_INFINITY, element: null });
}
