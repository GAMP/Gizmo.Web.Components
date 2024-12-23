function initDraggable(listId: string) {
    const gizmoSortableList = document.getElementById(listId) as HTMLDivElement;
    let gizmoDraggedItem: HTMLDivElement | null = null;
    let lastDragOverTime = 0;

    function getGizmoDragAfterElement(container: HTMLElement, y: number): HTMLElement | null {
        const draggableElements = Array.from(container.querySelectorAll('div:not(.dragging)')) as HTMLElement[];
        return draggableElements.reduce<HTMLElement | null>((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 3;
            if (offset < 0 && offset > (closest?.getBoundingClientRect().top ?? Number.NEGATIVE_INFINITY)) {
                return child;
            } else {
                return closest;
            }
        }, null);
    }

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
        const now = Date.now();
        if (now - lastDragOverTime < 700) return; // Debounce: limit to one update every 700ms
        lastDragOverTime = now;

        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem && afterElement && afterElement !== gizmoDraggedItem) {
            gizmoSortableList.querySelectorAll('div:not(.dragging)').forEach(item => {
                (item as HTMLDivElement).style.transform = '';
            });
            const draggedIndex = Array.from(gizmoSortableList.children).indexOf(gizmoDraggedItem);
            const afterIndex = Array.from(gizmoSortableList.children).indexOf(afterElement);
            if (draggedIndex > afterIndex) {
                afterElement.style.transform = 'translateY(50px)';
            } else {
                afterElement.style.transform = 'translateY(-50px)';
            }
        }
    });

    gizmoSortableList.addEventListener('dragleave', () => {
        gizmoSortableList.querySelectorAll('div').forEach(item => {
            (item as HTMLDivElement).style.transform = '';
        });
    });

    gizmoSortableList.addEventListener('drop', (e: DragEvent) => {
        e.preventDefault();
        const afterElement = getGizmoDragAfterElement(gizmoSortableList, e.clientY);
        if (gizmoDraggedItem) {
            if (afterElement == null) {
                gizmoSortableList.appendChild(gizmoDraggedItem);
            } else if (gizmoSortableList.contains(afterElement)) {
                gizmoSortableList.insertBefore(gizmoDraggedItem, afterElement);
            }
            gizmoSortableList.querySelectorAll('div').forEach(item => {
                (item as HTMLDivElement).style.transform = '';
            });
            gizmoDraggedItem.classList.remove('dragging');
            gizmoDraggedItem = null;
        }
    });
}

(window as any).initDraggable = initDraggable;