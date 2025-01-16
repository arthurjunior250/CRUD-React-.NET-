const ITEMS_KEY = "items";

export const storageService = {
  getItems: () => {
    const items = localStorage.getItem(ITEMS_KEY);
    return items ? JSON.parse(items) : [];
  },

  addItem: (item) => {
    const items = storageService.getItems();
    const newItem = {
      ...item,
      id: Date.now(), // Use timestamp as ID
      createdAt: new Date().toISOString(),
    };
    items.push(newItem);
    localStorage.setItem(ITEMS_KEY, JSON.stringify(items));
    return newItem;
  },

  updateItem: (id, updatedItem) => {
    const items = storageService.getItems();
    const index = items.findIndex((item) => item.id === id);
    if (index !== -1) {
      items[index] = { ...items[index], ...updatedItem };
      localStorage.setItem(ITEMS_KEY, JSON.stringify(items));
      return items[index];
    }
    return null;
  },

  deleteItem: (id) => {
    const items = storageService.getItems();
    const filteredItems = items.filter((item) => item.id !== id);
    localStorage.setItem(ITEMS_KEY, JSON.stringify(filteredItems));
  },
};
