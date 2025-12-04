import { useEffect, useState } from "react";
import Navbar from "./NavBar";
import '../index.css'; // <-- ADD THIS LINE
const StudentHome = () => {
    const [courses, setCourses] = useState([]);
    const[search,setSearch]=useState("");
    const token = localStorage.getItem("token");
    const studentId = localStorage.getItem("studentId");





    // Fetch all courses
    useEffect(() => {
        const fetchCourses = async () => {

            const response = await fetch("http://localhost:5049/api/Course/GetAllCourses", {

                headers: {
                    Authorization: `Bearer ${token}`
                },
            });
            const data = await response.json();
            setCourses(data);
        };
        fetchCourses();
    }, [token]);

    // Fetch student enrollments




    const handleEnroll = async (courseId) => {
        try {
            const response = await fetch("http://localhost:5049/api/Student/EnrollCourse", {
                method: "POST",
                headers: { "Content-Type": "application/json", Authorization: `Bearer ${token}` },
                body: JSON.stringify({ StudentId1: studentId, CourseId1: courseId }),
            });

            if (response.ok) {
                alert("Enrolled successfully!");
                window.location.reload();
            }

            else {

                const err = await response.json();
                alert(err.message);
            }
        } catch (err) {
            console.error(err);
            alert("Failed to enroll");
        }
    };

        const filteredCourses = courses
        .filter(course => course.availableSeats > 0) // only courses with seats available
        .filter(course =>
            course.courseName.toLowerCase().includes(search.toLowerCase())
        );





    return (
        <div>
            <Navbar />
            <hr />

            <h2 className="pu1">Student Dashboard</h2>

            <div className="pu2">
                <h2>All Courses</h2>

                {/* Search Section (Separate) */}
                <div className="pu3">
                    <input
                        type="text"
                        placeholder="Search course..."
                        value={search}
                        onChange={(e) => setSearch(e.target.value)}
                       
                    />
                </div>

                {/* Table */}
                <table border="1"className="pu4">
                    <thead className="pu5">
                        <tr>
                            <th>Course ID</th>
                            <th>Course Name</th>
                            <th>Description</th>
                            <th>Available Seats</th>
                            <th>Action</th>
                        </tr>
                    </thead>

                    <tbody className="pu6">
                        {filteredCourses.map((course) => (
                            <tr key={course.courseId}>
                                <td>{course.courseId}</td>
                                <td>{course.courseName}</td>
                                <td>{course.courseDescription}</td>
                                <td>{course.availableSeats}</td>
                                <td>
                                    <button
                                        onClick={() => handleEnroll(course.courseId)}
                                     
                                    >
                                        Enroll
                                    </button>
                                </td>
                            </tr>
                        ))}
                    </tbody>
                </table>

            </div>
        </div>

    );
};

export default StudentHome;
