import {Routes, Route} from 'react-router-dom'
import './App.css'

import Header from "~/components/Header"

import Generator from "~/pages/Generator"
import Scanner from "~/pages/Scanner"

function App() {
  return (
    <>
      <Header />
      <Routes>
        <Route path="/" element={<Generator />} />
        <Route path="/generator" element={<Generator />} />
        <Route path="/scanner" element={<Scanner />} />
      </Routes>
    </>
  )
}

export default App;