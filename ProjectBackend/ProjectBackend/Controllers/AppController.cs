using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using ProjectBackend.Models;
using System.Web.UI.WebControls;
using System.Runtime.Remoting.Messaging;
using System.Xml.Linq;
using System.Collections;
using static System.Collections.Specialized.BitVector32;
using System.Reflection;

namespace ProjectBackend.Controllers
{
     [RoutePrefix("api")]

     public class AppController : ApiController
     {
          SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["conn"].ConnectionString);
          SqlCommand cmd = new SqlCommand();
          SqlDataAdapter da = new SqlDataAdapter();
          SqlDataReader dataReader;

          static string role = " ";
          static admins AData;
          static teacher TData;


          [HttpPost]
          [Route("RegisterStudent")]
          public string RegisterStudent(registerStudent regisdata)
          {
               string response = String.Empty;
               string subjects = String.Empty;


               foreach(string sub in regisdata.courses)
               {
                    subjects = sub +","+ subjects;
               }
               try
               {
                    string commandText = "insert into registrations values(@name,@number,@gender,@mode,@courses,@hrs,@fees);";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@name", regisdata.fullName);
                    cmd.Parameters.Add("@number", regisdata.phoneNumber);
                    cmd.Parameters.Add("@gender", regisdata.gender);
                    cmd.Parameters.Add("@mode", regisdata.mode);
                    cmd.Parameters.Add("@courses", subjects);
                    cmd.Parameters.Add("@hrs", regisdata.hours);
                    cmd.Parameters.Add("@fees", regisdata.expFees);

                    conn.Open();
                    int r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {
                         response = "Success";
                    }
                    else
                    {
                         response = "Faield";
                    }
               }

               catch(Exception e)
               {
                    response = "Failed";
               }

               return response;
          }

          [HttpPost]
          [Route("SignUp")]
          public string SignUp(teacher teacherData)
          {
               
               string resp = String.Empty;

               try
               {
                    string commandText = "select firstName from teachers where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", teacherData.email);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    commandText = "select firstName from admins where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", teacherData.email);

                    DataTable dt2 = new DataTable();
                    da.Fill(dt2);

                    if (dt.Rows.Count == 0 && dt2.Rows.Count == 0)
                    {
                         try
                         {
                              commandText = "insert into teachers values(@fname,@lname,@email,@phone,@subject,@password);";
                              cmd = new SqlCommand(commandText, conn);
                              cmd.Parameters.Add("@fname", teacherData.firstName);
                              cmd.Parameters.Add("@lname", teacherData.lastName);
                              cmd.Parameters.Add("@phone", teacherData.phoneNumber);
                              cmd.Parameters.Add("@email", teacherData.email);
                              cmd.Parameters.Add("@password", teacherData.password);
                              cmd.Parameters.Add("@subject", teacherData.subjects);
   
                              conn.Open();
                              int r = cmd.ExecuteNonQuery();
                              conn.Close();

                              if (r > 0)
                              {
                                   resp = "Success";
                              }
                              else
                              {
                                   resp = "Failed";
                              }
                         }

                         catch(Exception e)
                         {
                              resp = "Failed";
                         }


                    }
                    else
                    {
                         resp = "Failed";
                    }


               }

               catch (Exception e)
               {
                    resp = "Failed";
              
               }
         

               return resp;
          }

          [HttpPost]
          [Route("Login")]
          public string Login(teacher teacherData)
          {

               string resp = String.Empty;

               try
               {
                    string commandText = "select * from admins where email = @email and password = @password;";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@email", teacherData.email);
                    cmd.Parameters.Add("@password", teacherData.password);

                    conn.Open();

                    dataReader = cmd.ExecuteReader();

                    AData = new admins();
                    TData = new teacher();

                    while (dataReader.Read())
                    {
                         role = "admin";
                         

                         AData.id = dataReader.GetValue(0).ToString();
                         AData.firstName = dataReader.GetValue(1).ToString();
                         AData.lastName = dataReader.GetValue(2).ToString();
                         AData.email = dataReader.GetValue(3).ToString();
                         AData.password = dataReader.GetValue(4).ToString();
                         AData.phoneNumber = dataReader.GetValue(5).ToString();

                         return "Success";
                    }

                    conn.Close();


                    commandText = "select * from teachers where email = @email and password = @password;";
                    cmd = new SqlCommand(commandText, conn);
                    cmd.Parameters.Add("@email", teacherData.email);
                    cmd.Parameters.Add("@password", teacherData.password);

                    conn.Open();
                    dataReader = cmd.ExecuteReader();
           

                    while (dataReader.Read())
                    {

                         role = "teacher";
                         

                         TData.id = dataReader.GetValue(0).ToString();
                         TData.firstName = dataReader.GetValue(1).ToString();
                         TData.lastName = dataReader.GetValue(2).ToString();
                         TData.email = dataReader.GetValue(3).ToString();
                         TData.phoneNumber = dataReader.GetValue(4).ToString();
                         TData.subjects = dataReader.GetValue(5).ToString();
                         TData.password = dataReader.GetValue(6).ToString();
                         

                         return "Success";
                       
                    }

                    conn.Close();


                    resp = "Failed";
                    
               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("Logout")]
          public string Logout(Response respo)
          {

               string resp = String.Empty;
               
               if (role == "admin")
               {
                    role = " ";
                    resp = "Success";
               }
               else if (role == "teacher")
               {
                    role = " ";
                    resp = "Success";
               }

               else
               {
                    resp = "Failed";
               }


               return resp;
          }

          [HttpPost]
          [Route("UpdatePersonalInfo")]
          public string UpdatePersonalInfo(teacher teacherData)
          {
               string commandText = String.Empty;
               string resp = String.Empty;

               try
               {
                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET firstName = @fname, lastName = @lname, phoneNumber = @num WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);
                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET firstName = @fname, lastName = @lname, phoneNumber = @num, subjects = @subj WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);
                         cmd.Parameters.Add("@subj", teacherData.subjects);
                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@fname", teacherData.firstName);
                    cmd.Parameters.Add("@lname", teacherData.lastName);
                    cmd.Parameters.Add("@num", teacherData.phoneNumber);
                    

                    conn.Open();
                    int r = cmd.ExecuteNonQuery();
                    conn.Close();


                    if (r > 0)
                    {
                         if (role == "admin")
                         {
                              AData.firstName = teacherData.firstName;
                              AData.lastName = teacherData.lastName;
                              AData.phoneNumber = teacherData.phoneNumber;
                         }
                         else
                         {
                              TData.firstName = teacherData.firstName;
                              TData.lastName = teacherData.lastName;
                              TData.phoneNumber = teacherData.phoneNumber;
                              TData.subjects = teacherData.subjects;
                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("UpdateAccountInfo")]
          public string UpdateAccountInfo(admins adminData)
          {

               string resp = String.Empty;
               string commandText = String.Empty;
               int r = 0;

               try
               {
                    commandText = "select firstName from teachers where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", adminData.email);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    commandText = "select firstName from admins where email = @email;";
                    da = new SqlDataAdapter(commandText, conn);
                    da.SelectCommand.Parameters.Add("@email", adminData.email);

                    DataTable dt2 = new DataTable();
                    da.Fill(dt2);

                    if (dt.Rows.Count != 0 || dt2.Rows.Count != 0)
                    {
                         return "Taken";

                    }
               }

               catch (Exception e)
               {
                    return "Failed";
               }




               try
               {
                    
                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET email = @email, password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);

                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET email = @email, password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);

                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@email", adminData.email);
                    cmd.Parameters.Add("@password", adminData.password);
                    

                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {

                         if (role == "admin")
                         {
                              AData.email = adminData.email;
                              AData.password = adminData.password;

                         }

                        else
                         {
                              TData.email = adminData.email;
                              TData.password = adminData.password;

                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }


          [HttpPost]
          [Route("UpdatePassword")]
          public string UpdatePassword(admins adminData)
          {

               string resp = String.Empty;
               string commandText = String.Empty;
               int r = 0;


               try
               {

                    if (role == "admin")
                    {
                         commandText = "UPDATE admins SET password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", AData.id);

                    }

                    else if (role == "teacher")
                    {
                         commandText = "UPDATE teachers SET password = @password WHERE id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", TData.id);

                    }
                    else
                    {
                         return "Failed";
                    }

                    cmd.Parameters.Add("@password", adminData.password);


                    conn.Open();
                    r = cmd.ExecuteNonQuery();
                    conn.Close();

                    if (r > 0)
                    {

                         if (role == "admin")
                         {
                              AData.password = adminData.password;

                         }

                         else
                         {
                              TData.password = adminData.password;

                         }

                         resp = "Success";
                    }

                    else
                    {
                         resp = "Failed";
                    }

               }

               catch (Exception e)
               {
                    resp = "Failed";

               }


               return resp;
          }

          [HttpPost]
          [Route("RemoveTeacher")]
          public string RemoveTeacher(Response resp)
          {
               string res = string.Empty;

               try
               {

                    for (int i = 0; i < resp.ids.Count; i++)
                    {
                         string commandText = "delete from teachers where id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", resp.ids[i].ToString());

                         conn.Open();
                         int r = cmd.ExecuteNonQuery();
                         conn.Close();

                         if (r > 0)
                         {
                              res = "Success";
                         }

                         else
                         {
                              return "Failed";
                         }


                    }
               }
               catch(Exception e)
               {
                    res = "Failed";
               }

               return res;

          }

          [HttpPost]
          [Route("RemoveRegistration")]
          public string RemoveRegistration(Response resp)
          {
               string res = string.Empty;

               try
               {

                    for (int i = 0; i < resp.ids.Count; i++)
                    {
                         string commandText = "delete from registrations where id = @id;";
                         cmd = new SqlCommand(commandText, conn);
                         cmd.Parameters.Add("@id", resp.ids[i].ToString());

                         conn.Open();
                         int r = cmd.ExecuteNonQuery();
                         conn.Close();

                         if (r > 0)
                         {
                              res = "Success";
                         }

                         else
                         {
                              return "Failed at updating";
                         }


                    }
               }
               catch (Exception e)
               {
                    res = "Failed";
               }

               return res;

          }


          [HttpGet]
          [Route("GetCourses")]
          public ArrayList GetCourses()
          {
               ArrayList regis = new ArrayList();
               ArrayList rw;

               try
               {

                    string commandText = "select Course_Name from Courses;";
                    da = new SqlDataAdapter(commandText, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);
            

                    foreach (DataRow row in dt.Rows)
                    {    
                         
                         string id = row["Course_Name"].ToString();

                         regis.Add(id);

                         //regis.Add(rw);
                    }
               }

               catch(Exception e)
               {
                    Console.WriteLine(e);
               }

               return regis;
          }

          [HttpGet]
          [Route("GetAttendance")]
          public ArrayList GetAttendance()
          {
               ArrayList regis = new ArrayList();
               ArrayList rw;

               try
               {

                    string commandText = "select Attendance.ID,Attendance.Roll_no,Student.Name,Attendance.Attendance from dbo.Attendance inner join dbo.Student on Attendance.Roll_no = Student.Roll;";
                    da = new SqlDataAdapter(commandText, conn);

                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    foreach (DataRow row in dt.Rows)
                    {
                         rw = new ArrayList();
                         string id = row["ID"].ToString();
                         string fullName = row["Roll_no"].ToString();
                         string phoneNumber = row["Name"].ToString();
                         string hours = row["Attendance"].ToString();
              

                         rw.Add(id);
                         rw.Add(fullName);
                         rw.Add(phoneNumber);
                         rw.Add(hours);
             

                         regis.Add(rw);
                    }
               }

               catch (Exception e)
               {
                    Console.WriteLine(e);
               }

               return regis;
          }

        [HttpGet]
        [Route("GetStudents")]
        public ArrayList GetStudents()
        {
            ArrayList regis = new ArrayList();
            ArrayList rw;

            try
            {

                string commandText = "select Student.Roll,Student.Name from Student  Inner Join  Teacher on Teacher.Section = Student.Section  INNER JOIN  Courses on Teacher.Course_ID=Courses.Course_Code  where Courses.Course_Name = 'IPT' and Teacher.Name='Murtaza' and Student.Section='7G'";
                da = new SqlDataAdapter(commandText, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    rw = new ArrayList();
                    string id = row["Roll"].ToString();
                    string fullName = row["Name"].ToString();

                    rw.Add(id);
                    rw.Add(fullName);

                    regis.Add(rw);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return regis;
        }

        [HttpGet]
        [Route("GetTeaches")]
        public ArrayList GetTeaches()
        {
            ArrayList regis = new ArrayList();
            ArrayList rw;

            try
            {

                string commandText = "select Course_Code from Teaches where Teacher_Teacherid='CS1313'";
                da = new SqlDataAdapter(commandText, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    rw = new ArrayList();
                    string Course_Code = row["Course_Code"].ToString();

                    rw.Add(Course_Code);
                    regis.Add(rw);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return regis;
        }

        [HttpGet]
        [Route("GetSections")]
        public ArrayList GetSections()
        {
            ArrayList regis = new ArrayList();
            ArrayList rw;

            try
            {

                string commandText = "select section from Teaches where Teacher_Teacherid='CS1313' and Course_Code='CS3002'";
                da = new SqlDataAdapter(commandText, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    rw = new ArrayList();
                    string sections = row["section"].ToString();

                    rw.Add(sections);
                    regis.Add(rw);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return regis;
        }
        //select Student.Rollno, Student.Name, StudentAttendance.Att from Studies Inner Join Student on Student.Rollno= Studies.Student_Rollno inner join StudentAttendance on StudentAttendance.Studies_Student_Rollno= Student.Rollno where Studies.Course_Code= 'CS3002' and Studies.section = '7G' and StudentAttendance.Date= '2022-12-08'

        [HttpGet]
        [Route("GetAttendances")]
        public ArrayList GetAttendances()
        {
            ArrayList regis = new ArrayList();
            ArrayList rw;

            try
            {

                string commandText = "select Student.Rollno,Student.Name,StudentAttendance.Att from Studies Inner Join Student on Student.Rollno=Studies.Student_Rollno inner join StudentAttendance on StudentAttendance.Studies_Student_Rollno=Student.Rollno where Studies.Course_Code='CS3002' and Studies.section ='7G' and StudentAttendance.Date='2022-12-08'";
                da = new SqlDataAdapter(commandText, conn);

                DataTable dt = new DataTable();
                da.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    rw = new ArrayList();
                    string Rollno = row["Rollno"].ToString();
                    string Name = row["Name"].ToString();
                    string att = row["att"].ToString();

                                                  
                    rw.Add(Rollno);
                    rw.Add(Name);
                    rw.Add(att);
                    regis.Add(rw);
                }
            }

            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return regis;
        }
    }
}
