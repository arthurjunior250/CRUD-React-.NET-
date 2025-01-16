import React, { useState } from "react";
import "./ItemForm.css";

function ItemForm() {
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const response = await fetch("http://localhost:5000/api/items", {
        method: "POST",
        credentials: "include",
        headers: {
          Accept: "application/json",
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          title,
          description,
        }),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Failed to create item");
      }

      setTitle("");
      setDescription("");
      window.location.reload();
    } catch (error) {
      console.error("Error creating item:", error);
    }
  };

  return (
    <form className="item-form" onSubmit={handleSubmit}>
      <h2 className="item-form__title">Add New Item</h2>
      <div className="item-form__field">
        <label className="item-form__label">Title:</label>
        <input
          type="text"
          className="item-form__input"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          required
        />
      </div>
      <div className="item-form__field">
        <label className="item-form__label">Description:</label>
        <textarea
          className="item-form__textarea"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          required
        />
      </div>
      <button type="submit" className="item-form__submit">
        Add Item
      </button>
    </form>
  );
}

export default ItemForm;
