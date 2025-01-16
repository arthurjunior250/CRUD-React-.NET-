import React from "react";
import ItemList from "./components/ItemList";
import ItemForm from "./components/ItemForm";
import "./App.css";

function App() {
  return (
    <div className="app">
      <header className="app__header">
        <h1 className="app__title">CRUD Application</h1>
      </header>
      <main className="app__main">
        <ItemForm />
        <ItemList />
      </main>
    </div>
  );
}

export default App;
