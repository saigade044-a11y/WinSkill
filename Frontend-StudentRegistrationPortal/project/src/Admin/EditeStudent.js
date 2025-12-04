import React, { useState } from "react";
import { useLocation, useParams, useNavigate } from "react-router-dom";
import './Css/EditStudent1.css';
import NavBar from "./NavBar";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa"; // import icons


const EditeStudent = () => {
    const { id } = useParams();
    const navigate = useNavigate();
    const location = useLocation();

    const token = localStorage.getItem("token");


    const student = location.state?.student;

    const [fullName, setFullName] = useState(student?.fullName || "");
    const [email, setEmail] = useState(student?.email || "");
    const [password, setPassword] = useState(student?.password || "");
    const [phone, setPhone] = useState(student?.phone || "");
    const [address, setAddress] = useState(student?.address || "");
    const [role, setRole] = useState(student?.role || "Student");

    const handleUpdate = async (e) => {
        e.preventDefault();

        if (!student) {
            alert("Student data missing!");
            return;
        }

        const updatedStudent = {
            studentId: student.studentId,
            fullName,
            email,
            password,
            phone,
            address,
            role,
        };

        try {
            const response = await fetch(
                `http://localhost:5049/api/Admin/Update/${student.studentId}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`



                    },
                    body: JSON.stringify(updatedStudent),
                }
            );

            if (!response.ok) {
                const json = await response.json();

                // If backend sends "message"
                if (json.message) {
                    alert(json.message);
                    return;
                }

                // If backend sends "errors" object
                if (json.errors) {
                    const errorMessages = Object.values(json.errors).flat();
                    alert(errorMessages.join("\n"));
                    return;
                }

                alert("Something went wrong!");
                return;
            }



            alert("Student updated successfully!");
            navigate("/AllStudents");

        } catch (err) {
            alert(err.message);
        }
    };

    return (

        <>
            <NavBar />
            <div className="h-container">

                {/* Back arrow on the left */}
                <button onClick={() => navigate("/AllStudents")} className="h92">
                    <FaArrowLeft size={25} />
                </button>

                {/* Form Card */}
                <form onSubmit={handleUpdate} className="h2">
                    {!student && (
                        <h3 className="h-error">No student data received.</h3>
                    )}

                    <label>Full Name</label>
                    <input
                        type="text"
                        value={fullName}
                        onChange={(e) => setFullName(e.target.value)}
                        placeholder="Full Name"
                    />

                    <label>Email</label>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Email"
                    />

                    <label>Password</label>
                    <input
                        type="text"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        placeholder="Password"
                    />

                    <label>Phone</label>
                    <input
                        type="text"
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        placeholder="Phone"
                    />

                    <label>Address</label>
                    <input
                        type="text"
                        value={address}
                        onChange={(e) => setAddress(e.target.value)}
                        placeholder="Address"
                    />

                    <label>Role</label>
                    <select value={role} onChange={(e) => setRole(e.target.value)}>
                        <option value="Student">Student</option>
                    </select>

                    <button type="submit" className="h3">Update Student</button>
                </form>
            </div>

        </>
    );
};

export default EditeStudent;
