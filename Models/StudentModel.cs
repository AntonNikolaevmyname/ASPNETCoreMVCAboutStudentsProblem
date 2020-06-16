using System;
using System.ComponentModel.DataAnnotations;

namespace MvcProblemOfStudents.Models
{
    public class StudentModel
    {
        // Количество сообщений, которые студент может отправить.
        [Required]
        [Range(0, int.MaxValue)]
        public int CanSendMessagesCount { get; set; }

        // Сколько отправил сообщений.
        [Range(0, int.MaxValue)]
        public int SentMessagesCount { get; set; }
        
        // Получил ли студент сообщение.
        public bool IsGetMessage { get; set; }
    }
}
