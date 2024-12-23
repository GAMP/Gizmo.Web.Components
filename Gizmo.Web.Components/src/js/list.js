"use strict";
function initDraggable(listId) {
    const gizmoSortableList = document.getElementById(listId);
    let gizmoDraggedItem = null;
    let lastDragOverTime = 0;
    function getGizmoDragAfterElement(container, y) {
        const draggableElements = Array.from(container.querySelectorAll('div:not(.dragging)'));
        return draggableElements.reduce((closest, child) => {
            var _a;
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 3;
            if (offset < 0 && offset > ((_a = closest === null || closest === void 0 ? void 0 : closest.getBoundingClientRect().top) !== null && _a !== void 0 ? _a : Number.NEGATIVE_INFINITY)) {
                return child;
            }
            else {
                return closest;
            }
        }, null);
    }
    gizmoSortableList.addEventListener('dragstart', (e) => {
        if (e.target && e.target instanceof HTMLDivElement) {
            gizmoDraggedItem = e.target;
            e.target.classList.add('dragging');
        }
    });
    gizmoSortableList.addEventListener('dragend', (e) => {
        if (gizmoDraggedItem) {
            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });
    gizmoSortableList.addEventListener('dragover', (e) => {
        e.preventDefault();
        const now = Date.now();
        if (now - lastDragOverTime < 700)
            return; // Debounce: limit to one update every 700ms
        lastDragOverTime = now;
        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem && afterElement && afterElement !== gizmoDraggedItem) {
            gizmoSortableList.querySelectorAll('div:not(.dragging)').forEach(item => {
                item.style.transform = '';
            });
            const draggedIndex = Array.from(gizmoSortableList.children).indexOf(gizmoDraggedItem);
            const afterIndex = Array.from(gizmoSortableList.children).indexOf(afterElement);
            if (draggedIndex > afterIndex) {
                afterElement.style.transform = 'translateY(50px)';
            }
            else {
                afterElement.style.transform = 'translateY(-50px)';
            }
        }
    });
    gizmoSortableList.addEventListener('dragleave', () => {
        gizmoSortableList.querySelectorAll('div').forEach(item => {
            item.style.transform = '';
        });
    });
    gizmoSortableList.addEventListener('drop', (e) => {
        e.preventDefault();
        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem) {
            if (afterElement == null) {
                gizmoSortableList.appendChild(gizmoDraggedItem);
            }
            else if (gizmoSortableList.contains(afterElement)) {
                gizmoSortableList.insertBefore(gizmoDraggedItem, afterElement);
            }
            gizmoSortableList.querySelectorAll('div').forEach(item => {
                item.style.transform = '';
            });
            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });
}
window.initDraggable = initDraggable;
