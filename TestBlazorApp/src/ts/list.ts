function initDraggable(listId: string, dotnetObject: any) {
    const draggableList = document.getElementById(listId) as HTMLDivElement | null;

    if (!draggableList) {
        console.error(`Element with id ${listId} not found`);
        return;
    }

    let draggedItem: HTMLDivElement | null = null;

    function getVisibleDraggableChildren(container: HTMLElement): HTMLElement[] {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && ((el as HTMLElement).draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        }) as HTMLElement[];
    }

    function getDragAfterElement(container: HTMLElement, clientY: number): HTMLElement | null {
        const children = getVisibleDraggableChildren(container);
        let closest = {offset: Number.NEGATIVE_INFINITY, element: null as HTMLElement | null};
        for (const child of children) {
            const rect = child.getBoundingClientRect();
            const offset = clientY - (rect.top + rect.height * 2);
            if (offset < 0 && offset > closest.offset) {
                closest = {offset, element: child};
            }
        }
        return closest.element;
    }

    draggableList.addEventListener('dragstart', (e: DragEvent) => {
        const targetItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (targetItem) {
            draggedItem = targetItem;
            draggedItem.classList.add('dragging');
            requestAnimationFrame(() => (draggedItem!.style.opacity = '0.5'));
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
        if (draggedItem) {
            e.preventDefault();
            const afterElement = getDragAfterElement(draggableList, e.clientY);
            if (afterElement && afterElement !== draggedItem) {
                draggableList.insertBefore(draggedItem, afterElement.nextSibling);
            }
        }
    });

    draggableList.addEventListener('drop', async (e: DragEvent) => {
        if (draggedItem) {
            e.preventDefault();

            const afterElement = getDragAfterElement(draggableList, e.clientY);
            const draggedItemId = draggedItem.getAttribute('id');
            const targetItemId = afterElement?.getAttribute('id') ?? null;

            draggedItem.style.opacity = '';
            draggedItem.classList.remove('dragging');
            draggedItem = null;

            if (draggedItemId) {
                try {
                    await dotnetObject.invokeMethodAsync('HandleDragDrop', draggedItemId, targetItemId);
                } catch (error) {
                    console.error('Error invoking HandleDragDrop:', error);
                }
            }
        }
    });
}

(window as any).initDraggable = initDraggable;