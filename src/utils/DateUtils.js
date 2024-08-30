// src/utils/groceryUtils.js


export function getOffsetFromDate(date) {
    const now = new Date();
    now.setHours(0, 0, 0, 0);
    const targetDate = new Date(date);
    targetDate.setHours(0, 0, 0, 0);
    return Math.floor((now - targetDate) / (1000 * 60 * 60 * 24));
}

export function getLabelForOffset(offset, date) {
    if (offset === 0) return 'Today';
    if (offset === 1) return 'Yesterday';
    return new Date(date).toLocaleDateString();
}

export function organizeGroceriesByOffset(groceries) {
    const now = new Date();
    now.setHours(0, 0, 0, 0);

    return groceries.reduce((acc, grocery) => {
        const offset = getOffsetFromDate(grocery.createdAt);

        if (!acc[offset]) {
            acc[offset] = [];
        }
        acc[offset].push(grocery);
        return acc;
    }, {});

}

export function getOffsets(groceriesByOffset) {
    return Object.keys(groceriesByOffset).map(Number).sort((a, b) => a - b);
}

export function getMinDate(groceries) {
    const minDate = new Date(Math.min(...groceries.map(g => new Date(g.createdAt))));
    minDate.setHours(0, 0, 0, 0);
    return minDate;
}