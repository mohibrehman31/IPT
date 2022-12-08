import "./App.css";
import { useEffect, useState } from "react";
import axios from "axios";
import Dropdown from "react-bootstrap/Dropdown";
import { DropdownButton, Container, Col, Row, Table } from "react-bootstrap";

const Main = () => {
  const [Students, setStudents] = useState([]);
  const [courses, setCourses] = useState([]);
  const [Teacher, setTeacher] = useState("");
  const setData = (value) => {
    console.log(value);
    setTeacher(value);
  };
  useEffect(() => {
    const getData = async () => {
      try {
        axios
          .get("https://localhost:44323/api/GetStudents")
          .then((responce) => {
            setStudents(responce.data);
            console.log(responce.data);
          });
      } catch (error) {
        console.log(error);
      }

      try {
        axios.get("https://localhost:44323/api/GetCourses").then((responce) => {
          setCourses(responce.data);
          console.log(responce.data);
        });
      } catch (error) {
        console.log(error);
      }
    };

    getData();
  }, []);

  return (
    <Container style={{ backgroundColor: "" }}>
      <Row>
        <Col>
          <DropdownButton id="dropdown-basic-button" title="Teacher Name">
            <Dropdown.Item onSelect={setData}>Action</Dropdown.Item>
            <Dropdown.Item>Another action</Dropdown.Item>
            <Dropdown.Item>Something else</Dropdown.Item>
          </DropdownButton>
        </Col>
        <Col>
          <DropdownButton id="dropdown-basic-button" title="Course Code">
            {courses.map((cour) => {
              return (
                <Dropdown.Item key={cour} href="#/action-3">
                  {cour}
                </Dropdown.Item>
              );
            })}
          </DropdownButton>
        </Col>
        <Col>
          <DropdownButton id="dropdown-basic-button" title="Date">
            <Dropdown.Item href="#/action-1">Action</Dropdown.Item>
            <Dropdown.Item href="#/action-2">Another action</Dropdown.Item>
            <Dropdown.Item href="#/action-3">Something else</Dropdown.Item>
          </DropdownButton>
        </Col>
        <Col>
          <DropdownButton id="dropdown-basic-button" title="Section">
            <Dropdown.Item>Action</Dropdown.Item>
            <Dropdown.Item href="#/action-2">Another action</Dropdown.Item>
            <Dropdown.Item href="#/action-3">Something else</Dropdown.Item>
          </DropdownButton>
        </Col>
      </Row>

      <Row>
        <Table striped bordered hover variant="dark">
          <thead>
            <tr>
              <th>Roll Number</th>
              <th>Student Name</th>
              <th>Attendance</th>
            </tr>
          </thead>

          {Students.map((attend) => {
            return (
              <tbody>
                <tr>
                  <td>{attend[0]}</td>
                  <td>{attend[1]}</td>
                  <td>
                    <DropdownButton id="dropdown-basic-button" title="-">
                      <Dropdown.Item>P</Dropdown.Item>
                      <Dropdown.Item>A</Dropdown.Item>
                      <Dropdown.Item>L</Dropdown.Item>
                    </DropdownButton>
                  </td>
                </tr>
              </tbody>
            );
          })}
        </Table>
      </Row>
    </Container>
  );
};

export default Main;
