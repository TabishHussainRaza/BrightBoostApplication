namespace BrightBoostApplication.Models.ViewModel
{
    public class StudentSignUpViewModel
    {
        public int StudentSignUpId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public bool? StudentAttendanceStatus { get; set; }
    }

    // public class StudentSignUpCRUDViewModel
    // {
    //     public List<int> StudentSignUpId { get; set; }
    //     public int SessionId { get; set; }
    //     public bool RemoveAll { get; set; }
    // }
}

