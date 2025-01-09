function initDraggable(listId: string, dotnetObject: any) {
    const draggableList = document.getElementById(listId) as HTMLDivElement | null;

    if (!draggableList) {
        console.error(`Element with id ${listId} not found`);
        return;
    }

    let draggedElement: HTMLDivElement | null = null;
    let targetElement: HTMLDivElement | null = null;

    function getVisibleDraggableElements(container: HTMLElement): HTMLElement[] {
        return Array.from(container.children).filter((el) => {
            const isElement = el instanceof HTMLElement;
            const isVisible = isElement && el.style.display !== 'none';
            const notDragging = isElement && !el.classList.contains('dragging');
            const isDraggable = isElement && ((el as HTMLElement).draggable || el.getAttribute('draggable') !== 'false');
            return isElement && isVisible && notDragging && isDraggable;
        }) as HTMLElement[];
    }

    function getTargetItem(container: HTMLElement, clientY: number): HTMLElement | null {
        const children = getVisibleDraggableElements(container);
        let closest: { offset: number; element: HTMLElement | null } = {
            offset: Number.NEGATIVE_INFINITY,
            element: null
        };

        children.forEach(element => {
            const rect = element.getBoundingClientRect();
            const offset = clientY - (rect.top + rect.height / 2);

            if (offset < 0 && offset > closest.offset) {
                closest = {offset, element};
            }
        });

        if (!closest.element && children.length > 0) {
            const lastElement = children[children.length - 1];
            const lastRect = lastElement.getBoundingClientRect();
            if (clientY > lastRect.bottom) {
                return null;
            }
        }

        return closest.element;
    }

    draggableList.addEventListener('dragstart', (e: DragEvent) => {
        const draggedItem = e.target instanceof HTMLDivElement ? e.target : null;
        if (draggedItem) {
            draggedElement = draggedItem;
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

            const targetItem = getTargetItem(draggableList, e.clientY);

            if (targetItem && targetItem !== draggedElement) {
                let element = draggableList.insertBefore(draggedElement, targetItem)
                targetElement = (element.previousElementSibling ?? element.nextElementSibling) as HTMLDivElement;
            } else {
                const element = draggableList.appendChild(draggedElement);
                targetElement = element.previousElementSibling as HTMLDivElement;
            }
        }
    });

    draggableList.addEventListener('drop', async (e: DragEvent) => {
        if (draggedElement) {
            e.preventDefault();

            draggedElement.style.opacity = '';
            draggedElement.classList.remove('dragging');

            try {
                await dotnetObject.invokeMethodAsync('HandleDragDrop', draggedElement.id, targetElement?.id);
            } catch (error) {
                console.error('Error invoking HandleDragDrop:', error);
            }

            draggedElement = null;
        }
    });
}

(window as any).initDraggable = initDraggable;