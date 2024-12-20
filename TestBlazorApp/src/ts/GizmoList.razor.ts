/** @format */

export const gizmoInit = (listId: string) => {
    // Get the list container
    const gizmoSortableList = document.getElementById(listId) as HTMLDivElement;

    // Store the dragged element
    let gizmoDraggedItem: HTMLDivElement | null = null;

    function getGizmoDragAfterElement(container: HTMLElement, y: number): HTMLElement | null {
        const draggableElements = Array.from(container.querySelectorAll('div:not(.dragging)')) as HTMLElement[];

        return draggableElements.reduce<HTMLElement | null>((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 2; // Distance from center
            if (offset < 0 && offset > (closest?.getBoundingClientRect().top ?? Number.NEGATIVE_INFINITY)) {
                return child;
            } else {
                return closest;
            }
        }, null);
    }

    // Event listeners for drag-and-drop
    gizmoSortableList.addEventListener('dragstart', (e: DragEvent) => {
        if (e.target && e.target instanceof HTMLDivElement) {
            gizmoDraggedItem = e.target;
            e.target.classList.add('dragging');
        }
    });

    gizmoSortableList.addEventListener('dragend', (e: DragEvent) => {
        if (gizmoDraggedItem) {
            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });

    gizmoSortableList.addEventListener('dragover', (e: DragEvent) => {
        e.preventDefault();

        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem && afterElement && afterElement !== gizmoDraggedItem) {
            gizmoSortableList.querySelectorAll('div:not(.dragging)').forEach(item => {
                const div = item as HTMLDivElement;
                div.style.transform = '';
            });

            const draggedIndex = Array.from(gizmoSortableList.children).indexOf(gizmoDraggedItem);
            const afterIndex = Array.from(gizmoSortableList.children).indexOf(afterElement);

            if (draggedIndex > afterIndex) {
                afterElement.style.transform = 'translateY(20px)';
            } else {
                afterElement.style.transform = 'translateY(-20px)';
            }
        }
    });

    gizmoSortableList.addEventListener('dragleave', () => {
        gizmoSortableList.querySelectorAll('div').forEach(item => {
            const div = item as HTMLDivElement;
            div.style.transform = '';
        });
    });

    gizmoSortableList.addEventListener('drop', (e: DragEvent) => {
        e.preventDefault();

        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);

        if (gizmoDraggedItem) {
            if (afterElement == null) {
                gizmoSortableList.appendChild(gizmoDraggedItem);
            } else {
                gizmoSortableList.insertBefore(gizmoDraggedItem, afterElement);
            }

            // Reset transforms
            gizmoSortableList.querySelectorAll('div').forEach(item => {
                const div = item as HTMLDivElement;
                div.style.transform = '';
            });

            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });
}