function initDraggable(listId: string, dotnetObject: any) {
    const draggableList = document.getElementById(listId) as HTMLDivElement | null;

    if (!draggableList)
    {
        console.error(`Element with id ${listId} not found`);
        return;
    }

    let draggedItem: HTMLDivElement | null = null;

    function getDragAfterElement(container: HTMLElement, clientY: number): HTMLElement | null {
        const draggableElements = [...container.querySelectorAll('div:not(.dragging)')] as HTMLElement[];
        return draggableElements.reduce<HTMLElement | null>((closest, child) => {
            const {top, height} = child.getBoundingClientRect();
            const offset = clientY - top - height * 2;
            return offset < 0 && (!closest || offset > closest.offsetTop) ? child : closest;
        }, null);
    }

    draggableList.addEventListener('dragstart', (e: DragEvent) => {
        const target = e.target as HTMLDivElement;
        if (target) {
            draggedItem = target;
            target.classList.add('dragging');
            requestAnimationFrame(() => target.style.opacity = '0.5');
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
        }
    });
}

(window as any).initDraggable = initDraggable;