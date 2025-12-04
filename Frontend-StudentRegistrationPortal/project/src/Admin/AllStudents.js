import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useNavigate } from "react-router-dom";
import NavBar from "./NavBar";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa"; // import icons
import '../index.css'; // <-- ADD THIS LINE

const AllStudents = () => {


    const [students, setStudents] = useState([]);

    const [loading, setLoading] = useState(true);

    const [error, setError] = useState(null);

    const [search, setSearch] = useState("");

    const token = localStorage.getItem("token");



    useEffect(() => {
        const handleFetch = async () => {
            try {
                setLoading(true);

                const response = await fetch(
                    "http://localhost:5049/api/Admin/AllStudents",
                    {
                        method: "GET",
                        headers: {
                            "Content-Type": "application/json",
                            Authorization: `Bearer ${token}`
                        }
                    }


                );

                if (!response.ok) {
                    throw new Error("We are facing an error while fetching student data.");
                }

                const data = await response.json();
                setStudents(data);
            } catch (err) {
                setError(err.message);
                alert(err.message);
            } finally {
                setLoading(false);
            }
        };

        handleFetch();
    }, []);



    //based on search
    const filteredStudents = students.filter((item) =>
        item.fullName.toLowerCase().includes(search.toLowerCase())
    );



    //delete based on id
    const handleDelete = async (id) => {
        const confirmDelete = window.confirm(
            "Are you sure you want to delete this student?"
        );
        if (!confirmDelete) {
            return;
        }

        try {
            const response = await fetch(
                `http://localhost:5049/api/Admin/Delete/${id}`,
                {
                    method: "DELETE",
                    headers: {
                        Authorization: `Bearer ${token}`


                    }
                }
            );

            if (!response.ok) throw new Error("Failed to delete student");

            alert("Student deleted successfully!");

            window.location.reload();


        } catch (err) {
            alert(err.message);
        }
    };


    const navigate=useNavigate();



    return (
        <>
            <NavBar />
            <div className="s2">
                <div className="s29">
                    <form className="s3">
                        <button onClick={() => navigate("/SideBar")} className="nav-btn">
                            <FaArrowLeft size={20} />
                        </button>

                        <input
                            type="text"
                            autoFocus
                            placeholder="Enter The Student Name"
                            value={search}
                            onChange={(e) => setSearch(e.target.value)}
                        />
                    </form>
                </div>

                <h2>Student Details</h2>


                {loading && <h3>Loading…</h3>}
                {error && <h3 style={{ color: "red" }}>{error}</h3>}

                {!loading && !error && (
                    <h3>Total Students: {filteredStudents.length}</h3>
                )}

                <div className="s1">
                    <table border="1" cellPadding="8" className="s4">
                        <thead className="s5">
                            <tr>
                                <th>StudentId</th>
                                <th>FullName</th>
                                <th>Email</th>
                                <th>Password</th>
                                <th>Phone</th>
                                <th>Address</th>
                                <th>Role</th>
                                <th>DateTime</th>
                                <th>Update</th>
                                <th>Delete</th>
                            </tr>
                        </thead>

                        <tbody className="s6">
                            {filteredStudents.map((item) => (
                                <tr key={item.studentId}>
                                    <td>{item.studentId}</td>
                                    <td>{item.fullName}</td>
                                    <td>{item.email}</td>
                                    <td>{item.password}</td>
                                    <td>{item.phone}</td>
                                    <td>{item.address}</td>
                                    <td>{item.role}</td>
                                    <td>{item.datetime}</td>
                                    <td>
                                        <Link
                                            to={`/EditeStudent/${item.studentId}`}
                                            state={{ student: item }}
                                        >
                                            <button>Edit</button></Link>
                                    </td>
                                    <td>
                                        <button onClick={() => handleDelete(item.studentId)}>
                                            Delete
                                        </button>
                                    </td>
                                </tr>
                            ))}
                        </tbody>
                    </table>
                </div>
            </div>
        </>
    );
};

export default AllStudents;
