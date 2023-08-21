﻿namespace BrightBoostApplication.Models
{
    public class Expertise
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public int TutorId { get; set; }
        public Tutor Tutor { get; set; }
    }
}
