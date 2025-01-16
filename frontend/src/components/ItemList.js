import React, { useState, useEffect } from "react";
import "./ItemList.css";

function ItemList() {
  const [items, setItems] = useState([]);
  const [editingItem, setEditingItem] = useState(null);
  const [editTitle, setEditTitle] = useState("");
  const [editDescription, setEditDescription] = useState("");
  const [deletingIds, setDeletingIds] = useState(new Set());

  useEffect(() => {
    fetchItems();
  }, []);

  const fetchItems = async () => {
    try {
      const response = await fetch("http://localhost:5000/api/items", {
        credentials: "include",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
      });
      if (!response.ok) {
        throw new Error("Failed to fetch items");
      }
      const data = await response.json();
      setItems(data);
    } catch (error) {
      console.error("Error fetching items:", error);
    }
  };

  const handleEdit = (item) => {
    console.log("Editing item:", item);
    setEditingItem(item);
    setEditTitle(item.title);
    setEditDescription(item.description);
  };

  const handleUpdate = async () => {
    try {
      const response = await fetch(
        `http://localhost:5000/api/items/${editingItem.id}`,
        {
          method: "PUT",
          credentials: "include",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            id: editingItem.id,
            title: editTitle,
            description: editDescription,
          }),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        console.error("Update failed:", errorData);
        throw new Error(errorData.message || "Failed to update item");
      }

      const updatedItem = await response.json();
      console.log("Item updated successfully:", updatedItem);
      setEditingItem(null);
      fetchItems();
    } catch (error) {
      console.error("Error updating item:", error);
      alert("Failed to update item: " + error.message);
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm("Are you sure you want to delete this item?")) {
      try {
        setDeletingIds((prev) => new Set(prev).add(id));
        const response = await fetch(`http://localhost:5000/api/items/${id}`, {
          method: "DELETE",
          credentials: "include",
          headers: {
            Accept: "application/json",
            "Content-Type": "application/json",
          },
        });

        if (!response.ok) {
          const errorData = await response.json();
          throw new Error(errorData.message || "Failed to delete item");
        }

        setItems((prevItems) => prevItems.filter((item) => item.id !== id));
      } catch (error) {
        console.error("Error deleting item:", error);
        alert("Failed to delete item: " + error.message);
        fetchItems();
      } finally {
        setDeletingIds((prev) => {
          const newSet = new Set(prev);
          newSet.delete(id);
          return newSet;
        });
      }
    }
  };

  return (
    <div className="item-list">
      <h2 className="item-list__title">Items</h2>
      <div className="item-list__container">
        {items.map((item) => (
          <div key={item.id} className="item-list__item">
            {editingItem?.id === item.id ? (
              <div className="item-list__edit-form">
                <input
                  type="text"
                  className="item-list__edit-input"
                  value={editTitle}
                  onChange={(e) => setEditTitle(e.target.value)}
                  maxLength={100}
                  required
                />
                <textarea
                  className="item-list__edit-textarea"
                  value={editDescription}
                  onChange={(e) => setEditDescription(e.target.value)}
                  maxLength={500}
                />
                {editTitle.length === 0 && (
                  <div className="item-list__error">Title is required</div>
                )}
                <div className="item-list__edit-buttons">
                  <button
                    onClick={handleUpdate}
                    className="item-list__save-button"
                  >
                    Save
                  </button>
                  <button
                    onClick={() => setEditingItem(null)}
                    className="item-list__cancel-button"
                  >
                    Cancel
                  </button>
                </div>
              </div>
            ) : (
              <>
                <h3 className="item-list__item-title">{item.title}</h3>
                <p className="item-list__item-description">
                  {item.description}
                </p>
                <div className="item-list__buttons">
                  <button
                    onClick={() => handleEdit(item)}
                    className="item-list__edit-button"
                  >
                    Edit
                  </button>
                  <button
                    onClick={() => handleDelete(item.id)}
                    className="item-list__delete-button"
                    disabled={deletingIds.has(item.id)}
                  >
                    {deletingIds.has(item.id) ? "Deleting..." : "Delete"}
                  </button>
                </div>
              </>
            )}
          </div>
        ))}
      </div>
    </div>
  );
}

export default ItemList;
