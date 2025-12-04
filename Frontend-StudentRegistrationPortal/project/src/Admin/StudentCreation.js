
import React, { useState } from 'react';

import './Css/CreateStudent.css';
import NavBar from './NavBar';
import AllCourses from './AllCourses';
import AllStudents from './AllStudents';

const StudentCreation = () => {

    const [fullName, setFullName] = useState("");
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [phone, setPhone] = useState("");
    const [address, setAddress] = useState("");
    const [role, setRole] = useState("Student");
    const token = localStorage.getItem("token");


    const handleSubmit = async (e) => {
        e.preventDefault();

        const data = {
            fullName,
            email,
            password,
            phone,
            address,
            role
        };

        try {
            const response = await fetch(
                "http://localhost:5049/api/Admin/StudentRegister",
                {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`

                    },
                    body: JSON.stringify(data),
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


            alert("Student Registered Successfully!");

            // Clear form
            setFullName("");
            setEmail("");
            setPassword("");
            setPhone("");
            setAddress("");
            setRole("Student");

        } catch (err) {
            alert(err.message);
        }
    };

    return (
        <>
            <NavBar />
            <div className='p11'>
                <form onSubmit={handleSubmit} className='p12'>
                    <h2>Student Registration Page</h2>

                    <label>Full Name</label><br />
                    <input
                        type="text"
                        value={fullName}
                        onChange={(e) => setFullName(e.target.value)}
                        required
                    /><br /><br />

                    <label>Email</label><br />
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        required
                    /><br /><br />

                    <label>Password</label><br />
                    <input
                        type="password"
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        required
                    /><br /><br />

                    <label>Phone</label><br />
                    <input
                        type="text"
                        value={phone}
                        onChange={(e) => setPhone(e.target.value)}
                        required
                        minLength={10}
                    /><br /><br />

                    <label>Address</label><br />
                    <input
                        type="text"
                        value={address}
                        onChange={(e) => setAddress(e.target.value)}
                        required
                    /><br /><br />

                    <label>Role</label><br />
                    <select
                        value={role}
                        onChange={(e) => setRole(e.target.value)}
                    >
                        <option value="Student">Student</option>
                    </select><br /><br />

                    <button type="submit" className='p13'>Register Student</button>
                </form>
            </div>
        </>
    );
};

export default StudentCreation;
