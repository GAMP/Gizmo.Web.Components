type Direction = "down" | "up";

function initDraggable(listId: string, dotnetObject: any) {
    const draggableList = document.getElementById(listId) as HTMLDivElement | null;

    if (!draggableList) {
        console.error(`Element with id ${listId} not found`);
        return;
    }

    let draggedElement: HTMLDivElement | null = null;

    function getVisibleDraggableElements(container: HTMLElement): HTMLElement[] {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && ((el as HTMLElement).draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        }) as HTMLElement[];
    }

    function getTargetElement(container: HTMLElement, clientY: number): HTMLElement | null {
        const children = getVisibleDraggableElements(container);
        let closest = {
            offset: Number.NEGATIVE_INFINITY,
            element: null as HTMLElement | null
        };

        for (const element of children) {
            const rect = element.getBoundingClientRect();
            const offset = clientY - (rect.top + rect.height / 2);

            if (offset < 0 && offset > closest.offset) {
                closest = {offset, element};
            }
        }
        return closest.element;
    }

    draggableList.addEventListener('dragstart', (e: DragEvent) => {
        const targetItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (targetItem) {
            draggedElement = targetItem;
            draggedElement.classList.add('dragging');
            requestAnimationFrame(() => (draggedElement!.style.opacity = '0.5'));
        }
    });

    draggableList.addEventListener('dragend', () => {
        if (draggedElement) {
            draggedElement.classList.remove('dragging');
            draggedElement.style.opacity = '';
            draggedElement = null;
        }
    });

    draggableList.addEventListener('dragover', (e: DragEvent) => {
        if (draggedElement) {
            e.preventDefault();

            const targetElement = getTargetElement(draggableList, e.clientY);
            if (targetElement && targetElement !== draggedElement) {
                draggableList.insertBefore(draggedElement, targetElement);
            }
        }
    });

    draggableList.addEventListener('drop', async (e: DragEvent) => {
        if (draggedElement) {
            e.preventDefault();

            const targetElement = getTargetElement(draggableList, e.clientY);
            const targetElementId = targetElement?.getAttribute('id') ?? null;
            const draggedElementId = draggedElement.getAttribute('id');

            draggedElement.style.opacity = '';
            draggedElement.classList.remove('dragging');
            draggedElement = null;

            if (draggedElementId) {
                try {
                    await dotnetObject.invokeMethodAsync('HandleDragDrop', draggedElementId, targetElementId);
                } catch (error) {
                    console.error('Error invoking HandleDragDrop:', error);
                }
            }
        }
    });
}

(window as any).initDraggable = initDraggable;