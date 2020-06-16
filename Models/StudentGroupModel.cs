using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MvcProblemOfStudents.Models
{
    public class StudentGroupModel
    {
        // Количество студентов.
        [Required]
        [Range(2, 100, ErrorMessage = "Недопустимое количество студентов (2..100).")]
        public int StudentsCount { get; set; }

        // Список всех студентов.
        [Required]
        public List<StudentModel> StudentsList { get; set; }

        // Кто кому отправил сообщение.
        public Dictionary<StudentModel, StudentModel> FromStudentToStudent { get; set; }

        // Счётчик всех сообщений в группе.
        [Required]
        [Range(0, int.MaxValue)]
        public int AllMessagesCounter { get; set; }
    }
}
