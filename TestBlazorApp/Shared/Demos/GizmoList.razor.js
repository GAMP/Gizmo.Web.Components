/** @format */
export const gizmoInit = (listId) => {
    // Get the list container
    const gizmoSortableList = document.getElementById(listId);
    // Store the dragged element
    let gizmoDraggedItem = null;
    function getGizmoDragAfterElement(container, y) {
        const draggableElements = Array.from(container.querySelectorAll('div:not(.dragging)'));
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
        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem && afterElement && afterElement !== gizmoDraggedItem) {
            gizmoSortableList.querySelectorAll('div:not(.dragging)').forEach(item => {
                const div = item;
                div.style.transform = '';
            });
            const draggedIndex = Array.from(gizmoSortableList.children).indexOf(gizmoDraggedItem);
            const afterIndex = Array.from(gizmoSortableList.children).indexOf(afterElement);
            if (draggedIndex > afterIndex) {
                afterElement.style.transform = 'translateY(20px)';
            }
            else {
                afterElement.style.transform = 'translateY(-20px)';
            }
        }
    });
    gizmoSortableList.addEventListener('dragleave', () => {
        gizmoSortableList.querySelectorAll('div').forEach(item => {
            const div = item;
            div.style.transform = '';
        });
    });
    gizmoSortableList.addEventListener('drop', (e) => {
        e.preventDefault();
        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem) {
            if (afterElement == null) {
                gizmoSortableList.appendChild(gizmoDraggedItem);
            }
            else {
                gizmoSortableList.insertBefore(gizmoDraggedItem, afterElement);
            }
            // Reset transforms
            gizmoSortableList.querySelectorAll('div').forEach(item => {
                const div = item;
                div.style.transform = '';
            });
            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });
};
