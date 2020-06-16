using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MvcProblemOfStudents.Models;

namespace MvcProblemOfStudents.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(int count, string messages)
        {
            List<int> canSendMessagesList;
            List<string> response = new List<string>();

            // Распарсим строку в список.
            try
            {
                canSendMessagesList = messages.Split(' ').Select(x => int.Parse(x)).ToList();
            }
            catch(Exception e)
            {
                response.Add($"Неправильная последовательность чисел: {messages}." +
                    $"Ошибка: {e.Message}");
                return View();
            }

            
            // Текущая группа студентов.
            var studentGroupModel = new StudentGroupModel
            {
                StudentsCount = count,
                AllMessagesCounter = 0,
                StudentsList = new List<StudentModel>()
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(studentGroupModel);

            // Проверяем правильность данных введеннх пользователем.
            if (!Validator.TryValidateObject(studentGroupModel, context, results, true))
            {
                string err = string.Empty;
                foreach (var error in results)
                {
                    err += $" {error.ErrorMessage} ";
                }
                response.Add(err);
                return View();
            }

            // Создадим студентов внутри группы.
            for (int i = 0; i < studentGroupModel.StudentsCount; i++)
            {
                studentGroupModel.StudentsList.Add(new StudentModel());
                studentGroupModel.StudentsList[i].CanSendMessagesCount = canSendMessagesList[i];
            }

            // Поликарп первым узнал новость.
            studentGroupModel.StudentsList[0].IsGetMessage = true;

            // Сортируем студентов по убыванию количества сообщений, которые они могут послать.
            // Для того, чтобы все студенты могли получить сообщения.
            var sortedList = studentGroupModel.StudentsList.
                OrderByDescending(o => o.CanSendMessagesCount).
                ToList().Where(o => o != studentGroupModel.StudentsList[0]).Select(o => o);

            var studentModels = new List<StudentModel>();
            studentModels.Add(studentGroupModel.StudentsList[0]);
            studentModels.AddRange(sortedList.ToList());
            
            List<StudentModel> ls = sortedList.ToList();
            // Отправка студентами, знающими о зачете, сообщений
            // другим студентам, не знающим о зачете.
            // Проходим циклом по всем студентам.
            for (int i = 0; i < studentModels.Count; i++)
            {
                // Каждый студент отправляет сообщений не больше, чем позволяет его социофобия (ai).
                for(int j = 0; 
                    studentModels[i].CanSendMessagesCount != studentModels[i].SentMessagesCount; 
                    j++)
                {
                    // Если студент не получал сообщение, то 
                    // он его и не может отправить другим.
                    if(!studentModels[i].IsGetMessage)
                    {
                        break;
                    }

                    try
                    {
                        // Отправка сообщения, если студент не получил сообщение.
                        if (!studentModels[i + 1 + j].IsGetMessage)
                        {
                            studentModels[i + 1 + j].IsGetMessage = true;
                            studentModels[i].SentMessagesCount++;
                            // Поскольку считаем с нуля, а в задаче с единицы...
                            response.Add($"{i + 1} - {i + 1 + j + 1}");
                            studentGroupModel.AllMessagesCounter++;
                        }
                    }
                    catch
                    {
                        break;
                    }
                }
            }

            response.Add($"Всего сообщений отправлено: {studentGroupModel.AllMessagesCounter}");

            // Если невозможно сообщить всем студентам об экзамене.
            foreach (var st in studentModels)
            {
                if(!st.IsGetMessage)
                {
                    response.Clear();
                    response.Add($"-1");
                    response.Add($"Сообщений отправлено: {studentGroupModel.AllMessagesCounter}");
                    response.Add($"Студентов: {studentGroupModel.StudentsCount}");
                }
            }
            ViewData.Add("Response", response);
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
