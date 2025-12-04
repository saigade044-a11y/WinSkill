import { Navigate } from "react-router-dom";
import '../index.css'; // <-- ADD THIS LINE 
export default function ProtectedRoute({ children }) {

  const token = localStorage.getItem("token");
 
  if (!token) {
    

    return <Navigate to="/" replace />;

  }
 
  return children;
}
