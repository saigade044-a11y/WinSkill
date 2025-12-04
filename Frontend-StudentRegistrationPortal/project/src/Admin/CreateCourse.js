import React, { useState } from "react";
import NavBar from "./NavBar";
import AllCourses from "./AllCourses";
import { useNavigate } from "react-router-dom";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa"; // import icons
import '../index.css'; // <-- ADD THIS LINE
const CreateCourse = () => {
    const [courseName, setCourseName] = useState("");
    const [courseDescription, setCourseDescription] = useState("");
    const [capacity, setCapacity] = useState("");
    const token = localStorage.getItem("token");

    const navigate = useNavigate();


    const handleSubmit = async (e) => {
        e.preventDefault();

        const data = {
            courseName,
            courseDescription,
            capacity: Number(capacity)
        };

        try {
            const response = await fetch(
                "http://localhost:5049/api/Course/CreateCourse",
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


            const result = await response.json();
            alert("Course created successfully! ID = " + result.course.courseId);
            setCourseName('');
            setCourseDescription('');
            setCapacity('');
            navigate("/SideBar");

        } catch (err) {
            alert(err.message);
        }
    };

    return (
        <>
            <NavBar />

            <div className="v93">

            <button onClick={() => navigate("/SideBar")} className="v92">
                <FaArrowLeft size={20} />
            </button>

            <div className="v1">

                <h2 className="v2">Create Course</h2>

                <form onSubmit={handleSubmit} className="v3">
                    <label>Course Name</label>
                    <input
                        type="text"
                        placeholder="Enter The Course Name"
                        value={courseName}
                        onChange={(e) => setCourseName(e.target.value)}
                    />

                    <label>Course Description</label>
                    <textarea
                        placeholder="Enter The Course Description"
                        value={courseDescription}
                        onChange={(e) => setCourseDescription(e.target.value)}
                    />

                    <label>Capacity</label>
                    <input
                        type="number"
                        placeholder="Enter The Capacity"
                        value={capacity}
                        onChange={(e) => setCapacity(e.target.value)}
                    />

                    <button className="v4" type="submit" >Create Course</button>
                </form>
            </div>
            </div>
        </>
    );
};

export default CreateCourse;
