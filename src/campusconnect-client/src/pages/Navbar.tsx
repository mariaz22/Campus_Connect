import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';

interface User {
  id?: string;
  name?: string;
  profilePictureUrl?: string; 
}
const Navbar = () => {
  const [query, setQuery] = useState('');
  const [category, setCategory] = useState('events');
  const navigate = useNavigate();
  const [user, setUser] = useState<User | null>(null);
  const defaultProfilePic = "https://t4.ftcdn.net/jpg/00/64/67/63/360_F_64676383_LdbmhiNM6Ypzb3FM4PPuFP9rHe7ri8Ju.jpg";
  
  useEffect(() => {
    const user = localStorage.getItem('user'); 
    
    if (user) {
      try {
        setUser(JSON.parse(user)); 
      } catch (error) {
        console.error("Eroare la citirea userului:", error);
      }
    }
  }, []);

  const handleSearch = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!query.trim()) return;
    navigate(`/${category}?search=${encodeURIComponent(query)}`);
  };

  return (
    <nav className="navbar">
      <div className="nav-logo" onClick={() => navigate('/dashboard')}>
        CampusConnect
      </div>
      <form onSubmit={handleSearch} className="search-container">
        <select 
          value={category} 
          onChange={(e) => setCategory(e.target.value)}
          className="search-dropdown"
        >
          <option value="events">Events</option>
          <option value="announcements">Announcements</option>
          <option value="users">Users</option>
          <option value="library">Library</option>
        </select>
        
        <input 
          type="text" 
          placeholder={`Search ${category}...`} 
          value={query}
          onChange={(e) => setQuery(e.target.value)}
          className="search-input"
        />
        <button type="submit" className="search-button">🔍</button>

        <button
          type="button"
          className="profile-button"
          onClick={() => navigate('/profile')}>
          <img 
            src={user?.profilePictureUrl || defaultProfilePic} 
            alt="Profile" 
            className="profile-img"
          />
   </button>
      </form>
      <button 
        onClick={() => navigate(-1)}
        style={{ position: "fixed", top: "90px", left: "15px", padding: "3px 6px", fontSize: "12px", border: "1px solid #ccc", backgroundColor: "#007BFF", color: "#fff", cursor: "pointer", width: "30px", height: "25px", textAlign: "center", fontWeight: "bold", zIndex: 1000 }}
      >
        ←
      </button>
    </nav>

  );
};

export default Navbar;