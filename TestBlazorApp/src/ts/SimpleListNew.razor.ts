/** @format */

// Get the list container
const sortableList = document.getElementById('sortable-list') as HTMLUListElement;

// Store the dragged element
let draggedItem: HTMLLIElement | null = null;

function getDragAfterElement(container: HTMLElement, y: number): HTMLElement | null {
    const draggableElements = Array.from(container.querySelectorAll('li:not(.dragging)')) as HTMLElement[];

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
sortableList.addEventListener('dragstart', (e: DragEvent) => {
    if (e.target && e.target instanceof HTMLLIElement) {
        draggedItem = e.target;
        e.target.classList.add('dragging');
    }
});

sortableList.addEventListener('dragend', (e: DragEvent) => {
    if (draggedItem) {
        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
});

sortableList.addEventListener('dragover', (e: DragEvent) => {
    e.preventDefault();

    const afterElement = getDragAfterElement(sortableList, e.clientY);
    if (draggedItem && afterElement && afterElement !== draggedItem) {
        sortableList.querySelectorAll('li:not(.dragging)').forEach(item => {
            const li = item as HTMLLIElement;
            li.style.transform = '';
        });

        const draggedIndex = Array.from(sortableList.children).indexOf(draggedItem);
        const afterIndex = Array.from(sortableList.children).indexOf(afterElement);

        if (draggedIndex > afterIndex) {
            afterElement.style.transform = 'translateY(20px)';
        } else {
            afterElement.style.transform = 'translateY(-20px)';
        }
    }
});

sortableList.addEventListener('dragleave', () => {
    sortableList.querySelectorAll('li').forEach(item => {
        const li = item as HTMLLIElement;
        li.style.transform = '';
    });
});

sortableList.addEventListener('drop', (e: DragEvent) => {
    e.preventDefault();

    const afterElement = getDragAfterElement(sortableList, e.clientY);

    if (draggedItem) {
        if (afterElement == null) {
            sortableList.appendChild(draggedItem);
        } else {
            sortableList.insertBefore(draggedItem, afterElement);
        }

        // Reset transforms
        sortableList.querySelectorAll('li').forEach(item => {
            const li = item as HTMLLIElement;
            li.style.transform = '';
        });

        draggedItem.classList.remove('dragging');
        draggedItem = null;
    }
});

