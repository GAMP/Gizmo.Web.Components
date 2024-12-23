function initDraggable(listId: string, dotnetObject: any) {
    const draggableList = document.getElementById(listId) as HTMLDivElement | null;
    if (!draggableList) return;

    let draggedItem: HTMLDivElement | null = null;

    function getDragAfterElement(container: HTMLElement, y: number): HTMLElement | null {
        const draggableElements = Array.from(container.querySelectorAll('div:not(.dragging)')) as HTMLElement[];
        return draggableElements.reduce<HTMLElement | null>((closest, child) => {
            const box = child.getBoundingClientRect();
            const offset = y - box.top - box.height / 2;
            if (offset < 0 && (!closest || offset > closest.offsetTop)) {
                return child;
            } else {
                return closest;
            }
        }, null);
    }

    draggableList.addEventListener('dragstart', (e: DragEvent) => {
        if (e.target instanceof HTMLDivElement) {
            draggedItem = e.target;
            e.target.classList.add('dragging');
            requestAnimationFrame(() => (e.target as HTMLElement).style.opacity = '0.5');
        }
    });

    draggableList.addEventListener('dragend', () => {
        if (draggedItem) {
            draggedItem.classList.remove('dragging');
            draggedItem.style.opacity = '';
            draggedItem = null;
        }
    });

    draggableList.addEventListener('dragover', (e: DragEvent) => {
        e.preventDefault();
        if (!draggedItem) return;
        const afterElement = getDragAfterElement(draggableList, e.clientY);
        if (afterElement && afterElement !== draggedItem) {
            draggableList.insertBefore(draggedItem, afterElement.nextSibling);
        }
    });

    draggableList.addEventListener('drop', async (e: DragEvent) => {
        e.preventDefault();
        if (!draggedItem) return;
        draggedItem.style.opacity = '';

        const afterElement = getDragAfterElement(draggableList, e.clientY);
        const draggedItemId = draggedItem.getAttribute('id');
        const targetItemId = afterElement?.getAttribute('id') ?? null;

        if (draggedItemId) {
            try {
                await dotnetObject.invokeMethodAsync('HandleDragDrop', draggedItemId, targetItemId);
            } catch (error) {
                console.error('Error invoking HandleDragDrop:', error);
            }
        }

        if (draggedItem) {
            draggedItem.classList.remove('dragging');
            draggedItem = null;
        }
    });
}

(window as any).initDraggable = initDraggable;