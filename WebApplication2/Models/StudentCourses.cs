﻿using System.ComponentModel.DataAnnotations.Schema;
using WebApplication2.Models;
using System.Collections.Generic;

public class StudentCourses
{
    public int Id { get; set; }
    public string StudentId { get; set; }
    public Users Student { get; set; }
    public int CourseId { get; set; }
    public Course Course { get; set; }
 
}
