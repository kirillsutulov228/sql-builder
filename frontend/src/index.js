import React from 'react';
import ReactDOM from 'react-dom/client';
import './index.scss';
import Builder from './Builder';
import reportWebVitals from './reportWebVitals';
import { BlockDataProvider } from './store/blockDataContext';
import { SelectContextProvider } from './store/selectContext';
import { Route, Routes } from 'react-router';
import IndexLayout from './components/IndexLayout/IndexLayout';
import App from './App';
import { BrowserRouter } from 'react-router-dom';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <React.StrictMode>
    <BrowserRouter>
      <BlockDataProvider>
        <SelectContextProvider>
          <App />
          {/* <Builder /> */}
        </SelectContextProvider>
      </BlockDataProvider>
    </BrowserRouter>
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
