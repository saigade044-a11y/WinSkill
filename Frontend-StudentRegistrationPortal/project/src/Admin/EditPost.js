import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import SideBar from "./SideBar";
import NavBar from "./NavBar";
import { Navigate } from "react-router-dom";
import { FaArrowLeft, FaArrowRight } from "react-icons/fa"; // import icons

import '../index.css'; // <-- ADD THIS LINE

const EditPost = () => {
    const { id } = useParams();

    const [courseName, setCourseName] = useState("");
    const [courseDescription, setCourseDescription] = useState("");
    const [capacity, setCapacity] = useState("");

    const [loading, setLoading] = useState(true);

    const token = localStorage.getItem("token");

    const navigate = useNavigate();


    useEffect(() => {


        const fetchCourse = async () => {

            try {

                const response = await fetch(`http://localhost:5049/api/Course/GetCourseById/${id}`, {
                    headers: {
                        Authorization: `Bearer ${token}`
                    }

                }

                );



                if (!response.ok) {
                    throw new Error("Failed to fetch course data");

                }

                const data = await response.json();

                // Pre-fill form fields
                setCourseName(data.course.courseName);
                setCourseDescription(data.course.courseDescription);
                setCapacity(data.course.capacity);

            } catch (error) {
                alert(error);
            } finally {
                setLoading(false);
            }
        };

        fetchCourse();
    }, [id]);

    // Handle Submit
    const handleSubmit = async (e) => {
        e.preventDefault();

        const updatedCourse = {
            courseId: id,
            courseName,
            courseDescription,
            capacity: Number(capacity),
        };

        try {
            const response = await fetch(
                `http://localhost:5049/api/Course/UpdateCourseById/${id}`,
                {
                    method: "PUT",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`

                    },
                    body: JSON.stringify(updatedCourse),
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

            alert("Course updated successfully!");
            setCourseDescription('');
            setCapacity('');
            setCourseName('');
            navigate(`/ViewSingleCourse/${id}`);

        } catch (error) {
            alert(error.message);
        }
    };

    if (loading) return <p>Loading Course Details...</p>;

    return (
        <>
            <NavBar />

            <div className="m92">

                <button onClick={() => navigate(`/ViewSingleCourse/${id}`)} className="m98">
                    <FaArrowLeft size={20} />
                </button>


                <div className="m1">
                    <h2 className="m2">Edit Course</h2>


                    <form onSubmit={handleSubmit} className="m3">

                        <div className="m4">
                            <label>Course Name:</label><br />
                            <input
                                type="text"
                                value={courseName} x
                                onChange={(e) => setCourseName(e.target.value)}
                                required
                                className="m4"

                            />
                        </div>

                        <br />

                        <div className="m5">
                            <label>Course Description:</label><br />
                            <textarea
                                value={courseDescription}
                                onChange={(e) => setCourseDescription(e.target.value)}
                                rows="4"
                                required

                            ></textarea>
                        </div>

                        <br />

                        <div className="m6">
                            <label>Capacity:</label><br />
                            <input
                                type="number"
                                value={capacity}
                                onChange={(e) => setCapacity(e.target.value)}
                                required
                            />
                        </div>

                        <br />

                        <button className="m7" type="submit" >Update Course</button>
                    </form>
                </div >
            </div>
        </>
    );
};

export default EditPost;
