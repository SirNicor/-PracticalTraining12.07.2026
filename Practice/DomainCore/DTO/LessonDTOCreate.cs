    namespace DomainCore.DTO;

    public class LessonDTOCreate
    {
        public int ID { get; set; }
        public TimeOnly DateStart { get; set; }
        public int DayOfWeek { get; set; }
        public int NumberOfWeek { get; set; }
        public string TeacherName{ get; set; }
        public string ClassroomNumber{ get; set; }
        public string SubjectName { get; set; }
        public List<string> GroupNames { get; set; } 
    }
